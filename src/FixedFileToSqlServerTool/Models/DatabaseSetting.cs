using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public class DatabaseSetting
{
    public ObjectId Id { get; set; }

    public string Server { get; set; }

    public int Port { get; set; }

    public string UserId { get; set; }

    public string Password { get; set; }
}
