using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class TableDefinitionRepository
{
    private const string CollectionName = "tableDefinitions";

    private readonly LiteDatabase _database;

    public TableDefinitionRepository(LiteDatabase database) =>
        _database = database ?? throw new ArgumentNullException(nameof(database));

    public List<TableDefinition> FindAll() =>
        _database.GetCollection<TableDefinition>(CollectionName).FindAll().ToList();

    public void Save(List<TableDefinition> tables)
    {
        _database.GetCollection<TableDefinition>(CollectionName).DeleteAll();
        _database.GetCollection<TableDefinition>(CollectionName).InsertBulk(tables);
    }
}
