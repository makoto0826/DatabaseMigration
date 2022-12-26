using System.Data;

namespace DatabaseMigration.Core.Data;

public class DestinationColumn
{
    public required string Name { get; init; }

    public DbType Type { get; init; }

    public bool IsNull { get; init; }

    public int Size { get; init; }
}
