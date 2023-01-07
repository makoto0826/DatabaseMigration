using System.IO;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public class MigrationHandler
{
    private readonly DataTableCreator _dataTableCreator;

    public MigrationHandler(DataTableCreator dataTableCreator) =>
        _dataTableCreator = dataTableCreator ?? throw new ArgumentNullException(nameof(dataTableCreator));

    public async Task HandleAsync(string filePath, MappingTable mappingTable, DatabaseSetting databaseSetting)
    {
        var connection = databaseSetting.CreateConnection();
        SqlTransaction? transaction = null;

        try
        {
            using var fs = new FileStream(filePath, FileMode.Open);
            var dataTable = await _dataTableCreator.CreateAsync(mappingTable, fs);

            await connection.OpenAsync();
            transaction = await connection.BeginTransactionAsync() as SqlTransaction;

            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = dataTable.TableName;

            await bulkCopy.WriteToServerAsync(dataTable);
            await transaction!.CommitAsync();
        }
        catch (Exception ex)
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync();
            }

            throw new ModelException("", ex);
        }
    }
}
