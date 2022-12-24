namespace DatabaseMigration.Data;

public class ColumnMapping
{
    public int StartPosition { get; set; }

    public int EndPosition { get; set; }

    public bool IsGeneration { get; set; }

    public string Name { get; init; }

    public string? ConvertMethodName { get; init; }

    public string? GenerationMethodName { get; init; }
}
