using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixedFileToSqlServerTool.Models;
using LiteDB;

namespace FixedFileToSqlServerTool.Infrastructures;

public class DatabaseSettingRepository
{
    private readonly LiteDatabase _database;

    public DatabaseSettingRepository(LiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public DatabaseSetting? Get() =>
        _database.GetCollection<DatabaseSetting>().FindAll().FirstOrDefault();

    public void Save(DatabaseSetting setting)
    {
        _database.GetCollection<DatabaseSetting>().DeleteAll();
        _database.GetCollection<DatabaseSetting>().Insert(setting);
    }
}
