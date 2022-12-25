namespace DatabaseMigration.Core;

public class TemplateContext
{
    public DateTime Now { get; } = DateTime.Now;

    public MigrationContext Migration { get; init; }
}