using RazorEngineCore;

namespace DatabaseMigration;

public static class RazorEngineExtensions
{
    public static async Task<IRazorEngineCompiledTemplate> CompileFromFileAsync(this IRazorEngine engine, string filePath, Action<IRazorEngineCompilationOptionsBuilder>? buildAction = null)
    {
        using var reader = new StreamReader(filePath);
        var content = await reader.ReadToEndAsync();
        return await engine.CompileAsync(content, buildAction);
    }

    public static async Task<string> RunFromFileAsync(this IRazorEngine engine, string filePath, Action<IRazorEngineCompilationOptionsBuilder>? buildAction = null, object? model = null)
    {
        var template = await engine.CompileFromFileAsync(filePath);
        return await template.RunAsync(model);
    }
}