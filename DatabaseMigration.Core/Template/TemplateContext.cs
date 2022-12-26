using DatabaseMigration.Core.Data;

namespace DatabaseMigration.Core.Template;

public class TemplateContext
{
    public DateTime Now { get; } = DateTime.Now;

    public required MigrationProject Project { get; init; }
}
