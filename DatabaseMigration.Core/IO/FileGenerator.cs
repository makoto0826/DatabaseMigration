using Microsoft.Extensions.Logging;

namespace DatabaseMigration.Core.IO;

public class FileGenerator
{
    private readonly string _destinationPath;

    private readonly ILogger _logger;

    public FileGenerator(string destinationPath, ILogger<FileGenerator> logger)
    {
        _destinationPath = destinationPath;

        if (!Directory.Exists(_destinationPath))
        {
            Directory.CreateDirectory(_destinationPath);
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public static FileGenerator Create(MigrationContext context, ILogger<FileGenerator> logger, string? basePath = null)
    {
        if (basePath is null)
        {
            return new FileGenerator(Path.Combine(AppContext.BaseDirectory, context.ProjectName), logger);
        }

        return new FileGenerator(basePath, logger);
    }

    public async Task GenerateAsync(List<(string, string)> contents)
    {
        foreach (var (name, content) in contents)
        {
            var path = Path.Combine(_destinationPath, name);
            _logger.LogInformation($"ファイル作成:{path}");

            using var writer = new StreamWriter(path, false);
            await writer.WriteLineAsync(content);
            await writer.FlushAsync();
        }
    }
}