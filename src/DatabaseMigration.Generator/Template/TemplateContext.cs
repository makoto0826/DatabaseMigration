using DatabaseMigration.Data;

namespace DatabaseMigration.Generator.Template;

public class TemplateContext
{
    public DateTime Now { get; } = DateTime.Now;

    public required MigrationProject Project { get; init; }
}
