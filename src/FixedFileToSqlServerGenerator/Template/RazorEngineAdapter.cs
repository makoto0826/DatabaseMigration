using Microsoft.Extensions.Logging;
using RazorEngineCore;

namespace FixedFileToSqlServerGenerator.Generator.Template;

public class RazorEngineAdapter
{
    private readonly string _templateDirectoryPath;

    private readonly IRazorEngine _engine;

    private readonly ILogger _logger;

    public RazorEngineAdapter(string templateDirectoryPath, ILogger<RazorEngineAdapter> logger)
    {
        _templateDirectoryPath = templateDirectoryPath;

        if (!Directory.Exists(_templateDirectoryPath))
        {
            throw new ArgumentException("テンプレートディレクトリがありません");
        }

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _engine = new RazorEngine();
    }

    public async Task<TemplateResult> RunAsync(TemplateContext context)
    {
        var nameWithTaskList =
            new DirectoryInfo(_templateDirectoryPath)
               .GetFiles()
               .Select(fileInfo =>
               {
                   var sp = fileInfo.Name.Split(new char[] { '.' });
                   var name = $"{sp[0]}.{sp[1]}";

                   if (sp[1] == "csproj")
                   {
                       name = $"{context.Project.Name}.{sp[1]}";
                   }

                   var task = _engine.RunFromFileAsync(fileInfo.FullName, model: context);
                   return (name, task);
               });

        await Task.WhenAll(nameWithTaskList.Select(x => x.task));

        return new TemplateResult
        {
            Items = nameWithTaskList.Select(x => new TemplateItem(x.name, x.task.Result)).ToList()
        };
    }
}
