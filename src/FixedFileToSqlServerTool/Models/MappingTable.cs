using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public record class MappingTable
{
    public required ObjectId Id { get; init; }

    public required string Encoding { get; init; }

    public required string Name { get; init; }

    public string? TableName { get; init; }

    public required List<MappingColumn> Columns { get; init; }

    public static MappingTable Create(string name) =>
        new MappingTable
        {
            Id = ObjectId.NewObjectId(),
            Name = name,
            Encoding = "Shift-JIS",
            Columns = new List<MappingColumn>()
        };
}
