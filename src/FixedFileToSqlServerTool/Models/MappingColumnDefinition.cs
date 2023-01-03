using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public record class MappingColumnDefinition
{
    public bool IsGeneration { get; init; }

    public FixedColumn? Source { get; init; }

    public required ColumnDefinition Destination { get; init; }

    public Script? GenerationScript { get; init; }

    public Script? ConvertScript { get; init; }
}
