namespace FixedFileToSqlServerGenerator.Data;

public class ColumnMapping
{
    public bool IsGeneration { get; init; }

    public string? GenerationMethod { get; init; }

    public string? ConvertMethod { get; init; }

    public SourceColumn? Source { get; init; }

    public required DestinationColumn Destination { get; init; }
}
