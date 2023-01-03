using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public record class MappingTableDefinition
{
    public required ObjectId Id { get; init; }

    public required string Encoding { get; init; }

    public required string Name { get; init; }

    public required List<MappingColumnDefinition> Columns { get; init; }

    public static MappingTableDefinition Create(string name) =>
        new MappingTableDefinition
        {
            Id = ObjectId.NewObjectId(),
            Name = name,
            Encoding = "Shift-JIS",
            Columns = new List<MappingColumnDefinition>()
        };
}
