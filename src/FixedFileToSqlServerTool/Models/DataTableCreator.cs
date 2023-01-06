using System.Data;
using System.IO;
using System.Text;

namespace FixedFileToSqlServerTool.Models;

public class DataTableCreator
{
    private readonly ScriptRunner _scriptRunner;

    public DataTableCreator(ScriptRunner scriptRunner) =>
        _scriptRunner = scriptRunner ?? throw new ArgumentNullException(nameof(scriptRunner));

    public DataTable CreateEmpty(MappingTable table)
    {
        var dataTable = new DataTable()
        {
            TableName = table.TableName
        };

        foreach (var column in table.Columns)
        {
            dataTable.Columns.Add(new DataColumn(column.Destination.Name));
        }

        return dataTable;
    }

    public async Task<DataTable> CreateAsync(MappingTable table, Stream sourceStream)
    {
        var dataTable = CreateEmpty(table);

        string? line = null;
        using var reader = new StreamReader(sourceStream, Encoding.GetEncoding(table.Encoding));

        while ((line = reader.ReadLine()) != null)
        {
            var row = dataTable.NewRow();

            foreach (var column in table.Columns)
            {
                if (column.IsGeneration)
                {
                    if (column.GenerationScript is null)
                    {
                        throw new ModelException("生成スクリプトが設定されていません");
                    }

                    row[column.Destination.Name] = await _scriptRunner.RunAsync(column.GenerationScript.Code);
                }
                else
                {
                    if (column.Source is null)
                    {
                        throw new ModelException($"列名:{column.Destination.Name} 開始位置または終了位置が設定されていません");
                    }

                    var value = line[column.Source.StartPosition..column.Source.EndPosition];

                    if (column.ConvertScript is not null)
                    {
                        row[column.Destination.Name] = await _scriptRunner.RunAsync(column.ConvertScript.Code, value);
                    }
                    else
                    {
                        row[column.Destination.Name] = value;
                    }
                }
            }

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }
}
