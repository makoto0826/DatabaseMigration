using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public class MigrationHandler
{
    private readonly SqlConnection _connection;

    public MigrationHandler(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> HandleAsync()
    {
        await _connection.OpenAsync();
        var transaction = await _connection.BeginTransactionAsync();

        try
        {
            using var bulkCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.Default, transaction as SqlTransaction);
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }
}
