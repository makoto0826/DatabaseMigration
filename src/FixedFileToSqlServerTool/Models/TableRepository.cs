using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class TableRepository
{
    private const string CollectionName = "Tables";

    private readonly LiteDatabase _database;

    public TableRepository(LiteDatabase database) =>
        _database = database ?? throw new ArgumentNullException(nameof(database));

    public List<Table> FindAll() =>
        _database.GetCollection<Table>(CollectionName).FindAll().ToList();

    public void Save(List<Table> tables)
    {
        _database.GetCollection<Table>(CollectionName).DeleteAll();
        _database.GetCollection<Table>(CollectionName).InsertBulk(tables);
    }
}
