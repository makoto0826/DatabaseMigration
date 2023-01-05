namespace FixedFileToSqlServerTool.Models;

public record class Column
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public required string Type { get; init; }

    public int MaxLength { get; init; }

    public bool IsNullable { get; init; }
}
