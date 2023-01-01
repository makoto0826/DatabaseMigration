using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Infrastructures;

public class SqlServerMetadataRepository
{
    public SqlConnection Connection { get; }

    public SqlServerMetadataRepository(SqlConnection connection) =>
        this.Connection = connection;
}
