using DatabaseMigration.Core.Data;

namespace DatabaseMigration.Core;

public class MigrationContext
{
    public string ProjectName { get; init; }

    public string ConnectionString {get; init;}

    public DatabaseType DatabaseType { get; set; }

    public TableMapping Table { get; init; }
}