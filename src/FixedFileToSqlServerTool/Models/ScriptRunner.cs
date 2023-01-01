using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FixedFileToSqlServerTool.Models;

public static class ScriptRunner
{
    public static Task<ScriptRunResult> RunAsync(string code, ScriptVariables variables)
    {
        return Task.Run(async () =>
        {
            try
            {
                var script = CSharpScript.Create<object>(code, globalsType: typeof(ScriptVariables));
                script.Compile();

                var scriptState = await script.RunAsync(variables);
                return new ScriptRunResult(true, scriptState.ReturnValue, null);
            }
            catch (CompilationErrorException ex)
            {
                return new ScriptRunResult(false, null, String.Join(Environment.NewLine, ex.Diagnostics));
            }
            catch (Exception ex)
            {
                return new ScriptRunResult(false, null, ex.Message);
            }
        });
    }
}
