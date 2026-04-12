╔═════════════════════════════════════════════╗
║                                             ║
║           New update available!             ║
║          Run: coderabbit update             ║
║                                             ║
╚═════════════════════════════════════════════╝

Starting CodeRabbit review in plain text mode...

Connecting to review service
Setting up
Summarizing
Reviewing

============================================================================
File: src/Etl/appsettings.json
Line: 1 to 6
Type: potential_issue

Comment:
Move connection strings to secure configuration providers.

Connection strings should not be committed to source control, even with placeholder passwords. This creates a security risk if real credentials are accidentally committed or if the file is copied with real values.

For a .NET project, use:
- Development: User Secrets (dotnet user-secrets set "ConnectionStrings:ReadOnly" "")
- Production: Environment variables or Azure Key Vault

If you need a template in source control, consider:
1. Creating an appsettings.template.json with placeholders (not loaded by the app)
2. Adding appsettings.json to .gitignore and documenting the setup in README
3. Using only appsettings.Development.json (gitignored) for local connection strings




🔐 Example using User Secrets for development

Remove this file from source control and set secrets locally:

dotnet user-secrets init --project src/Etl
dotnet user-secrets set "ConnectionStrings:ReadOnly" "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readonly;Password=" --project src/Etl
dotnet user-secrets set "ConnectionStrings:ReadWrite" "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readwrite;Password=" --project src/Etl


For production, use environment variables:
ConnectionStrings__ReadOnly=""
ConnectionStrings__ReadWrite=""

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/appsettings.json around lines 1 - 6, The ConnectionStrings block in appsettings.json (keys "ConnectionStrings", "ReadOnly", "ReadWrite") must be removed from source control and moved to secure providers: delete or gitignore appsettings.json, add an appsettings.template.json with placeholder values for "ConnectionStrings:ReadOnly" and "ConnectionStrings:ReadWrite", document using dotnet user-secrets (for development) and environment variables/Azure Key Vault (for production) in the README, and update any startup/config code that reads configuration to fallback to IConfiguration for those keys so values come from secrets or environment variables rather than the committed file.

============================================================================
File: DadosTabulares.slnx
Line: 2
Type: refactor_suggestion

Comment:
Remove leading and trailing slashes from folder names.

The folder names /src/ and /tests/ include leading and trailing slashes, which is unconventional for Visual Studio solution files. Standard practice is to use simple folder names without slashes (e.g., src and tests). This could cause unexpected behavior in some Visual Studio versions.




♻️ Proposed fix

-  
+  
     
     
     
     
   
-  
+  
     
     
     
     
   




Also applies to: 8-8

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @DadosTabulares.slnx at line 2, The solution file contains folder entries using leading/trailing slashes (e.g., "/src/" and "/tests/"); update those Folder Name values to remove the slashes (use "src" and "tests" respectively) so the Folder elements (Folder Name="/src/") become Folder Name="src" and Folder Name="/tests/" become Folder Name="tests" to match Visual Studio conventions and avoid potential parse issues.

============================================================================
File: tests/Etl.Tests/Pnad/PnadCsvParserTests.cs
Line: 1
Type: potential_issue

Comment:
Missing using Xunit; directive.

The file uses [Fact] attribute and Assert.Collection but does not import the Xunit namespace. This will cause compilation errors. Additionally, verify that System.IO is available via implicit usings for StringReader, or add it explicitly.




🐛 Proposed fix

 using Etl.Pnad;
+using Xunit;
 
 namespace Etl.Tests.Pnad;

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @tests/Etl.Tests/Pnad/PnadCsvParserTests.cs at line 1, The test file PnadCsvParserTests is missing the Xunit import causing [Fact] and Assert.Collection to be unresolved; add a using Xunit; directive at the top of the file and also ensure StringReader is available (add using System.IO; if implicit usings are not enabled) so tests in class PnadCsvParserTests that use the [Fact] attribute and Assert.Collection and create a new StringReader compile correctly.

============================================================================
File: tests/Domain.Tests/Tse/PartidoTests.cs
Line: 1 to 4
Type: potential_issue

Comment:
Missing using Xunit; directive.

The file uses [Fact] attribute and Assert methods but doesn't import the Xunit namespace. Unless the project has implicit usings configured for Xunit, add:

 namespace Domain.Tests.Tse;

 using DadosTabulares.Domain.Tse;
+using Xunit;

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @tests/Domain.Tests/Tse/PartidoTests.cs around lines 1 - 4, The test file is missing the Xunit namespace import required for the [Fact] attribute and Assert methods; add a using Xunit; directive at the top of the file so attributes like [Fact] and calls to Assert.* in the PartidoTests test class are recognized and the tests compile.

============================================================================
File: src/Data/Infrastructure/PublicDataServiceCollectionExtensions.cs
Line: 17 to 18
Type: potential_issue

Comment:
Error message should be in English for consistency.

The exception message is in Portuguese ("não foi configurada"), which is inconsistent with the rest of the codebase and makes searching logs more difficult for international teams.




🛠️ Proposed fix

 var connectionString = configuration.GetConnectionString(connectionStringName)
-    ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' não foi configurada.");
+    ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' was not configured.");

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Infrastructure/PublicDataServiceCollectionExtensions.cs around lines 17 - 18, The InvalidOperationException message when retrieving the connection string should be in English; update the throw in the connectionString assignment (the line that uses configuration.GetConnectionString(connectionStringName) ?? throw new InvalidOperationException(...)) to use an English message such as $"Connection string '{connectionStringName}' is not configured." so the code in PublicDataServiceCollectionExtensions (the connectionString variable/assignment) emits a consistent English error message.

============================================================================
File: src/Api/appsettings.json
Line: 24
Type: potential_issue

Comment:
Major: Restrict AllowedHosts in production environments.

Setting AllowedHosts to "*" disables host header validation, which could expose the application to host header injection attacks in production.

Recommendation:
- Keep "*" for local development
- Use environment-specific configuration files (e.g., appsettings.Production.json) to specify actual allowed hostnames for production:
    "AllowedHosts": "yourdomain.com;www.yourdomain.com"

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Api/appsettings.json at line 24, Current appsettings.json sets "AllowedHosts" to "", which is unsafe for production; change configuration to use environment-specific files: keep "AllowedHosts": "" in appsettings.Development.json for local dev, add an appsettings.Production.json that sets "AllowedHosts" to your real hostnames (e.g., "yourdomain.com;www.yourdomain.com"), and ensure the app uses ASPNETCORE_ENVIRONMENT to load the correct file; update any config bootstrap if necessary so the production environment does not rely on the wildcard.

============================================================================
File: src/Api/Health/DatabaseHealthEndpoint.cs
Line: 16 to 26
Type: potential_issue

Comment:
Wrap CanConnectAsync in try-catch to ensure consistent 503 response on failures.

CanConnectAsync can throw exceptions for certain failure modes (invalid connection string, network errors, timeouts) rather than returning false. When this happens, the endpoint returns 500 instead of 503, which may confuse monitoring systems and load balancers expecting a consistent unhealthy status code.


🛡️ Proposed fix to handle exceptions

 private static async Task HandleAsync(
     PublicDataDbContext dbContext,
     CancellationToken ct)
 {
-    if (await dbContext.Database.CanConnectAsync(ct))
+    try
     {
-        return TypedResults.Ok(new HealthResponse("healthy"));
+        if (await dbContext.Database.CanConnectAsync(ct))
+        {
+            return TypedResults.Ok(new HealthResponse("healthy"));
+        }
+    }
+    catch (Exception)
+    {
+        // Connection attempt failed with exception
     }
 
     return TypedResults.Json(new HealthResponse("unhealthy"), statusCode: StatusCodes.Status503ServiceUnavailable);
 }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Api/Health/DatabaseHealthEndpoint.cs around lines 16 - 26, The HandleAsync method calls dbContext.Database.CanConnectAsync which can throw; wrap that call in a try-catch inside HandleAsync (catch Exception) and on any exception return TypedResults.Json(new HealthResponse("unhealthy"), statusCode: StatusCodes.Status503ServiceUnavailable) so failures produce a consistent 503; optionally log the exception via whatever logger is available, but ensure the catch covers CanConnectAsync and returns the same 503/HealthResponse("unhealthy") as the false path.

============================================================================
File: src/Data/Infrastructure/PublicDataDbContextFactory.cs
Line: 10 to 12
Type: potential_issue

Comment:
Avoid hard-coded credentials in source code.

The connection string contains plaintext credentials (Username=postgres;Password=postgres). Even for design-time factories used only during development, committing credentials to source control poses a security risk and violates best practices.

Consider using environment variables or .NET user secrets:




🔒 Proposed fix using environment variables

 public PublicDataDbContext CreateDbContext(string[] args)
 {
     var builder = new DbContextOptionsBuilder();
-    builder.UseNpgsql("Host=localhost;Port=5432;Database=dados_publicos;Username=postgres;Password=postgres");
+    var connectionString = Environment.GetEnvironmentVariable("PUBLIC_DATA_CONNECTION_STRING")
+        ?? "Host=localhost;Port=5432;Database=dados_publicos;Username=postgres;Password=postgres";
+    builder.UseNpgsql(connectionString);
     return new PublicDataDbContext(builder.Options);
 }




Alternatively, document that this factory is strictly for local development and ensure .gitignore or secret scanning is in place if using a shared repository.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Infrastructure/PublicDataDbContextFactory.cs around lines 10 - 12, The PublicDataDbContextFactory currently hard-codes DB credentials in the UseNpgsql call; change it to read the connection string from a secure source (e.g., an environment variable or IConfiguration/user secrets) instead of inlining plaintext credentials. Update the code that builds DbContextOptions (the builder and the UseNpgsql call in PublicDataDbContextFactory) to fetch a connection string like ENV["PUBLIC_DB_CONNECTION"] (or IConfiguration.GetConnectionString("PublicData")) and fail fast with a clear error if the value is missing; ensure no credentials remain in source and document that this factory is for local dev if you still want a default fallback stored out-of-repo.

============================================================================
File: tests/Domain.Tests/Tse/ResultadoEleitoralTests.cs
Line: 65 to 69
Type: potential_issue

Comment:
Same ZonaEleitoral instance inconsistency.

Same issue—CriarZona() is called twice, creating mismatched instances.




🔧 Proposed fix

     [Fact]
     public void Deve_aceitar_zero_votos()
     {
+        var zona = CriarZona();
         var resultado = new ResultadoEleitoral(
             candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
-            turno: 1, zonaEleitoral: CriarZona(),
-            secaoEleitoral: new SecaoEleitoral(42, CriarZona()),
+            turno: 1, zonaEleitoral: zona,
+            secaoEleitoral: new SecaoEleitoral(42, zona),
             quantidadeVotos: 0);
 
         Assert.Equal(0, resultado.QuantidadeVotos);
     }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @tests/Domain.Tests/Tse/ResultadoEleitoralTests.cs around lines 65 - 69, The test constructs ResultadoEleitoral using two separate CriarZona() calls which produce different ZonaEleitoral instances and makes SecaoEleitoral and ResultadoEleitoral reference different zones; change the test to create a single ZonaEleitoral instance (e.g., var zona = CriarZona()) and pass that same zona into both the ResultadoEleitoral constructor and the SecaoEleitoral creation so ResultadoEleitoral, SecaoEleitoral and zona are consistent.

============================================================================
File: src/Data/PublicDataDbContext.cs
Line: 154 to 169
Type: potential_issue

Comment:
Same concern: description field in composite primary key.

PrestacaoContasRecord also uses Descricao (500 chars) in its 5-part composite key, compounding the index size concern. Combined with other string fields in the key, this risks exceeding PostgreSQL's index row size limit.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/PublicDataDbContext.cs around lines 154 - 169, PrestacaoContasRecord is using Descricao (500 chars) as part of a 5-column composite primary key which risks exceeding PostgreSQL index size limits; change the entity configuration to remove Descricao from the composite key and introduce a surrogate primary key (e.g., add an Id property and make it the HasKey) in the modelBuilder for PrestacaoContasRecord, configure Id as value-generated (Identity) and keep a separate unique index or alternate key on the necessary identifying columns (exclude Descricao) if uniqueness is required, leaving Descricao configured as a regular Property with HasMaxLength(500).

============================================================================
File: src/Data/PublicDataDbContext.cs
Line: 138 to 152
Type: potential_issue

Comment:
Using description field in composite primary key may cause issues.

Descricao (up to 500 characters) is part of the composite primary key. This can lead to:
- PostgreSQL btree index size limits (~2712 bytes) potentially being exceeded
- Performance degradation for key lookups
- Semantically, descriptions are typically not stable identifiers

Consider introducing a surrogate key or a hash/identifier column instead of using the full description text.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/PublicDataDbContext.cs around lines 138 - 152, The composite primary key on BemDeclaradoRecord currently includes Descricao (modelBuilder.Entity HasKey(..., x.Descricao)), which can exceed index size limits and is semantically unstable; change the key to avoid using the full Descricao by introducing a surrogate identifier or stable hash: add a new property on BemDeclaradoRecord (e.g., Id or DescricaoHash), populate it (GUID/sequence for Id or a fixed-length hash for DescricaoHash), update the modelBuilder HasKey to use the new Id or the fixed-length hash instead of Descricao, and keep Descricao as a non-key property (optionally indexed) with its current configuration (HasMaxLength(500)). Ensure any code that relied on the old composite key is updated to use the new identifier.

============================================================================
File: tests/Domain.Tests/Tse/ResultadoEleitoralTests.cs
Line: 45 to 49
Type: potential_issue

Comment:
Inconsistent ZonaEleitoral instances between SecaoEleitoral and ResultadoEleitoral.

CriarZona() is called twice, creating different ZonaEleitoral instances—one for zonaEleitoral (line 47) and another inside SecaoEleitoral (line 48). This differs from the pattern in Deve_criar_resultado_eleitoral_valido which correctly shares one instance. If the domain validates that secaoEleitoral.ZonaEleitoral == zonaEleitoral, the test may fail for the wrong reason or mask a real bug.




🔧 Proposed fix

     [Theory]
     [InlineData(0)]
     [InlineData(3)]
     [InlineData(-1)]
     public void Deve_rejeitar_turno_invalido(int turno)
     {
+        var zona = CriarZona();
         Assert.Throws(() => new ResultadoEleitoral(
             candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
-            turno: turno, zonaEleitoral: CriarZona(),
-            secaoEleitoral: new SecaoEleitoral(42, CriarZona()),
+            turno: turno, zonaEleitoral: zona,
+            secaoEleitoral: new SecaoEleitoral(42, zona),
             quantidadeVotos: 100));
     }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @tests/Domain.Tests/Tse/ResultadoEleitoralTests.cs around lines 45 - 49, The test constructs two different ZonaEleitoral instances by calling CriarZona() twice, which can make ResultadoEleitoral validation fail incorrectly; fix by creating one ZonaEleitoral variable (var zona = CriarZona()) and pass that same zona into both the ResultadoEleitoral constructor (zonaEleitoral parameter) and into the SecaoEleitoral constructor so the SecaoEleitoral.ZonaEleitoral and the ResultadoEleitoral.zonaEleitoral are the same instance when invoking new ResultadoEleitoral(...) and new SecaoEleitoral(...).

============================================================================
File: .vscode/tasks.json
Line: 28 to 39
Type: potential_issue

Comment:
dotnet watch run is not appropriate for a test project.

The watch task uses dotnet watch run, but test projects don't have an entry point to run. This will likely fail or produce unexpected behavior.

If you want hot-reload for tests, use dotnet watch test. If you want to watch the main API project, update the path to target src/Api/Api.csproj instead.



🛠️ Proposed fix for watching tests

         {
             "label": "watch",
             "command": "dotnet",
             "type": "process",
             "args": [
                 "watch",
-                "run",
+                "test",
                 "--project",
                 "${workspaceFolder}/tests/Api.Tests/Api.Tests.csproj"
             ],
             "problemMatcher": "$msCompile"
         }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @.vscode/tasks.json around lines 28 - 39, The "watch" task currently calls dotnet with args ["watch","run","--project","${workspaceFolder}/tests/Api.Tests/Api.Tests.csproj"] which is invalid for a test project; update the task labeled "watch" so it either uses ["watch","test","--project","${workspaceFolder}/tests/Api.Tests/Api.Tests.csproj"] to enable hot-reload for tests, or point the existing ["watch","run","--project",...] args at the main API project (e.g., replace the project path with the API csproj) depending on whether you intend to watch tests or the API.

============================================================================
File: src/Api/appsettings.json
Line: 2 to 5
Type: potential_issue

Comment:
Critical: Replace placeholder passwords and avoid storing credentials in configuration files.

The connection strings have multiple security concerns:

1. Placeholder passwords: Both connection strings use CHANGE_ME as the password, which will cause connection failures at runtime.
2. Plaintext credentials: Storing database credentials in appsettings.json is a security risk, especially if this file is committed to source control.

Recommended approach:
- Use User Secrets for local development
- Use environment variables or a secrets management service (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault) for production
- Remove the Password parameter from these connection strings and inject it at runtime





🔒 Proposed fix: Remove passwords from appsettings.json

   "ConnectionStrings": {
-    "ReadOnly": "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readonly;Password=CHANGE_ME",
-    "ReadWrite": "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readwrite;Password=CHANGE_ME"
+    "ReadOnly": "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readonly",
+    "ReadWrite": "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readwrite"
   },


Then configure passwords via environment variables or User Secrets:
{
  "ConnectionStrings:ReadOnly": "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readonly;Password=",
  "ConnectionStrings:ReadWrite": "Host=localhost;Port=5432;Database=dados_publicos;Username=api_readwrite;Password="
}

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Api/appsettings.json around lines 2 - 5, Remove plaintext passwords from the "ConnectionStrings" entries in appsettings.json (the "ReadOnly" and "ReadWrite" values) and stop committing secrets; instead delete the Password=... segments and load/inject the actual passwords at runtime via User Secrets for local dev or environment variables / a secrets manager (e.g., Azure Key Vault) for production, ensuring the application reads the secret-backed password when building the final connection string for DB access (look for code that reads Configuration["ConnectionStrings:ReadOnly"] / Configuration["ConnectionStrings:ReadWrite"] and update wiring to append the secret password at runtime).

============================================================================
File: src/Data/PersistenceRecords.cs
Line: 27 to 39
Type: potential_issue

Comment:
CPF is sensitive PII - ensure proper handling.

Cpf (Brazilian tax ID) is highly sensitive personal data. Verify that:
- Logging/serialization of these records masks or excludes the CPF field.
- Access to persisted data containing CPF is appropriately restricted.
- Data retention policies comply with LGPD (Brazilian data protection law).

Consider adding a [JsonIgnore] or similar attribute if this field should be excluded from default serialization, or document the intended handling.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/PersistenceRecords.cs around lines 27 - 39, CandidatoRecord contains a sensitive Cpf property; update handling by adding a serialization exclusion (e.g., apply [JsonIgnore] or equivalent attribute to the Cpf property in the CandidatoRecord class), ensure any ToString/diagnostic methods do not include Cpf (mask or omit it), and add a brief comment documenting that CPF must be stored/accessed only via restricted/encrypted persistence and excluded from logs; also verify callers that persist or log CandidatoRecord respect the new attribute/contract.

============================================================================
File: src/Data/Repositories/Ibge/IbgeRepositories.cs
Line: 18 to 24
Type: potential_issue

Comment:
Potential NullReferenceException on siglaUf parameter.

If siglaUf is null, calling ToUpperInvariant() throws a NullReferenceException. This pattern repeats in all six repositories (lines 21, 46, 71, 96, 121, 146).

Consider adding a guard clause or using the null-conditional operator:



🛡️ Proposed fix with guard clause

 public async Task> ObterPorUfAsync(string siglaUf, CancellationToken ct = default)
+{
+    ArgumentNullException.ThrowIfNull(siglaUf);
     => await context.DadosPopulacionais
         .AsNoTracking()
         .Where(x => x.UfSigla == siglaUf.ToUpperInvariant())
         .OrderByDescending(x => x.Quantidade)
         .Select(x => x.ToDomain())
         .ToListAsync(ct);
+}

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Repositories/Ibge/IbgeRepositories.cs around lines 18 - 24, The method ObterPorUfAsync calls siglaUf.ToUpperInvariant() without checking for null/empty; add a guard at the start of ObterPorUfAsync to validate siglaUf (e.g., if string.IsNullOrWhiteSpace(siglaUf) throw new ArgumentNullException(nameof(siglaUf)) or ArgumentException for empty) before using ToUpperInvariant, and apply the same guard to the other repository methods that call siglaUf.ToUpperInvariant() so none of the six repository methods can throw a NullReferenceException.

============================================================================
File: src/Etl/Pnad/PnadCsvRow.cs
Line: 3
Type: potential_issue

Comment:
Add validation for Trimestre to ensure data integrity.

The Trimestre property represents a quarter but accepts any integer value. This allows invalid quarters (e.g., 0, 5, -1, 999) to pass through, potentially corrupting the ETL pipeline with bad data.




✅ Proposed fix to validate quarter range

-public sealed record PnadCsvRow(string UfSigla, int Ano, int Trimestre, decimal Valor);
+public sealed record PnadCsvRow(string UfSigla, int Ano, int Trimestre, decimal Valor)
+{
+    public PnadCsvRow
+    {
+        if (Trimestre is  4)
+            throw new ArgumentOutOfRangeException(nameof(Trimestre), Trimestre, "Trimestre must be between 1 and 4.");
+    }
+}

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Pnad/PnadCsvRow.cs at line 3, The PnadCsvRow positional record currently allows any integer for Trimestre; update PnadCsvRow to validate Trimestre is between 1 and 4 in the record body (e.g., add a validation block in the positional record definition for PnadCsvRow) and throw an ArgumentOutOfRangeException (including the parameter name "Trimestre" and a clear message) when out of range so invalid quarters cannot be constructed.

============================================================================
File: src/Etl/Pnad/PnadCsvParser.cs
Line: 137 to 152
Type: potential_issue

Comment:
Ambiguous decimal parsing for values like "1.234".

The heuristic may misinterpret values with dots. For example, "1.234" in pt-BR format (where dot is thousands separator) represents 1234, but the invariant culture parse on line 146 would interpret it as 1.234. This could lead to values being off by a factor of 1000.

Consider adding logic to detect pt-BR dot-as-thousands-separator format, or document the expected input format requirement.




🛠️ Proposed fix with additional heuristic

     private static decimal ParseDecimal(string value)
     {
         var style = NumberStyles.Number;
 
+        // pt-BR format: comma as decimal separator
         if (value.Contains(',') && !value.Contains('.'))
         {
             return decimal.Parse(value, style, CultureInfo.GetCultureInfo("pt-BR"));
         }
 
+        // pt-BR format with thousands separator: e.g., "1.234,56"
+        if (value.Contains(',') && value.Contains('.') && value.LastIndexOf('.')

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Pnad/PnadCsvParser.cs around lines 137 - 152, The ParseDecimal method can misinterpret values like "1.234" (pt-BR thousands sep) as 1.234 under InvariantCulture; update ParseDecimal to detect dot-as-thousands-separator patterns before choosing cultures: if value contains '.' and not ',' and matches a thousands-group regex like ^\d{1,3}(\.\d{3})+(,\d+)?$ then parse using CultureInfo.GetCultureInfo("pt-BR"); otherwise fall back to decimal.TryParse with CultureInfo.InvariantCulture and then as a last resort pt-BR. Reference the ParseDecimal method, NumberStyles, CultureInfo.GetCultureInfo("pt-BR") and CultureInfo.InvariantCulture when implementing the heuristic.

============================================================================
File: src/Api/Auth/JwtAuthentication.cs
Line: 79 to 82
Type: potential_issue

Comment:
Insecure default signing key poses production risk.

The default SigningKey value is acceptable for local development but could be deployed to production if the configuration is missing. Consider throwing an exception in non-development environments when the default key is detected.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Api/Auth/JwtAuthentication.cs around lines 79 - 82, The JwtAuthentication.SigningKey has an insecure development default that could be used in production; update the startup/config validation to detect when JwtAuthentication.SigningKey equals "development-signing-key-change-me" and, when the environment is not development (use IHostEnvironment/EnvironmentName or ASPNETCORE_ENVIRONMENT), throw an exception or fail fast with a clear error message requiring a real signing key; ensure this check runs early (during configuration binding or in the component that reads JwtAuthentication) so the app won't start in non-development without an overridden SigningKey.

============================================================================
File: src/Etl/Tse/TseIngestionLoader.cs
Line: 77 to 81
Type: potential_issue

Comment:
Critical performance issue: Loading entire table into memory.

This code fetches all records from the database table into memory to build a key set. For large electoral datasets (potentially millions of rows), this will cause memory exhaustion, timeouts, and extremely slow performance.

Consider alternative approaches:
1. Query only the keys using a projection: set.Select(keySelector).ToHashSetAsync()
2. Use batch lookups with WHERE key IN (...) for the incoming records
3. Use database-side EXCEPT or NOT EXISTS queries
4. For very large datasets, consider chunked processing with database-side deduplication




🛠️ Suggested approach: Project keys only

If you must load keys into memory, project only the key columns to reduce memory footprint significantly:

// Example for ResultadosEleitorais - create a method per entity or use expression trees
var existingKeys = await set
    .AsNoTracking()
    .Select(r => keySelector(r))
    .ToHashSetAsync(ct);


However, for truly large datasets, consider a chunked approach that queries existence in batches against the database.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Tse/TseIngestionLoader.cs around lines 77 - 81, The current code builds existingKeys by loading entire entities via set.AsNoTracking().ToListAsync(ct) then selecting keys, which will OOM for large tables; change to project only the key values server-side (use set.AsNoTracking().Select(r => keySelector(r)).ToHashSetAsync(ct)) or implement batched existence checks (WHERE key IN (...)) / NOT EXISTS/EXCEPT queries instead of ToListAsync, referencing the existingKeys variable, the set DbSet, and the keySelector to locate and replace the ToListAsync call; for very large datasets use chunked processing with database-side deduplication.

============================================================================
File: src/Data/Repositories/RepositoryMapping.cs
Line: 287 to 295
Type: potential_issue

Comment:
Add null guards to prevent runtime exceptions.

SerializePartidos will throw NullReferenceException if partidos is null. DeserializePartidos will throw ArgumentNullException if json is null—the null-coalescing operator only handles a null deserialization result, not null input.




🛡️ Proposed fix to add null guards

 public static string SerializePartidos(IReadOnlyList partidos)
-    => JsonSerializer.Serialize(partidos.Select(x => new PartidoDto(x.Numero, x.Sigla, x.Nome)));
+    => partidos is null 
+        ? "[]" 
+        : JsonSerializer.Serialize(partidos.Select(x => new PartidoDto(x.Numero, x.Sigla, x.Nome)));

 public static IReadOnlyList DeserializePartidos(string json)
-    => JsonSerializer.Deserialize>(json)?.Select(x => new Partido(x.Numero, x.Sigla, x.Nome)).ToList()
-       ?? [];
+    => string.IsNullOrEmpty(json) 
+        ? [] 
+        : JsonSerializer.Deserialize>(json)?.Select(x => new Partido(x.Numero, x.Sigla, x.Nome)).ToList() ?? [];

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Repositories/RepositoryMapping.cs around lines 287 - 295, Add explicit null guards for the public methods: in SerializePartidos validate that partidos is not null and throw ArgumentNullException(nameof(partidos)) (or return an empty JSON array if you prefer a permissive behavior), and in DeserializePartidos validate that json is not null and throw ArgumentNullException(nameof(json)) before calling JsonSerializer.Deserialize; keep the existing fallback for a null deserialization result (the ?? [] behavior). These checks should be added to the methods SerializePartidos and DeserializePartidos (the PartidoDto record can stay unchanged).

============================================================================
File: tests/Domain.Tests/Tse/ResultadoEleitoralTests.cs
Line: 55 to 59
Type: potential_issue

Comment:
Same ZonaEleitoral instance inconsistency.

Same issue as above—CriarZona() is called twice, creating mismatched instances.




🔧 Proposed fix

     [Fact]
     public void Deve_rejeitar_votos_negativos()
     {
+        var zona = CriarZona();
         Assert.Throws(() => new ResultadoEleitoral(
             candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
-            turno: 1, zonaEleitoral: CriarZona(),
-            secaoEleitoral: new SecaoEleitoral(42, CriarZona()),
+            turno: 1, zonaEleitoral: zona,
+            secaoEleitoral: new SecaoEleitoral(42, zona),
             quantidadeVotos: -1));
     }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @tests/Domain.Tests/Tse/ResultadoEleitoralTests.cs around lines 55 - 59, The test constructs ResultadoEleitoral with two distinct ZonaEleitoral instances because CriarZona() is called twice; create a single ZonaEleitoral instance (e.g., var zona = CriarZona()) and pass that same instance both as the zonaEleitoral argument to ResultadoEleitoral and as the zone parameter when constructing SecaoEleitoral so the objects are consistent; update the test that calls new ResultadoEleitoral(...) to reuse the single zona variable and keep the negative quantidadeVotos assertion unchanged.

============================================================================
File: src/Api/Auth/JwtAuthentication.cs
Line: 13 to 14
Type: potential_issue

Comment:
Add validation for signing key.

If SigningKey is empty or too short, the application will fail with cryptographic errors at runtime. Consider adding a guard clause to fail fast during startup with a clear error message.



🛡️ Proposed fix to validate signing key

         var options = configuration.GetSection(JwtAuthOptions.SectionName).Get() ?? new JwtAuthOptions();
+
+        if (string.IsNullOrWhiteSpace(options.SigningKey) || options.SigningKey.Length

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Api/Auth/JwtAuthentication.cs around lines 13 - 14, After reading JwtAuthOptions (variable options) in JwtAuthentication.cs, add a guard that validates options.SigningKey is not null/empty and meets a minimum length (e.g., at least 16 characters/bytes) before creating the SymmetricSecurityKey (variable signingKey); if the key is invalid throw an explicit InvalidOperationException (or similar) with a clear message referencing JwtAuthOptions.SigningKey so the app fails fast during startup instead of producing cryptographic errors at runtime.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.cs
Line: 252 to 265
Type: potential_issue

Comment:
Identity generation on Numero is incorrect for party numbers.

Brazilian political party numbers are assigned by TSE (e.g., PT=13, PSDB=45, PL=22) and are not sequential auto-generated values. Remove the identity annotation.


Proposed fix

 columns: table => new
 {
-    Numero = table.Column(type: "integer", nullable: false)
-        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
+    Numero = table.Column(type: "integer", nullable: false),
     Sigla = table.Column(type: "character varying(20)", maxLength: 20, nullable: false),

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.cs around lines 252 - 265, The migration currently configures the partidos table's Numero column as an identity (see migrationBuilder.CreateTable and the Numero column with Npgsql:ValueGenerationStrategy.IdentityByDefaultColumn); remove the identity annotation so Numero is a plain integer (not auto-generated) and keep it as the primary key (table.PrimaryKey("PK_partidos", x => x.Numero)); update the column definition to nullable: false without the Annotation for Npgsql:ValueGenerationStrategy.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.cs
Line: 237 to 250
Type: potential_issue

Comment:
Identity generation on CodigoIbge is incorrect.

IBGE municipality codes are externally assigned 7-digit identifiers (e.g., 3550308 for São Paulo), not auto-generated values. The IdentityByDefaultColumn annotation should be removed to allow explicit insertion of real IBGE codes.


Proposed fix

 columns: table => new
 {
-    CodigoIbge = table.Column(type: "integer", nullable: false)
-        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
+    CodigoIbge = table.Column(type: "integer", nullable: false),
     Nome = table.Column(type: "character varying(200)", maxLength: 200, nullable: false),

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.cs around lines 237 - 250, The CodigoIbge column is incorrectly configured as an identity; remove the .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn) from the CreateTable call so CodigoIbge is not auto-generated (keep nullable: false and type "integer"), and ensure the entity mapping for CodigoIbge (e.g., the Municipio class/property CodigoIbge or its EF Core configuration) is marked as non-generated (DatabaseGeneratedOption.None or ValueGeneratedNever) so real 7-digit IBGE codes can be inserted explicitly.

============================================================================
File: src/Api/Auth/JwtAuthentication.cs
Line: 35 to 36
Type: potential_issue

Comment:
Policy ModuleAccess is never registered.

The ModuleAccessHandler is registered, but the policy AuthPolicies.ModuleAccess referenced on line 56 is never defined. This will cause a runtime InvalidOperationException when the /auth/modules/{module} endpoint is hit.



🐛 Proposed fix to register the policy with its requirement

-        services.AddAuthorization();
+        services.AddAuthorization(options =>
+        {
+            options.AddPolicy(AuthPolicies.ModuleAccess, policy =>
+                policy.Requirements.Add(new ModuleAccessRequirement()));
+        });

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Api/Auth/JwtAuthentication.cs around lines 35 - 36, The ModuleAccess policy referenced by AuthPolicies.ModuleAccess is not registered even though ModuleAccessHandler is added; update the services.AddAuthorization call to register the policy and attach its requirement so the ModuleAccessHandler has something to handle (e.g., in services.AddAuthorization(options => options.AddPolicy(AuthPolicies.ModuleAccess, policy => policy.Requirements.Add(new ModuleAccessRequirement())))); ensure the ModuleAccessRequirement type (or the correct requirement class used by ModuleAccessHandler) is used so that ModuleAccessHandler can be invoked for that policy.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.cs
Line: 23 to 34
Type: potential_issue

Comment:
Identity generation on Ano column is inappropriate for election years.

Election years are externally defined values (e.g., 2018, 2020, 2022, 2024), not auto-generated sequential integers. Using IdentityByDefaultColumn here is semantically incorrect and could cause issues when inserting specific year values.

Remove the identity annotation from this column since the values should be explicitly provided.


Proposed fix

 columns: table => new
 {
-    Ano = table.Column(type: "integer", nullable: false)
-        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
+    Ano = table.Column(type: "integer", nullable: false)
 },

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.cs around lines 23 - 34, The Ano column in the migration's CreateTable call is wrongly configured with an identity generation strategy; remove the .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn) so Ano is a plain integer that must be provided explicitly. Locate the migrationBuilder.CreateTable for "anos_eleicao" and update the columns definition for Ano (the table.Column(type: "integer", nullable: false) expression) to omit any NpgsqlValueGenerationStrategy identity annotation and ensure the primary key (PK_anos_eleicao) remains on Ano.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 150 to 166
Type: potential_issue

Comment:
Missing foreign key for AnoEleicao.

The AnoEleicao field references AnoEleicaoRecord but no foreign key relationship is configured.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 150 - 166, The ColigacaoRecord mapping is missing a foreign key constraint for AnoEleicao; update the entity configuration for "Data.ColigacaoRecord" to configure a relationship to "Data.AnoEleicaoRecord" by adding a HasOne(...).WithMany(...).HasForeignKey("AnoEleicao") (and optionally configure OnDelete behavior) so the AnoEleicao property is mapped as an FK to the AnoEleicaoRecord primary key; ensure this change appears alongside the existing property definitions (Nome, AnoEleicao, PartidosJson) in the modelBuilder.Entity("Data.ColigacaoRecord", ...) block so the migration snapshot includes the FK relationship.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 196 to 198
Type: potential_issue

Comment:
Multiple entities have nullable fields in composite primary keys.

The following entities include nullable string fields as part of their composite primary keys:
- DadoEscolaridadeRecord: NivelEscolaridade (line 196-198) in key at line 213
- DadoInfraestruturaRecord: TipoInfraestrutura (line 248-250) in key at line 265
- DadoPopulacionalRecord: FaixaEtaria and Raca (lines 277-283) in key at line 298
- DadoRendaRecord: FaixaRenda (line 333-335) in key at line 350
- DadoSaneamentoRecord: TipoSaneamento (line 362-364) in key at line 379
- DadoUrbanizacaoRecord: TipoArea (line 391-393) in key at line 408

Primary key components must be non-nullable to ensure uniqueness and data integrity.



Also applies to: 248-250, 277-283, 333-335, 362-364, 391-393

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 196 - 198, Several entities use nullable string properties inside composite primary keys which must be non-nullable; update each entity/property to be required (non-nullable) and adjust mappings/migrations accordingly: change DadoEscolaridadeRecord.NivelEscolaridade, DadoInfraestruturaRecord.TipoInfraestrutura, DadoPopulacionalRecord.FaixaEtaria and DadoPopulacionalRecord.Raca, DadoRendaRecord.FaixaRenda, DadoSaneamentoRecord.TipoSaneamento, and DadoUrbanizacaoRecord.TipoArea to non-nullable (remove nullable annotations, set IsRequired/HasColumnType without nullable and HasMaxLength where needed), regenerate or add a migration so the snapshot (PublicDataDbContextModelSnapshot) and database schema reflect the non-nullable columns and composite key definitions, and run/update any code that constructs these entities to ensure those properties are always provided.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 100 to 148
Type: potential_issue

Comment:
Missing foreign key relationships.

This entity references AnoEleicao, UfSigla, and PartidoNumero/PartidoSigla without configured foreign keys, preventing database-level referential integrity enforcement.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 100 - 148, The CandidatoRecord entity in the model snapshot is missing FK configurations for AnoEleicao, UfSigla and the Partido keys; update the modelBuilder.Entity("Data.CandidatoRecord", ...) mapping to add proper relationships (HasOne/WithMany or HasMany/WithOne) and HasForeignKey calls pointing AnoEleicao to the elections entity, UfSigla to the estado/uf entity, and PartidoNumero/PartidoSigla to the partido entity (or create a composite FK mapping) so EF Core generates the corresponding foreign key constraints; reference the Data.CandidatoRecord mapping and the property names AnoEleicao, UfSigla, PartidoNumero and PartidoSigla when adding the FK configurations.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 47 to 53
Type: potential_issue

Comment:
Nullable fields in composite primary key.

The composite primary key (line 95) includes TipoBem and Descricao, which are both nullable fields. Primary key components must be non-nullable to ensure entity uniqueness and referential integrity.



🔧 Recommended fix

In the entity configuration class for BemDeclaradoRecord, ensure these properties are marked as required:

b.Property("TipoBem")
    .IsRequired()  // Add this
    .HasMaxLength(200)
    .HasColumnType("character varying(200)");

b.Property("Descricao")
    .IsRequired()  // Add this
    .HasMaxLength(500)
    .HasColumnType("character varying(500)");

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 47 - 53, The composite primary key for BemDeclaradoRecord currently uses nullable properties TipoBem and Descricao; make both properties non-nullable by marking them required in the entity configuration for BemDeclaradoRecord (add IsRequired() to the TipoBem and Descricao property configurations) so the composite key components cannot be null and preserve referential integrity.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 477 to 487
Type: potential_issue

Comment:
Nullable fields in composite primary key.

The composite primary key for PerfilEleitoradoRecord (line 502) includes FaixaEtaria, Escolaridade, and Genero, which are all nullable. Primary key components must be non-nullable.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 477 - 487, The composite PK for PerfilEleitoradoRecord includes FaixaEtaria, Escolaridade, and Genero but those properties are configured as nullable; primary key columns must be non-nullable. Update the model configuration so each of the properties FaixaEtaria, Escolaridade, and Genero is non-nullable (e.g., add .IsRequired() to the Property builder for "FaixaEtaria", "Escolaridade", and "Genero" in the PerfilEleitoradoRecord entity configuration and regenerate the migration/snapshot), ensuring the snapshot and any migration code reflect the non-nullable change.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 38 to 98
Type: potential_issue

Comment:
Missing foreign key relationships.

This entity references CandidatoCpf, AnoEleicao, UfSigla, and PartidoNumero but no foreign key relationships are configured. This causes:
- No database-level referential integrity enforcement
- Potential orphaned records
- Missing indexes that FKs would automatically create

Configure relationships in the entity's IEntityTypeConfiguration class.



📋 Example foreign key configuration

modelBuilder.Entity("Data.BemDeclaradoRecord", b =>
{
    // ... existing configuration ...
    
    b.HasOne("Data.CandidatoRecord")
        .WithMany()
        .HasForeignKey("CandidatoCpf", "AnoEleicao")
        .OnDelete(DeleteBehavior.Cascade);
    
    b.HasOne("Data.UfRecord")
        .WithMany()
        .HasForeignKey("UfSigla")
        .OnDelete(DeleteBehavior.Restrict);
    
    b.HasOne("Data.PartidoRecord")
        .WithMany()
        .HasForeignKey("PartidoNumero")
        .OnDelete(DeleteBehavior.Restrict);
});

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 38 - 98, The BemDeclaradoRecord mapping is missing FK relations: update its IEntityTypeConfiguration (and regenerate the snapshot) to add b.HasOne("Data.CandidatoRecord").WithMany().HasForeignKey("CandidatoCpf", "AnoEleicao").OnDelete(DeleteBehavior.Cascade); add b.HasOne("Data.UfRecord").WithMany().HasForeignKey("UfSigla").OnDelete(DeleteBehavior.Restrict); and add b.HasOne("Data.PartidoRecord").WithMany().HasForeignKey("PartidoNumero").OnDelete(DeleteBehavior.Restrict) so the CandidatoRecord (composite key CandidatoCpf,AnoEleicao), UfRecord (UfSigla) and PartidoRecord (PartidoNumero) are enforced and indexed. Ensure property names match the existing properties in BemDeclaradoRecord and then regenerate migrations so PublicDataDbContextModelSnapshot reflects the new FKs.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 518 to 528
Type: potential_issue

Comment:
Nullable fields in composite primary key.

The composite primary key for PrestacaoContasRecord (line 570) includes TipoReceita, Descricao, and TipoMovimentacao, which are all nullable. Primary key components must be non-nullable.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 518 - 528, The composite primary key for PrestacaoContasRecord includes nullable properties (TipoReceita, Descricao, TipoMovimentacao); make these key columns non-nullable by updating the model and migration: change the CLR properties in the PrestacaoContasRecord model to non-nullable strings (or add [Required]) and modify the migration/snapshot entries for TipoReceita, Descricao, and TipoMovimentacao to include .IsRequired() (or otherwise set nullable: false) so the columns are created as NOT NULL before keeping them in the composite key definition.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 168 to 413
Type: potential_issue

Comment:
Missing foreign key relationships across PNAD and IBGE entities.

Multiple entities reference UfSigla, MunicipioCodigoIbge, and TrimestreAno/TrimestreNumero without configured foreign keys:
- PNAD entities (DadoDesempregoRecord, DadoInformalidadeRecord, DadoRendaMediaRecord) → missing FKs to UfRecord and TrimestreRecord
- IBGE entities (DadoEscolaridadeRecord, DadoInfraestruturaRecord, DadoPopulacionalRecord, DadoRendaRecord, DadoSaneamentoRecord, DadoUrbanizacaoRecord) → missing FKs to MunicipioRecord and UfRecord

This prevents database-level referential integrity enforcement.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 168 - 413, Several entities lack configured foreign keys: add relationship configurations in the modelBuilder for the PNAD entities DadoDesempregoRecord, DadoInformalidadeRecord, DadoRendaMediaRecord to reference UfRecord (single-column FK UfSigla) and TrimestreRecord (composite FK TrimestreAno+TrimestreNumero), and for the IBGE entities DadoEscolaridadeRecord, DadoInfraestruturaRecord, DadoPopulacionalRecord, DadoRendaRecord, DadoSaneamentoRecord, DadoUrbanizacaoRecord to reference MunicipioRecord (FK MunicipioCodigoIbge) and UfRecord (FK UfSigla). Implement using HasOne(...).WithMany(...) and HasForeignKey(...) (use HasForeignKey(new[] { "TrimestreAno", "TrimestreNumero" }) for the composite trimestre FKs) and ensure principal key names match the referenced entities' key properties. Ensure these relationship calls are added inside the respective modelBuilder.Entity(...) lambda blocks.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 415 to 438
Type: potential_issue

Comment:
Missing foreign key relationships in TSE entities.

Multiple electoral entities reference other entities without configured foreign keys:
- MunicipioRecord → UfSigla (no FK to UfRecord)
- PerfilEleitoradoRecord → AnoEleicao, MunicipioCodigoIbge (missing FKs)
- PrestacaoContasRecord → CandidatoCpf/AnoEleicao, UfSigla (missing FKs)
- ResultadoEleitoralRecord → CandidatoCpf/AnoEleicao, MunicipioCodigoIbge, PartidoNumero (missing FKs despite having useful indexes)
- SecaoEleitoralRecord, ZonaEleitoralRecord → MunicipioCodigoIbge, UfSigla (missing FKs)

Consider configuring these relationships to enforce referential integrity at the database level.



Also applies to: 466-507, 509-573, 575-649, 651-675, 701-722

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 415 - 438, The snapshot lacks configured foreign key relationships for several TSE entities; add explicit relationships using the EF fluent API (e.g. in the same modelBuilder.Entity blocks or in OnModelCreating) so the DB enforces referential integrity: for MunicipioRecord.UfSigla -> UfRecord (use HasOne().WithMany(...).HasForeignKey("UfSigla") and set OnDelete behavior), PerfilEleitoradoRecord.AnoEleicao and .MunicipioCodigoIbge -> link to the appropriate Eleicao and MunicipioRecord keys, PrestacaoContasRecord.CandidatoCpf/AnoEleicao and .UfSigla -> link to Candidato and Uf/Eleicao, ResultadoEleitoralRecord.CandidatoCpf/AnoEleicao, .MunicipioCodigoIbge, .PartidoNumero -> link to Candidato, MunicipioRecord, Partido respectively, and SecaoEleitoralRecord / ZonaEleitoralRecord .MunicipioCodigoIbge and .UfSigla -> link to MunicipioRecord and UfRecord; use HasOne().WithMany(...).HasForeignKey("PropertyName") (or HasPrincipalKey when FK targets a non-PK) and choose appropriate OnDelete behavior, then re-generate the migration so the snapshot and DB schema include the FK constraints.

============================================================================
File: src/Etl/Pnad/PnadIngestionPipeline.cs
Line: 52 to 58
Type: potential_issue

Comment:
Return count reflects input, not inserted records.

The method returns items.Count, but ReplaceRowsAsync deduplicates items using DistinctBy. If duplicates exist in the CSV, the returned count will overstate the actual number of inserted records. Additionally, line 54 appears redundant since ReplaceRowsAsync already calls SaveChangesAsync on line 93.




🛠️ Proposed fix

-    private async Task ReplaceAsync(
+    private async Task ReplaceAsync(
         DbSet table,
         IReadOnlyList items,
         Func map,
         CancellationToken ct)
         where TRecord : class
     {
         await using var transaction = await context.Database.BeginTransactionAsync(ct);
 
         await EnsureTrimestresAsync(items, ExtractTrimestre, ct);
-        await ReplaceRowsAsync(table, items, map, ct);
-        await context.SaveChangesAsync(ct);
+        var insertedCount = await ReplaceRowsAsync(table, items, map, ct);
 
         await transaction.CommitAsync(ct);
 
-        return items.Count;
+        return insertedCount;
     }


And update ReplaceRowsAsync to return the count:

-    private async Task ReplaceRowsAsync(
+    private async Task ReplaceRowsAsync(
         DbSet table,
         IReadOnlyList items,
         Func map,
         CancellationToken ct)
         where TRecord : class
     {
         var distinctItems = items.DistinctBy(GetPrimaryKey).ToList();
 
         await table.ExecuteDeleteAsync(ct);
         context.ChangeTracker.Clear();
         await table.AddRangeAsync(distinctItems.Select(map), ct);
         await context.SaveChangesAsync(ct);
+        return distinctItems.Count;
     }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Pnad/PnadIngestionPipeline.cs around lines 52 - 58, The method currently returns items.Count which reflects input rows not the number actually inserted because ReplaceRowsAsync deduplicates via DistinctBy and also calls SaveChangesAsync itself; change ReplaceRowsAsync to return the number of records it upserts/inserted (e.g., an int), remove the redundant context.SaveChangesAsync call from the caller (the code around EnsureTrimestresAsync/ReplaceRowsAsync/transaction.CommitAsync), and update the caller to await the new ReplaceRowsAsync(...) and return its returned count so the API reports the true inserted record count (refer to ReplaceRowsAsync, EnsureTrimestresAsync, ExtractTrimestre).

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.Designer.cs
Line: 41 to 101
Type: potential_issue

Comment:
Consider adding foreign key constraints.

This entity references CandidatoCpf and AnoEleicao but no foreign key relationships are defined. Adding explicit foreign key constraints would:
- Ensure referential integrity
- Prevent orphaned records
- Make relationships explicit in the schema
- Enable cascade behaviors

Apply this consideration to other entities that reference candidates, municipalities, and other master data.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.Designer.cs around lines 41 - 101, The migration defines the BemDeclaradoRecord entity and composite key (CandidatoCpf, AnoEleicao, TipoBem, Descricao) but omits foreign key constraints; update the EF model so BemDeclaradoRecord has a proper relationship to the candidate/master entity (e.g., add a HasOne/WithMany and HasForeignKey using the composite key properties CandidatoCpf and AnoEleicao on the BemDeclaradoRecord entity) and configure the desired onDelete behavior (Restrict/Cascade); apply the same pattern for other entities that reference candidates, municipalities, or master data to create explicit foreign keys and enforce referential integrity.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.Designer.cs
Line: 43 to 98
Type: potential_issue

Comment:
Reconsider using description field in primary key.

The composite primary key includes Descricao (a 500-character text field). Including variable-length text fields in primary keys creates several issues:
- Free-text descriptions may not guarantee true uniqueness
- Large keys negatively impact index performance and storage
- String comparisons are slower than integer comparisons

Consider adding a surrogate auto-increment ID or using a hash/sequence number instead of the description.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.Designer.cs around lines 43 - 98, The composite primary key currently includes the long text field Descricao (property Descricao with HasMaxLength(500)) via b.HasKey("CandidatoCpf", "AnoEleicao", "TipoBem", "Descricao"); change this by introducing a surrogate key (e.g., an auto-increment/int Id or a stable hash column) on the entity and migration, update the migration to add the new Id property with value generation and set b.HasKey("Id") instead of including Descricao, and update the entity configuration/class that defines CandidatoCpf/AnoEleicao/TipoBem/Descricao to remove the PK constraint from Descricao and use the new Id as the primary key; regenerate or adjust dependent indexes/foreign keys accordingly.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.Designer.cs
Line: 514 to 573
Type: potential_issue

Comment:
Reconsider using description field in primary key.

Similar to BemDeclaradoRecord, this entity uses Descricao (500-character text) as part of a 5-column composite primary key. This creates the same issues with uniqueness, performance, and maintainability.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.Designer.cs around lines 514 - 573, The composite primary key includes the large text property Descricao which harms uniqueness and performance; remove Descricao from the primary key defined by the b.HasKey(...) call and instead introduce a compact surrogate primary key (e.g., add an Id/Guid property and make it the primary key) or use a smaller natural key (CandidatoCpf, AnoEleicao, TipoReceita, TipoMovimentacao) plus a non-key column for Descricao; update the entity/model to add the new Id property (or adjust Key attributes), update the migration so b.Property("Id") is added and b.HasKey("Id") is used, and if you still need to enforce uniqueness including Descricao add a separate unique index for the combination that includes Descricao.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.Designer.cs
Line: 28 to 725
Type: potential_issue

Comment:
Add indexes on foreign key columns for query performance.

Several entities reference foreign key columns without corresponding indexes. For example:
- BemDeclaradoRecord references CandidatoCpf and AnoEleicao (no indexes)
- CandidatoRecord references AnoEleicao (no index)
- PrestacaoContasRecord references CandidatoCpf and AnoEleicao (no indexes)

While some entities have appropriate indexes (e.g., ResultadoEleitoralRecord at lines 645-649), the strategy is inconsistent. Add indexes on columns used in joins and WHERE clauses to improve query performance.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.Designer.cs around lines 28 - 725, Add missing indexes on foreign-key columns used in joins/WHERE: for entity mappings in the migration, add HasIndex(...) for BemDeclaradoRecord on "CandidatoCpf" and "AnoEleicao", for CandidatoRecord on "AnoEleicao", and for PrestacaoContasRecord on "CandidatoCpf" and "AnoEleicao" (similar to existing HasIndex calls used in ResultadoEleitoralRecord and MunicipioRecord). Update the modelBuilder.Entity(...) blocks for Data.BemDeclaradoRecord, Data.CandidatoRecord and Data.PrestacaoContasRecord to call b.HasIndex(...) with the appropriate column names so the generated migration creates database indexes for those FK columns.

============================================================================
File: src/Data/Migrations/PublicDataDbContextModelSnapshot.cs
Line: 152 to 154
Type: potential_issue

Comment:
Nullable field in composite primary key.

Nome is nullable but is part of the composite primary key (line 163). Primary key components must be non-nullable.



🔧 Recommended fix

 b.Property("Nome")
+    .IsRequired()
     .HasMaxLength(200)
     .HasColumnType("character varying(200)");

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/PublicDataDbContextModelSnapshot.cs around lines 152 - 154, The snapshot shows the property Nome is nullable but is included in the composite primary key; make Nome non-nullable by marking the property required in the model and snapshot. Update the entity/property declaration for Nome to be a non-nullable string (remove the nullable annotation) and ensure the EF model/snapshot uses .IsRequired() on the Nome property (modify b.Property("Nome") to include .IsRequired().HasMaxLength(200).HasColumnType("character varying(200)")), and keep the existing composite key (HasKey(...)) unchanged.

============================================================================
File: src/Etl/Etl.csproj
Line: 8 to 15
Type: refactor_suggestion

Comment:
Consider adding Serilog integration packages for Microsoft.Extensions.DependencyInjection.

The project includes Serilog and Microsoft.Extensions.DependencyInjection but lacks integration packages. To use Serilog with dependency injection and configuration, consider adding:
- Serilog.Extensions.Hosting (or Serilog.Extensions.Logging) for DI integration
- Serilog.Settings.Configuration for configuring Serilog from appsettings.json

Without these, integrating Serilog with the DI container and configuration system will require manual setup.



📦 Suggested packages to add

   
     
     
     
     
     
+    
+

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Etl.csproj around lines 8 - 15, The project file's ItemGroup contains Serilog and DI packages but is missing Serilog integration packages required for smooth DI and configuration; update the Etl.csproj ItemGroup to add package references for Serilog.Extensions.Hosting (or Serilog.Extensions.Logging) and Serilog.Settings.Configuration so Serilog can be wired into Microsoft.Extensions.DependencyInjection and configured from appsettings.json—ensure the new PackageReference entries are alongside existing references (e.g., near the Serilog and Serilog.Sinks.Console entries) so the DI and configuration integration will work without manual wiring.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.Designer.cs
Line: 445 to 449
Type: potential_issue

Comment:
Remove auto-increment from party number.

Numero represents official political party numbers in Brazil (e.g., PT=13, PSDB=45), which are registered with the electoral court. These should not be auto-generated.



🐛 Proposed fix

                    b.Property("Numero")
-                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

-                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property("Numero"));
-
                    b.HasKey("Numero");

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.Designer.cs around lines 445 - 449, The Numero property is incorrectly configured as auto-generated; remove the .ValueGeneratedOnAdd() call and the NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(...) invocation for the "Numero" property so it is treated as a regular stored integer (or explicitly mark it .ValueGeneratedNever() if you prefer explicitness) — update the migration code around b.Property("Numero") to stop applying identity generation.

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.Designer.cs
Line: 30 to 34
Type: potential_issue

Comment:
Remove auto-increment from election year field.

The Ano field represents an election year (a semantic value like 2020, 2022, 2024) and should not be auto-generated. Using ValueGeneratedOnAdd() and UseIdentityByDefaultColumn() will cause the database to generate sequential integers (1, 2, 3...) instead of actual years.



🐛 Proposed fix

                    b.Property("Ano")
-                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

-                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property("Ano"));
-
                    b.HasKey("Ano");

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.Designer.cs around lines 30 - 34, The Ano property is incorrectly configured as auto-generated; remove the calls that make it identity/auto-increment so stored election years remain the values you set. Locate the configuration that references b.Property("Ano") and remove ValueGeneratedOnAdd() and the NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(...) invocation so Ano remains a regular integer column (keep HasColumnType("integer") or equivalent); also check for any fluent configuration or annotations on the entity/property that set ValueGeneratedOnAdd or Identity and remove them.

============================================================================
File: src/Api/QueryValidation/SqlQueryValidator.cs
Line: 457 to 460
Type: potential_issue

Comment:
Misleading error message for large LIMIT values.

If LIMIT is set to a number larger than int.MaxValue (e.g., LIMIT 99999999999), int.TryParse fails and the error message says "LIMIT must be a numeric literal or ALL" — but it is numeric, just too large to parse.


Proposed fix

-               if (!int.TryParse(tokens[limitInfo.LimitValueTokenIndex.Value].Text, out var limitValue))
-               {
-                   throw new SqlQueryValidationException("LIMIT must be a numeric literal or ALL.");
-               }
+               var limitText = tokens[limitInfo.LimitValueTokenIndex.Value].Text;
+               if (!int.TryParse(limitText, out var limitValue))
+               {
+                   throw new SqlQueryValidationException($"LIMIT value '{limitText}' is not a valid integer.");
+               }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Api/QueryValidation/SqlQueryValidator.cs around lines 457 - 460, The current code in SqlQueryValidator (around the LIMIT handling using tokens[limitInfo.LimitValueTokenIndex]) uses int.TryParse which treats very large numeric literals as "non-numeric"; change to parse the token with long.TryParse (or System.Numerics.BigInteger if unlimited range is required) and then validate the numeric range expected by your engine, throwing SqlQueryValidationException with distinct messages for "not a numeric literal" versus "numeric literal out of supported range"; update the error text and checks in the code that references limitInfo.LimitValueTokenIndex and the LIMIT parsing logic so very large numeric strings produce an "out of range" error instead of "must be a numeric literal or ALL."

============================================================================
File: src/Data/Migrations/20260412141709_InitialCreate.Designer.cs
Line: 420 to 424
Type: potential_issue

Comment:
Remove auto-increment from IBGE municipal code.

CodigoIbge represents official IBGE (Brazilian Institute of Geography and Statistics) municipal codes, which are standardized government-issued identifiers. These should not be auto-generated.



🐛 Proposed fix

                    b.Property("CodigoIbge")
-                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

-                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property("CodigoIbge"));
-
                    b.HasKey("CodigoIbge");

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/Migrations/20260412141709_InitialCreate.Designer.cs around lines 420 - 424, The CodigoIbge property is currently configured as auto-increment via ValueGeneratedOnAdd and UseIdentityByDefaultColumn; remove the auto-generation so IBGE municipal codes remain explicit values. Locate the property declaration b.Property("CodigoIbge") and delete the .ValueGeneratedOnAdd() call and the NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(...) invocation so the column remains HasColumnType("integer") but is not identity/auto-generated.

Review completed: 79 findings ✔
