namespace FixedFileToSqlServerTool.Hanlders;

public record class ScriptHandlerResult(
    bool IsSucceeded,
    object? ReturnValue,
    string? ErrorMessage
);
