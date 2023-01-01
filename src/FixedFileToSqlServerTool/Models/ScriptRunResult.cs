namespace FixedFileToSqlServerTool.Models;

public record class ScriptRunResult(
    bool IsSucceeded,
    object? ReturnValue,
    string? ErrorMessage
);
