namespace FixedFileToSqlServerTool.Models;

public record class ScriptRunnerContext(string Code, string Variable = "");
