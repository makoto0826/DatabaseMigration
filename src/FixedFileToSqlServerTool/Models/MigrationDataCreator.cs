using System.Data;
using System.IO;
using System.Text;

namespace FixedFileToSqlServerTool.Models;

public class MigrationDataCreator
{
    private readonly ScriptRunner _scriptRunner;

    public MigrationDataCreator(ScriptRunner scriptRunner)
    {
        _scriptRunner = scriptRunner;
    }

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

                    var result = await _scriptRunner.RunAsync(new ScriptRunnerContext(column.GenerationScript.Code));

                    if (!result.IsSucceeded)
                    {
                        throw new ModelException(result.ErrorMessage);
                    }

                    row[column.Destination.Name] = result.ReturnValue;
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
                        var result = await _scriptRunner.RunAsync(new ScriptRunnerContext(column.ConvertScript.Code, value));

                        if (!result.IsSucceeded)
                        {
                            throw new ModelException(result.ErrorMessage);
                        }

                        row[column.Destination.Name] = result.ReturnValue;
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
