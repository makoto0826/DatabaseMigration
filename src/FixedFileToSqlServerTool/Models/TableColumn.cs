using System.Data;

namespace FixedFileToSqlServerTool.Models;

public class TableColumn
{
    public string Name { get; set; }

    public DbType Type { get; set; }
}
