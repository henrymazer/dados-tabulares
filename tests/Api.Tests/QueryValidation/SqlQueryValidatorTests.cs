using Api.QueryValidation;

namespace Api.Tests.QueryValidation;

public class SqlQueryValidatorTests
{
    private readonly SqlQueryValidator _validator = new();

    [Fact]
    public void ValidateAndNormalize_adds_limit_when_missing()
    {
        var result = _validator.ValidateAndNormalize("SELECT * FROM ibge.populacao");

        Assert.Equal("SELECT * FROM ibge.populacao LIMIT 10000", result);
    }

    [Fact]
    public void ValidateAndNormalize_rejects_dangerous_statements()
    {
        var ex = Assert.Throws<SqlQueryValidationException>(() =>
            _validator.ValidateAndNormalize("DROP TABLE ibge.populacao"));

        Assert.Contains("Only SELECT statements are allowed", ex.Message);
    }

    [Fact]
    public void ValidateAndNormalize_rejects_unqualified_relations()
    {
        var ex = Assert.Throws<SqlQueryValidationException>(() =>
            _validator.ValidateAndNormalize("SELECT * FROM populacao"));

        Assert.Contains("schema-qualified", ex.Message);
    }

    [Theory]
    [InlineData("INSERT INTO ibge.populacao VALUES (1)")]
    [InlineData("UPDATE ibge.populacao SET total = 1")]
    [InlineData("DELETE FROM ibge.populacao")]
    [InlineData("DROP TABLE ibge.populacao")]
    [InlineData("CREATE TABLE test (id int)")]
    [InlineData("ALTER TABLE ibge.populacao ADD COLUMN x int")]
    [InlineData("TRUNCATE TABLE ibge.populacao")]
    public void ValidateAndNormalize_rejects_forbidden_statements(string sql)
    {
        var ex = Assert.Throws<SqlQueryValidationException>(() => _validator.ValidateAndNormalize(sql));

        Assert.Contains("Only SELECT statements are allowed", ex.Message);
    }

    [Fact]
    public void ValidateAndNormalize_rejects_select_into()
    {
        var ex = Assert.Throws<SqlQueryValidationException>(() =>
            _validator.ValidateAndNormalize("SELECT * INTO temp_table FROM ibge.populacao"));

        Assert.Contains("SELECT INTO", ex.Message);
    }

    [Fact]
    public void ValidateAndNormalize_rejects_cte_side_effects()
    {
        var sql = """
                  WITH bad AS (
                      DELETE FROM ibge.populacao
                  )
                  SELECT * FROM bad
                  """;

        var ex = Assert.Throws<SqlQueryValidationException>(() => _validator.ValidateAndNormalize(sql));

        Assert.Contains("Only SELECT statements are allowed", ex.Message);
    }

    [Fact]
    public void ValidateAndNormalize_accepts_valid_cte()
    {
        var sql = """
                  WITH base AS (
                      SELECT municipio_id, total
                      FROM ibge.populacao
                  )
                  SELECT *
                  FROM base
                  """;

        var result = _validator.ValidateAndNormalize(sql);

        Assert.Equal("""
                     WITH base AS (
                         SELECT municipio_id, total
                         FROM ibge.populacao
                     )
                     SELECT *
                     FROM base LIMIT 10000
                     """, result);
    }

    [Fact]
    public void ValidateAndNormalize_accepts_unions()
    {
        var sql = """
                  SELECT municipio_id FROM ibge.populacao
                  UNION
                  SELECT municipio_id FROM tse.eleitores
                  """;

        var result = _validator.ValidateAndNormalize(sql);

        Assert.Equal("""
                     SELECT municipio_id FROM ibge.populacao
                     UNION
                     SELECT municipio_id FROM tse.eleitores LIMIT 10000
                     """, result);
    }

    [Fact]
    public void ValidateAndNormalize_accepts_subqueries()
    {
        var sql = """
                  SELECT municipio_id
                  FROM ibge.populacao
                  WHERE municipio_id IN (
                      SELECT municipio_id
                      FROM pnad.renda
                  )
                  """;

        var result = _validator.ValidateAndNormalize(sql);

        Assert.Contains("WHERE municipio_id IN (", result);
        Assert.Contains("SELECT municipio_id", result);
        Assert.EndsWith(" LIMIT 10000", result);
    }

    [Fact]
    public void ValidateAndNormalize_accepts_joins()
    {
        var sql = """
                  SELECT p.municipio_id, r.renda
                  FROM ibge.populacao p
                  JOIN pnad.renda r ON r.municipio_id = p.municipio_id
                  """;

        var result = _validator.ValidateAndNormalize(sql);

        Assert.Equal("""
                     SELECT p.municipio_id, r.renda
                     FROM ibge.populacao p
                     JOIN pnad.renda r ON r.municipio_id = p.municipio_id LIMIT 10000
                     """, result);
    }

    [Theory]
    [InlineData("SELECT * FROM core.segredos")]
    [InlineData("SELECT * FROM public.segredos")]
    [InlineData("SELECT * FROM mysql.segredos")]
    public void ValidateAndNormalize_rejects_disallowed_schemas(string sql)
    {
        var ex = Assert.Throws<SqlQueryValidationException>(() => _validator.ValidateAndNormalize(sql));

        Assert.Contains("is not allowed", ex.Message);
    }

    [Fact]
    public void ValidateAndNormalize_rejects_stacked_queries()
    {
        var ex = Assert.Throws<SqlQueryValidationException>(() =>
            _validator.ValidateAndNormalize("SELECT * FROM ibge.populacao; DROP TABLE ibge.populacao"));

        Assert.Contains("Multiple SQL statements", ex.Message);
    }

    [Theory]
    [InlineData("SELECT * FROM ibge.populacao -- comment")]
    [InlineData("SELECT * FROM ibge.populacao /* comment */")]
    public void ValidateAndNormalize_rejects_sql_comments(string sql)
    {
        var ex = Assert.Throws<SqlQueryValidationException>(() => _validator.ValidateAndNormalize(sql));

        Assert.Contains("comments are not allowed", ex.Message);
    }

    [Fact]
    public void ValidateAndNormalize_preserves_string_literals()
    {
        var sql = """
                  SELECT *
                  FROM ibge.populacao
                  WHERE descricao = 'foo; -- not injection'
                  """;

        var result = _validator.ValidateAndNormalize(sql);

        Assert.Equal("""
                     SELECT *
                     FROM ibge.populacao
                     WHERE descricao = 'foo; -- not injection' LIMIT 10000
                     """, result);
    }

    [Fact]
    public void ValidateAndNormalize_clamps_limit_to_maximum()
    {
        var result = _validator.ValidateAndNormalize("SELECT * FROM ibge.populacao LIMIT 15000");

        Assert.Equal("SELECT * FROM ibge.populacao LIMIT 10000", result);
    }

    [Fact]
    public void ValidateAndNormalize_keeps_limit_below_maximum()
    {
        var result = _validator.ValidateAndNormalize("SELECT * FROM ibge.populacao LIMIT 2500");

        Assert.Equal("SELECT * FROM ibge.populacao LIMIT 2500", result);
    }

    [Fact]
    public void ValidateAndNormalize_inserts_limit_before_offset()
    {
        var result = _validator.ValidateAndNormalize("SELECT * FROM ibge.populacao ORDER BY id OFFSET 25");

        Assert.Equal("SELECT * FROM ibge.populacao ORDER BY id LIMIT 10000 OFFSET 25", result);
    }
}
