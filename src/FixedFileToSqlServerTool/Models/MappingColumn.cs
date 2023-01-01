namespace FixedFileToSqlServerTool.Models;

public class MappingColumn
{
    public bool IsGeneration { get; set; }

    public FixedColumn? FixedColumn { get; set; }

    public TableColumn TableColumn { get; set; }

    public string? GenerationMethodId { get; set; }

    public string? ConvertMethodId { get; set; }
}
