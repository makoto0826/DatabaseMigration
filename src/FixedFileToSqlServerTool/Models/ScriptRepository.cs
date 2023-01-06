using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class ScriptRepository
{
    private const string CollectionName = "Scripts";

    private readonly LiteDatabase _database;

    public ScriptRepository(LiteDatabase database) =>
        _database = database ?? throw new ArgumentNullException(nameof(database));

    public List<Script> FindAll() =>
        _database.GetCollection<Script>(CollectionName).FindAll().ToList();

    public void Save(Script script) =>
        _database.GetCollection<Script>(CollectionName).Upsert(script);

    public void Delete(Script script) =>
        _database.GetCollection<Script>(CollectionName).Delete(script.Id);
}
