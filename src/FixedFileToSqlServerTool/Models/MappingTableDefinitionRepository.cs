using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class MappingTableDefinitionRepository
{
    private const string CollectionName = "mappingTableDefinitions";

    private readonly LiteDatabase _database;

    public MappingTableDefinitionRepository(LiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public List<MappingTableDefinition> FindAll() =>
        _database.GetCollection<MappingTableDefinition>(CollectionName).FindAll().ToList();

    public void Save(MappingTableDefinition table) =>
        _database.GetCollection<MappingTableDefinition>(CollectionName).Upsert(table);

    public void Delete(MappingTableDefinition table) =>
        _database.GetCollection<MappingTableDefinition>(CollectionName).Delete(table.Id);
}
