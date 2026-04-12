using Data.Infrastructure;
using Etl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("ETL iniciado");
    var command = EtlCommandParser.Parse(args);

    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

    var services = new ServiceCollection();
    services.AddPublicDataReadWriteDbContext(configuration);
    services.AddScoped<EtlRunner>();

    await using var serviceProvider = services.BuildServiceProvider();
    await using var scope = serviceProvider.CreateAsyncScope();

    var runner = scope.ServiceProvider.GetRequiredService<EtlRunner>();
    var importedCount = await runner.RunAsync(command);

    Log.Information("ETL importou {ImportedCount} registros para {Source}", importedCount, command.Source);
    Log.Information("ETL finalizado");
}
catch (Exception ex)
{
    Log.Fatal(ex, "ETL falhou");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

return 0;
