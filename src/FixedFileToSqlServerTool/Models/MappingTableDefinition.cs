using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class MappingTableDefinition
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public List<MappingColumnDefinition> Columns { get; } = new List<MappingColumnDefinition>();

    public static MappingTableDefinition Create(string name) =>
        new MappingTableDefinition
        {
            Id = ObjectId.NewObjectId(),
            Name = name
        };
}
