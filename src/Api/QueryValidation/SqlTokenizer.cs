namespace Api.QueryValidation;

internal enum SqlTokenKind
{
    Word,
    Number,
    String,
    QuotedIdentifier,
    Symbol
}

internal readonly record struct SqlToken(
    SqlTokenKind Kind,
    string Text,
    int Start,
    int Length)
{
    public int End => Start + Length;
}

internal static class SqlTokenizer
{
    public static IReadOnlyList<SqlToken> Tokenize(string sql)
    {
        var tokens = new List<SqlToken>();
        var index = 0;

        while (index < sql.Length)
        {
            while (index < sql.Length && char.IsWhiteSpace(sql[index]))
            {
                index++;
            }

            if (index >= sql.Length)
            {
                break;
            }

            if (sql[index] == '-' && index + 1 < sql.Length && sql[index + 1] == '-')
            {
                throw new SqlQueryValidationException("SQL comments are not allowed.");
            }

            if (sql[index] == '/' && index + 1 < sql.Length && sql[index + 1] == '*')
            {
                throw new SqlQueryValidationException("SQL comments are not allowed.");
            }

            var start = index;
            var current = sql[index];

            if (current == '\'')
            {
                index++;
                while (index < sql.Length)
                {
                    if (sql[index] == '\'')
                    {
                        if (index + 1 < sql.Length && sql[index + 1] == '\'')
                        {
                            index += 2;
                            continue;
                        }

                        index++;
                        tokens.Add(new SqlToken(SqlTokenKind.String, sql[start..index], start, index - start));
                        goto nextToken;
                    }

                    index++;
                }

                throw new SqlQueryValidationException("Unterminated string literal.");
            }

            if (current == '"')
            {
                index++;
                while (index < sql.Length)
                {
                    if (sql[index] == '"')
                    {
                        if (index + 1 < sql.Length && sql[index + 1] == '"')
                        {
                            index += 2;
                            continue;
                        }

                        index++;
                        tokens.Add(new SqlToken(SqlTokenKind.QuotedIdentifier, sql[start..index], start, index - start));
                        goto nextToken;
                    }

                    index++;
                }

                throw new SqlQueryValidationException("Unterminated quoted identifier.");
            }

            if (char.IsLetter(current) || current == '_')
            {
                index++;
                while (index < sql.Length && (char.IsLetterOrDigit(sql[index]) || sql[index] == '_' || sql[index] == '$'))
                {
                    index++;
                }

                tokens.Add(new SqlToken(SqlTokenKind.Word, sql[start..index], start, index - start));
                goto nextToken;
            }

            if (char.IsDigit(current))
            {
                index++;
                while (index < sql.Length && char.IsDigit(sql[index]))
                {
                    index++;
                }

                tokens.Add(new SqlToken(SqlTokenKind.Number, sql[start..index], start, index - start));
                goto nextToken;
            }

            if (current == ';')
            {
                if (sql[start..].Trim().Length > 1)
                {
                    throw new SqlQueryValidationException("Multiple SQL statements are not allowed.");
                }

                break;
            }

            tokens.Add(new SqlToken(SqlTokenKind.Symbol, current.ToString(), start, 1));
            index++;

        nextToken:
            continue;
        }

        return tokens;
    }
}
