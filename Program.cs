using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorEngineCore;
using DatabaseMigration.Data;

namespace DatabaseMigration;

public class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddLogging(options => options.AddConsole());

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"実行フォルダ:{AppContext.BaseDirectory}");

        try
        {
            await RunAsync(provider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }

    public static async Task RunAsync(ServiceProvider provider)
    {
        var context = new TemplateContext
        {
            Migration = new MigrationContext()
            {
                ProjectName = "Sample",
                SourceName = "User1",
                Table = new TableMapping
                {
                    Name = "User",
                    Columns = new List<ColumnMapping>
                {
                    new ColumnMapping
                    {
                        Name = "Id",
                        StartPosition = 0,
                        EndPosition = 5,
                    },
                    new ColumnMapping
                    {
                        Name = "UserName",
                        StartPosition = 6,
                        EndPosition = 20,
                        ConvertMethodName = "ToUpper"
                    },
                    new ColumnMapping
                    {
                        IsGeneration = true,
                        Name = "CreatedAt",
                        GenerationMethodName = "DateTime.Now"
                    }
                }
                }
            }
        };

        var razorEngine = new RazorEngine();
        var basePath = AppContext.BaseDirectory;
        var builderAction = (IRazorEngineCompilationOptionsBuilder builder) => { };

        var appSettingContent = await razorEngine.RunFromFileAsync($"{basePath}/Templates/AppSettings.cshtml");
        var projectContent = await razorEngine.RunFromFileAsync($"{basePath}/Templates/Project.{context.Migration.DatabaseType}.cshtml");
        var programContent = await razorEngine.RunFromFileAsync($"{basePath}/Templates/Program.{context.Migration.DatabaseType}.cshtml", builderAction, context);
        var fileMapperContent = await razorEngine.RunFromFileAsync($"{basePath}/Templates/FileMapper.{context.Migration.DatabaseType}.cshtml", builderAction, context);
        var dataTableCreatorContent = await razorEngine.RunFromFileAsync($"{basePath}/Templates/DataTableCreator.{context.Migration.DatabaseType}.cshtml", builderAction, context);
        var extensionsContent = await razorEngine.RunFromFileAsync($"{basePath}/Templates/Extensions.cshtml", builderAction, context);

        var fileGenerator = FileGenerator.Create(context.Migration, provider.GetRequiredService<ILogger<FileGenerator>>());
        await fileGenerator.GenerateAsync(new List<(string, string)>
        {
            new ($"appsettings.json",appSettingContent),
            new ($"{context.Migration.ProjectName}.csproj",projectContent),
            new ("Program.cs",programContent),
            new ("FileMapper.cs",fileMapperContent),
            new ("DataTableCreator.cs",dataTableCreatorContent),
            new ("Extensions.cs",extensionsContent)
        });
    }
}
