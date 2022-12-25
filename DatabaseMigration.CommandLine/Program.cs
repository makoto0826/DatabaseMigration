using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorEngineCore;
using DatabaseMigration.Core.Data;
using System.Data;
using DatabaseMigration.Core.RazorEngineCore;
using DatabaseMigration.Core.IO;
using DatabaseMigration.Core;

namespace DatabaseMigration.CommandLine;

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
                ConnectionString = "Server=localhost;User ID=sa;Password=P@ssw0rd;encrypt=false",
                Table = new TableMapping
                {
                    Name = "Users",
                    Columns = new List<ColumnMapping>
                {
                    new ColumnMapping
                    {
                        Source = new SourceColumn
                        {
                            StartPosition = 0,
                            EndPosition = 5,
                        },
                        Destination = new DestinationColumn
                        {
                            Name = "UserId",
                            Type = DbType.String
                        },
                    },
                    new ColumnMapping
                    {
                        Source = new SourceColumn
                        {
                            StartPosition = 5,
                            EndPosition = 15,
                        },
                        Destination = new DestinationColumn
                        {
                            Name = "UserName",
                            Type = DbType.String
                        },
                        ConvertMethod = "Trim().ToUpper()"
                    },
                    new ColumnMapping
                    {
                        IsGeneration = true,
                        Destination = new DestinationColumn
                        {
                            Name = "CreatedAt",
                            Type = DbType.DateTime,
                        },
                        GenerationMethod = "DateTime.Now"
                    }
                }
                }
            }
        };

        var razorEngine = new RazorEngine();
        var nameWithTaskList =
            new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Templates"))
                .GetFiles()
                .Select(fileInfo =>
                {
                    var sp = fileInfo.Name.Split(new char[] { '.' });
                    var name = $"{sp[0]}.{sp[1]}";

                    if (sp[1] == "csproj")
                    {
                        name = $"{context.Migration.ProjectName}.{sp[1]}";
                    }

                    var task = razorEngine.RunFromFileAsync(fileInfo.FullName, model: context);
                    return (name, task);
                });

        await Task.WhenAll(nameWithTaskList.Select(x => x.task));

        var contents = nameWithTaskList.Select(x => (x.name, x.task.Result)).ToList();
        var fileGenerator = FileGenerator.Create(context.Migration, provider.GetRequiredService<ILogger<FileGenerator>>());
        await fileGenerator.GenerateAsync(contents);
    }
}
