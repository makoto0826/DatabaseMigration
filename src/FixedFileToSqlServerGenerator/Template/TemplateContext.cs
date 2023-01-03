using FixedFileToSqlServerGenerator.Data;

namespace FixedFileToSqlServerGenerator.Generator.Template;

public class TemplateContext
{
    public DateTime Now { get; } = DateTime.Now;

    public required MigrationProject Project { get; init; }
}
