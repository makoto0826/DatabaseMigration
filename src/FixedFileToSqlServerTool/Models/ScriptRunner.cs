using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FixedFileToSqlServerTool.Models;

public class ScriptRunner
{
    private readonly ConcurrentDictionary<string, Script<object>> _caches = new();

    public void CacheClear() => _caches.Clear();

    public Task<ScriptRunnerResult> RunAsync(ScriptRunnerContext context)
    {
        return Task.Run(async () =>
        {
            try
            {
                if (!_caches.TryGetValue(context.Code, out var script))
                {
                    script = CSharpScript.Create<object>(context.Code, globalsType: typeof(ScriptVariables));
                    script.Compile();
                    _caches.TryAdd(context.Code, script);
                }

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

    public class ScriptVariables
    {
        public string Value;

        public ScriptVariables(string? value)
        {
            Value = value ?? "";
        }
    }
}
