using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FixedFileToSqlServerTool.Models;

public class ScriptRunner
{
    public Task<ScriptRunnerResult> RunAsync(ScriptRunnerContext context)
    {
        return Task.Run(async () =>
        {
            try
            {
                var script = CSharpScript.Create<object>(context.Code, globalsType: typeof(ScriptVariables));
                script.Compile();

                var state = await script.RunAsync(new ScriptVariables(context.Variable));
                return new ScriptRunnerResult(true, state.ReturnValue, null);
            }
            catch (CompilationErrorException ex)
            {
                return new ScriptRunnerResult(false, null, string.Join(Environment.NewLine, ex.Diagnostics));
            }
            catch (Exception ex)
            {
                return new ScriptRunnerResult(false, null, ex.Message);
            }
        });
    }

    private class ScriptVariables
    {
        public string Value;

        public ScriptVariables(string? value)
        {
            Value = value ?? "";
        }
    }
}
