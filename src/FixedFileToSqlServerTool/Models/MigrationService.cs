using System.Data;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public class MigrationService
{
    private readonly SqlConnection _connection;

    public MigrationService(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task MigrateAsync(DataTable dataTable)
    {
        SqlTransaction? transaction = null;

        try
        {
            await _connection.OpenAsync();
            transaction = await _connection.BeginTransactionAsync() as SqlTransaction;

            using var bulkCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = dataTable.TableName;

            await bulkCopy.WriteToServerAsync(dataTable);
            await transaction?.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction?.RollbackAsync();
            throw new ModelException("", ex);
        }
    }
}
