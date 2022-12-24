using Razor.Templating.Core;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using DatabaseMigration.Data;

namespace DatabaseMigration;

public class Program
{
    public static async Task Main()
    {
        try
        {
            await RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadKey();
        }
    }

    public static async Task RunAsync()
    {
        var services = new ServiceCollection();

        services.AddLogging(options => options.AddConsole());
        services.AddMvcCore().AddRazorRuntimeCompilation();
        services.Configure<MvcRazorRuntimeCompilationOptions>(options => options.FileProviders.Add(new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "Templates"))));
        services.AddRazorTemplating();

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation($"実行フォルダ:{AppContext.BaseDirectory}");

        var context = new MigrationContext()
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
        };


        var appSettingContent = await RazorTemplateEngine.RenderAsync($"/Templates/AppSettings.cshtml");
        var projectContent = await RazorTemplateEngine.RenderAsync($"/Templates/Project.{context.DatabaseType}.cshtml");
        var programContent = await RazorTemplateEngine.RenderAsync($"/Templates/Program.{context.DatabaseType}.cshtml", context);
        var fileMapperContent = await RazorTemplateEngine.RenderAsync($"/Templates/FileMapper.{context.DatabaseType}.cshtml", context);
        var dataTableCreatorContent = await RazorTemplateEngine.RenderAsync($"/Templates/DataTableCreator.{context.DatabaseType}.cshtml", context);
        var stringExtensionsContent = await RazorTemplateEngine.RenderAsync("/Templates/StringExtensions.cshtml", context);

        var fileGenerator = FileGenerator.Create(context, provider.GetRequiredService<ILogger<FileGenerator>>());
        await fileGenerator.GenerateAsync(new List<(string, string)>
        {
            new ($"appsettings.json",appSettingContent),
            new ($"{context.ProjectName}.csproj",projectContent),
            new ("Program.cs",programContent),
            new ("FileMapper.cs",fileMapperContent),
            new ("DataTableCreator.cs",dataTableCreatorContent),
            new ("StringExtensions.cs",stringExtensionsContent)
        });
    }
}