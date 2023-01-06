using Dapper;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public class MetadataRepository
{
    private readonly SqlConnection _connection;

    public MetadataRepository(SqlConnection connection) =>
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));

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

        var rows = await _connection.QueryAsync<Row>(SQL);

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
                        MaxLength = row.max_length,
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
