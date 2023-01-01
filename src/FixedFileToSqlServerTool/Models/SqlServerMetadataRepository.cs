using Dapper;
using FixedFileToSqlServerTool.Models;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public class SqlServerMetadataRepository
{
    private readonly SqlConnection _connection;

    public SqlServerMetadataRepository(SqlConnection connection) =>
        _connection = connection;

    public async Task<List<TableDefinition>> GetTableDefinitionsAsync()
    {
        const string SQL = @"
WITH _table AS (SELECT * FROM sys.tables WHERE type = 'U') 
SELECT
    _table.object_id
    , _table.name AS table_name
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
    , _column.column_id";

        var rows = await _connection.QueryAsync<Row>(SQL);

        return rows.GroupBy(row => row.object_id)
            .Select(group =>
            {
                var first = group.First();
                return new TableDefinition
                {
                    Id = first.object_id,
                    Name = first.table_name,
                    Columns = group.Select(row =>
                        new ColumnDefinition
                        {
                            Name = row.column_name,
                            Type = row.column_type,
                            MaxLength = row.max_length,
                            IsNullable = row.is_nullable
                        })
                        .ToList()
                };
            })
            .ToList();
    }

    private class Row
    {
        public int object_id;

        public string table_name = String.Empty;

        public string column_name = String.Empty;

        public string column_type = String.Empty;

        public int max_length;

        public bool is_nullable;
    }
}
