using ClosedXML.Excel;
using DatabaseMigration.Data;
using Microsoft.Extensions.Logging;

namespace DatabaseMigration.Generator;

public class ExcelImporter
{
    private readonly ILogger _logger;

    public ExcelImporter(ILogger<ExcelImporter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public List<MigrationProject> ImportFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new ArgumentException("Excelファイルがありません");
        }

        var workbook = new XLWorkbook(filePath);
        var settingSheet = workbook.Worksheets.First(x => x.Name == "設定");

        if (settingSheet is null)
        {
            throw new InvalidOperationException("設定シートが存在しません");
        }

        var databaseType = settingSheet.Cell("B1").GetString() switch
        {
            var x when x.ToUpper() == "SQLSERVER" => DatabaseType.SqlServer,
            _ => DatabaseType.SqlServer
        };

        var connectionString = settingSheet.Cell("B2").GetString();

        return workbook.Worksheets
            .Where(x => x.Name != "設定")
            .Select(x =>
            {
                //A:1 Generation
                //B:2 StartPosition
                //C:3 EndPosition
                //D:4 ColumnName
                //E:5 Type
                //F:6 NULL
                //G:7 Size
                //H:8 Convert Method
                //I:9 Generation Method

                // search row from 5 to 1024
                var columns =
                    x.Rows(5, 1024)
                        .Where(x => !String.IsNullOrWhiteSpace(x.Cell(4).GetString()))
                        .Select(x =>
                        {
                            var isGeneration = !String.IsNullOrWhiteSpace(x.Cell(1).GetString());

                            return new ColumnMapping
                            {
                                IsGeneration = isGeneration,
                                ConvertMethod = x.Cell(8).GetString(),
                                GenerationMethod = x.Cell(9).GetString(),
                                Source = isGeneration ? null : new SourceColumn
                                {
                                    StartPosition = (int)x.Cell(2).GetDouble(),
                                    EndPosition = (int)x.Cell(3).GetDouble()
                                },
                                Destination = new DestinationColumn
                                {
                                    Name = x.Cell(4).GetString()
                                }
                            };
                        })
                        .ToList();

                var project = new MigrationProject
                {
                    Name = x.Cell("B1").GetString(),
                    DatabaseType = databaseType,
                    ConnectionString = connectionString,
                    Table = new TableMapping
                    {
                        Name = x.Cell("B2").GetString(),
                        Columns = columns
                    }
                };

                _logger.LogInformation($"プロジェクト名:{project.Name} テーブル名:{project.Table.Name}");

                foreach (var column in project.Table.Columns)
                {
                    _logger.LogInformation(
$"自動生成:{column.IsGeneration} 開始位置:{column.Source?.StartPosition} 終了位置:{column.Source?.EndPosition} 列名:{column.Destination.Name}");
                }

                return project;
            })
            .ToList();
    }
}
