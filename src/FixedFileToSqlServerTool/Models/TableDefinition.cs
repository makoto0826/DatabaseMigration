namespace FixedFileToSqlServerTool.Models;

public class TableDefinition
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<ColumnDefinition> Columns { get; set; } = new();
}
