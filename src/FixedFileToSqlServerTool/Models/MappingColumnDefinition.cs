using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class MappingColumnDefinition
{
    public bool IsGeneration { get; set; }

    public FixedColumn? Source { get; set; }

    public ColumnDefinition Destination { get; set; }

    public Script? GenerationScript { get; set; }

    public Script? ConvertScript { get; set; }
}
