namespace FixedFileToSqlServerGenerator.Data;

public class TableMapping
{
    public required string Name { get; init; }

    public required List<ColumnMapping> Columns { get; init; }
}
