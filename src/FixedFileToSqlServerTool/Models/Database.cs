using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public class Database
{
    private readonly DatabaseSetting _databaseSetting;

    public Database(DatabaseSetting databaseSetting) =>
        _databaseSetting = databaseSetting ?? throw new ArgumentNullException(nameof(databaseSetting));

    public async Task ConnectTestAsync()
    {
        using var connection = CreateConnection();

        try
        {
            await connection.OpenAsync();
        }
        catch (Exception ex)
        {
            throw new ModelException(ex.Message, ex);
        }
    }

    public async Task<List<Table>> GetTableDefinitionsAsync()
    {
        const string SQL = @"
WITH _table AS (SELECT * FROM sys.tables WHERE type = 'U') 
SELECT
    _table.object_id
    , _table.name AS table_name
    , _column.column_id AS column_id
    , _column.name AS column_name
    , _types.name AS column_type
    , _column.max_length
    , _column.is_nullable 
FROM
    _table
    INNER JOIN sys.columns _column 
        ON _table.object_id = _column.object_id 
    LEFT JOIN sys.types _types 
        ON _column.system_type_id = _types.system_type_id 
ORDER BY
    _table.object_id
    , _column.column_id
    , _types.name";

        using var connection = CreateConnection();

        try
        {
            var rows = await connection.QueryAsync<Row>(SQL);

            return rows.GroupBy(row => row.object_id)
                .Select(group =>
                {
                    var first = group.First();
                    var columns = group.Select(row =>
                        new Column
                        {
                            Id = row.column_id,
                            Name = row.column_name,
                            Type = row.column_type,
                            MaxLength = row.column_type switch
                            {
                                "nchar" or "nvarchar" => row.max_length / 2,
                                _ => row.max_length
                            },
                            IsNullable = row.is_nullable
                        })
                        .GroupBy(x => x.Id)
                        .Select(x => x.First())
                        .ToList();

                    return new Table
                    {
                        Id = first.object_id,
                        Name = first.table_name,
                        Columns = columns
                    };
                })
                .OrderBy(x => x.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new ModelException(ex.Message, ex);
        }
    }

    public async Task WriteAsync(DataTable dataTable)
    {
        var connection = CreateConnection();
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

    public SqlConnection CreateConnection()
    {
        var builder = new SqlConnectionStringBuilder();
        builder.Add("Server", $"{_databaseSetting.Server},{_databaseSetting.Port ?? 1433}");
        builder.Add("User ID", _databaseSetting.UserId ?? "");
        builder.Add("Password", _databaseSetting.Password ?? "");
        builder.Add("Database", _databaseSetting.Database ?? "");
        builder.Encrypt = false;

        return new SqlConnection(builder.ToString());
    }

    private class Row
    {
        public int object_id = 0;

        public string table_name = String.Empty;

        public int column_id = 0;

        public string column_name = String.Empty;

        public string column_type = String.Empty;

        public int max_length = 0;

        public bool is_nullable = false;
    }
}
