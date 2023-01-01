using System.Data;

namespace FixedFileToSqlServerTool.Models;

public class ColumnDefinition
{
    public string Name { get; set; }

    public string Type { get; set; }

    public int MaxLength { get; set; }

    public bool IsNullable { get; set; }
}
