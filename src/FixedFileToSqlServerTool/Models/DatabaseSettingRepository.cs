using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class DatabaseSettingRepository
{
    private const string CollectionName = "DatabaseSettings";

    private readonly LiteDatabase _database;

    public DatabaseSettingRepository(LiteDatabase database) =>
        _database = database ?? throw new ArgumentNullException(nameof(database));

    public DatabaseSetting? Get() =>
        _database.GetCollection<DatabaseSetting>(CollectionName).FindAll().FirstOrDefault();

    public void Save(DatabaseSetting setting)
    {
        _database.GetCollection<DatabaseSetting>(CollectionName).DeleteAll();
        _database.GetCollection<DatabaseSetting>(CollectionName).Insert(setting);
    }
}
