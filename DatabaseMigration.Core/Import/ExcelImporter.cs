using DatabaseMigration.Core.Data;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;

namespace DatabaseMigration.Core.Import;

public class ExcelImporter
{
    private readonly ILogger _logger;

    public ExcelImporter(ILogger<ExcelImporter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<MigrationProject>> ImportAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new ArgumentException("Excelファイルがありません");
        }

        return new List<MigrationProject>();
    }
}