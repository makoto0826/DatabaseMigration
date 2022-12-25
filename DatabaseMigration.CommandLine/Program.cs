using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DatabaseMigration.Core.Data;
using System.Data;
using DatabaseMigration.Core.Template;
using DatabaseMigration.Core.Generator;
using DatabaseMigration.Core.Import;

namespace DatabaseMigration.CommandLine;

public class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLogging(options => options.AddConsole());

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Program>>();
        var baseDirectoryPath = AppContext.BaseDirectory;

        logger.LogInformation($"実行フォルダ:{baseDirectoryPath}");

        try
        {
            await RunAsync(args[0], baseDirectoryPath, provider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error");
        }
    }

    public static async Task RunAsync(string excelFilePath, string baseDirectoryPath, ServiceProvider provider)
    {
        var importer = new ExcelImporter(provider.GetRequiredService<ILogger<ExcelImporter>>());
        var generator = new ProjectGenerator(baseDirectoryPath, provider.GetRequiredService<ILogger<ProjectGenerator>>());
        var engine = new RazorEngineAdapter(Path.Combine(baseDirectoryPath, "Templates"), provider.GetRequiredService<ILogger<RazorEngineAdapter>>());

        var projects = await importer.ImportAsync(excelFilePath);

        foreach (var project in projects)
        {
            var result = await engine.RunAsync(new TemplateContext { Project = project });
            await generator.GenerateAsync(project, result);
        }
    }
}
