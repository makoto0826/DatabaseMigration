using System.Data;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public class Database
{
    public async Task CopyAsync(DataTable dataTable, DatabaseSetting databaseSetting)
    {
        var connection = databaseSetting.CreateConnection();
        SqlTransaction? transaction = null;

        try
        {
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

            throw new ModelException(ex.Message, ex);
        }
    }
}
