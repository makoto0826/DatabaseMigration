using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using ICSharpCode.AvalonEdit.Document;
using LiteDB;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class ScriptPaneViewModel : ObservableObject, IPaneViewModel
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private bool isActive;

    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private TextDocument codeDocument;

    [ObservableProperty]
    private TextDocument logDocument;

    [ObservableProperty]
    private string testData;

    public string ContentId => nameof(ScriptPaneViewModel);

    public ObjectId Id { get; }

    private readonly ScriptWidgetViewModel _scriptWidget;

    private readonly ScriptRepository _scriptRepository;

    public ScriptPaneViewModel(ScriptWidgetViewModel scriptWidget,ScriptRepository scriptRepository)
    {
        _scriptWidget = scriptWidget;
        _scriptRepository = scriptRepository;

        this.Name = _scriptWidget.Script.Name;
        this.Id = _scriptWidget.Script.Id;
        this.codeDocument = new TextDocument(_scriptWidget.Script.Code);
        this.logDocument = new TextDocument();
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));

    [RelayCommand]
    private void Save()
    {
        var newScript = _scriptWidget.Script with {
            Name = this.Name,
            Code = this.CodeDocument.Text,
            UpdatedAt = DateTime.Now
        };

        _scriptRepository.Save(newScript);

        WeakReferenceMessenger.Default.Send(new SavedScriptMessage(newScript));
    }

    [RelayCommand]
    private async Task TestRun()
    {
        this.LogDocument = new TextDocument("実行中...しばらくお待ちください");
        var result = await ScriptRunner.RunAsync(this.CodeDocument.Text, new ScriptVariables { Value = this.testData });

        if (result.IsSucceeded)
        {
            this.LogDocument = new TextDocument($"""
実行結果
{result.ReturnValue?.ToString() ?? ""}
""");
        }
        else
        {
            this.LogDocument = new TextDocument(result.ErrorMessage);
        }
    }
}
