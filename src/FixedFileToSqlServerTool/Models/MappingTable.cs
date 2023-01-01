using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class MappingTable
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public List<MappingColumn> Columns { get; } = new List<MappingColumn>();

    public static MappingTable Create(string name) =>
        new MappingTable
        {
            Id = ObjectId.NewObjectId(),
            Name = name
        };
}
