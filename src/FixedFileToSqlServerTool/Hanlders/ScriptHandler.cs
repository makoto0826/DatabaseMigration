using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FixedFileToSqlServerTool.Hanlders;

public class ScriptHandler
{
    public Task<ScriptHandlerResult> HandleAsync(ScriptHandlerContext context)
    {
        return Task.Run(async () =>
        {
            try
            {
                var script = CSharpScript.Create<object>(context.Code, globalsType: typeof(ScriptVariables));
                script.Compile();

                var state = await script.RunAsync(new ScriptVariables(context.Variable));
                return new ScriptHandlerResult(true, state.ReturnValue, null);
            }
            catch (CompilationErrorException ex)
            {
                return new ScriptHandlerResult(false, null, string.Join(Environment.NewLine, ex.Diagnostics));
            }
            catch (Exception ex)
            {
                return new ScriptHandlerResult(false, null, ex.Message);
            }
        });
    }

    private class ScriptVariables
    {
        public string Value;

        public ScriptVariables(string? value)
        {
            this.Value = value ?? "";
        }
    }
}
