using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FixedFileToSqlServerTool.Models;

public class ScriptRunner
{
    private readonly ConcurrentDictionary<string, Script<object>> _caches = new();

    public void ClearCache() => _caches.Clear();

    public Task<object> RunAsync(string code, string? variable = null) =>
        Task.Run(async () =>
        {
            try
            {
                if (!_caches.TryGetValue(code, out var script))
                {
                    script = CSharpScript.Create<object>(code, globalsType: typeof(ScriptVariables));
                    script.Compile();
                    _caches.TryAdd(code, script);
                }

                var state = await script.RunAsync(new ScriptVariables(variable));
                return state.ReturnValue;
            }
            catch (CompilationErrorException ex)
            {
                throw new ModelException(String.Join(Environment.NewLine, ex.Diagnostics), ex);
            }
            catch (Exception ex)
            {
                throw new ModelException(ex.Message);
            }
        });

    public class ScriptVariables
    {
        public string Value;

        public ScriptVariables(string? value) => Value = value;
    }
}
