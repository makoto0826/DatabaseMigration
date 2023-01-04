using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public record class MigrationContext(MappingTable Table, string FilePath);
