namespace FixedFileToSqlServerTool.Models;

public record class ScriptRunnerResult(
    bool IsSucceeded,
    object? ReturnValue,
    string? ErrorMessage
);
