using System.Data;
using System.IO;
using System.Text;
using FixedFileToSqlServerTool.Models;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Hanlders;

public class MigrationHandler
{
    private readonly SqlConnection _connection;

    private readonly ScriptRunner _scriptRunner;

    public MigrationHandler(SqlConnection connection, ScriptRunner scriptRunner)
    {
        _connection = connection;
        _scriptRunner = scriptRunner;
    }

    public async Task HandleAsync(MigrationContext context)
    {
        var dataTable = CreateTable(context);
        await SetDataAsync(dataTable, context);

        SqlTransaction? transaction = null;

        try
        {
            await _connection.OpenAsync();
            transaction = (await _connection.BeginTransactionAsync()) as SqlTransaction;

            using var bulkCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = context.Table.Name;

            await bulkCopy.WriteToServerAsync(dataTable);
            await transaction?.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction?.RollbackAsync();
            throw new HandlerException("", ex);
        }
    }

    private async Task SetDataAsync(DataTable dataTable, MigrationContext context)
    {
        string? line = null;
        using var reader = new StreamReader(context.FilePath, Encoding.GetEncoding(context.Table.Encoding));

        while ((line = reader.ReadLine()) != null)
        {
            foreach (var column in context.Table.Columns)
            {
                var row = dataTable.NewRow();

                if (column.IsGeneration)
                {
                    if (column.GenerationScript is null)
                    {
                        throw new HandlerException("生成スクリプトが設定されていません");
                    }

                    var result = await _scriptRunner.RunAsync(new ScriptRunnerContext(column.GenerationScript.Code));

                    if (!result.IsSucceeded)
                    {
                        throw new HandlerException(result.ErrorMessage);
                    }

                    row[column.Destination.Name] = result.ReturnValue;
                }
                else
                {
                    if (column.Source is null)
                    {
                        throw new HandlerException($"列名:{column.Destination.Name} 開始位置または終了位置が設定されていません");
                    }

                    var value = line[column.Source.StartPosition..column.Source.EndPosition];

                    if (column.GenerationScript is not null)
                    {
                        var result = await _scriptRunner.RunAsync(new ScriptRunnerContext(column.ConvertScript.Code, value));

                        if (!result.IsSucceeded)
                        {
                            throw new HandlerException(result.ErrorMessage);
                        }

                        row[column.Destination.Name] = result.ReturnValue;
                    }
                    else
                    {
                        row[column.Destination.Name] = value;
                    }

                }

                dataTable.Rows.Add(row);
            }
        }
    }

    private DataTable CreateTable(MigrationContext context)
    {
        var dataTable = new DataTable();

        foreach (var column in context.Table.Columns)
        {
            dataTable.Columns.Add(new DataColumn(column.Destination.Name));
        }

        return dataTable;
    }
}
