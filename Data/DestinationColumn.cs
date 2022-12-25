using System.Data;

namespace DatabaseMigration.Data;

public class DestinationColumn
{
    public string Name { get; set; }

    public DbType Type { get; set; }

    public bool IsNull { get; set; }

    public int Size { get; set; }
}
