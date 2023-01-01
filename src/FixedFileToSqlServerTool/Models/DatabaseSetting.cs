namespace FixedFileToSqlServerTool.Models;

public record class DatabaseSetting
{
    public string Server { get; init; } = String.Empty;

    public int? Port { get; init; }

    public string UserId { get; init; } = String.Empty;

    public string Password { get; init; } = String.Empty;

    public string Database { get; init; } = String.Empty;
}
