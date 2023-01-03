using Microsoft.Data.SqlClient;

namespace FixedFileToSqlServerTool.Models;

public static class DatabaseSettingExtensions
{
    public static SqlConnection CreateConnection(this DatabaseSetting setting)
    {
        var builder = new SqlConnectionStringBuilder();
        builder.Add("Server", $"{setting.Server},{setting.Port ?? 1433}");
        builder.Add("User ID", setting.UserId);
        builder.Add("Password", setting.Password);
        builder.Add("Database", setting.Database);
        builder.Encrypt = false;

        return new SqlConnection(builder.ToString());
    }
}
