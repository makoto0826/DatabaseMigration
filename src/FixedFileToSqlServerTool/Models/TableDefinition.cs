namespace FixedFileToSqlServerTool.Models;

public record class TableDefinition
{
    public int Id { get; set; }

    public required string Name { get; init; }

    public required List<ColumnDefinition> Columns { get; init; }
}
