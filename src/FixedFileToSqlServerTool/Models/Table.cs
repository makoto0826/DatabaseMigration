using System.Data;

namespace FixedFileToSqlServerTool.Models;

public class Table
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<TableColumn> Columns { get; set; } = new();
}
