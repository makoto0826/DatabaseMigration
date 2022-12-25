namespace DatabaseMigration.Core.Data;

public class TableMapping
{
    public string Name { get; init; }

    public List<ColumnMapping> Columns { get; set; } = new();
}

