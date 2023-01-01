using DatabaseMigration.Data;
using DatabaseMigration.Generator.Template;
using Microsoft.Extensions.Logging;

namespace DatabaseMigration.Generator;

public class ProjectGenerator
{
    private readonly string _baseDirectoryPath;

    private readonly ILogger _logger;

    public ProjectGenerator(string destinationPath, ILogger<ProjectGenerator> logger)
    {
        _baseDirectoryPath = destinationPath;

        if (!Directory.Exists(_baseDirectoryPath))
        {
            Directory.CreateDirectory(_baseDirectoryPath);
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task GenerateAsync(MigrationProject project, TemplateResult result)
    {
        var destinationPath = Path.Combine(_baseDirectoryPath, project.Name);

        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        _logger.LogInformation($"プロジェクト名:{project.Name}");

        foreach (var (name, content) in result.Items)
        {
            var path = Path.Combine(destinationPath, name);
            _logger.LogInformation($"ファイル作成:{path}");

            using var writer = new StreamWriter(path, false);
            await writer.WriteLineAsync(content);
            await writer.FlushAsync();
        }
    }
}
