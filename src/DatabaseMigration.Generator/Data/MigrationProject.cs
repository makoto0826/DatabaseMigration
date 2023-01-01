namespace DatabaseMigration.Data;

public class MigrationProject
{
    public required string Name { get; init; }

    public required string ConnectionString { get; init; }

    public DatabaseType DatabaseType { get; init; }

    public required TableMapping Table { get; init; }
}
