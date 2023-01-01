namespace FixedFileToSqlServerTool.Models;

public record class FixedColumn
{
    public int StartPosition { get; init; }

    public int EndPosition { get; init; }
}
