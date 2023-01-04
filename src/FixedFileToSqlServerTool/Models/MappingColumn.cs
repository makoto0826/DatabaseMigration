namespace FixedFileToSqlServerTool.Models;

public record class MappingColumn
{
    public bool IsGeneration { get; init; }

    public FixedColumn? Source { get; init; }

    public required Column Destination { get; init; }

    public Script? GenerationScript { get; init; }

    public Script? ConvertScript { get; init; }

    public static MappingColumn Create(Column column) =>
        new MappingColumn
        {
            Destination = column with { }
        };
}
