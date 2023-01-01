using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixedFileToSqlServerTool.Models;
using LiteDB;

namespace FixedFileToSqlServerTool.Infrastructures;

public class ScriptRepository
{
    private readonly LiteDatabase _database;

    public ScriptRepository(LiteDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public List<Script> FindAll() =>
        _database.GetCollection<Script>().FindAll().ToList();

    public void Save(Script script) =>
        _database.GetCollection<Script>().Upsert(script);

    public void Delete(Script script) =>
        _database.GetCollection<Script>().Delete(script.Id);
}
