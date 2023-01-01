namespace FixedFileToSqlServerTool.Models;

public class MappingColumn
{
    public bool IsGeneration { get; set; }

    public FixedColumn? Source { get; set; }

    public ColumnDefinition Destination { get; set; }

    public string? GenerationScriptCode { get; set; }

    public string? ConvertScriptCode { get; set; }
}
