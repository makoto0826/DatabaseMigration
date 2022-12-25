namespace DatabaseMigration.Data;

public class ColumnMapping
{
    public bool IsGeneration { get; set; }

    public string? GenerationMethod { get; init; }

    public string? ConvertMethod { get; init; }

    public SourceColumn Source { get; set; }

    public DestinationColumn Destination { get; set; }
}
