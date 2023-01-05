using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class MappingTableRepository
{
    private const string CollectionName = "MappingTables";

    private readonly LiteDatabase _database;

    public MappingTableRepository(LiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public List<MappingTable> FindAll() =>
        _database.GetCollection<MappingTable>(CollectionName).FindAll().ToList();

    public void Save(MappingTable table) =>
        _database.GetCollection<MappingTable>(CollectionName).Upsert(table);

    public void Delete(MappingTable table) =>
        _database.GetCollection<MappingTable>(CollectionName).Delete(table.Id);
}
