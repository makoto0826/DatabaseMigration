using FixedFileToSqlServerTool.Models;
using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Hanlders;

public record class MigrationContext(MappingTableDefinition Table, string FilePath);
