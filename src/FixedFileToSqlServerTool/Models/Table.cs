namespace FixedFileToSqlServerTool.Models;

public record class Table
{
    public int Id { get; set; }

    public required string Name { get; init; }

    public required List<Column> Columns { get; init; }
}
