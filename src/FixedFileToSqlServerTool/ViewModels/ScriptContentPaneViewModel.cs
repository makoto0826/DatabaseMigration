using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using ICSharpCode.AvalonEdit.Document;
using LiteDB;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class ScriptContentPaneViewModel : IPaneViewModel
{
    [ObservableProperty]
    private string editName;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private TextDocument codeDocument;

    [ObservableProperty]
    private TextDocument logDocument;

    [ObservableProperty]
    private string testData;

    public ObjectId Id { get; }

    public ScriptWidgetViewModel ScriptWidget { get; }

    private readonly ScriptRepository _scriptRepository;

    private readonly ScriptRunner _scriptRunner;

    public ScriptContentPaneViewModel(
        ScriptWidgetViewModel scriptWidget,
        ScriptRepository scriptRepository,
        ScriptRunner scriptRunner)
    {
        _scriptRepository = scriptRepository;
        _scriptRunner = scriptRunner;

        this.ScriptWidget = scriptWidget;
        this.Id = scriptWidget.Script.Id;

        this.EditName = this.ScriptWidget.Script.Name;
        this.codeDocument = new TextDocument(scriptWidget.Script.Code);
        this.logDocument = new TextDocument();
    }

    partial void OnIsSelectedChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));

    [RelayCommand]
    private void Save()
    {
        var newScript = this.ScriptWidget.Script with
        {
            Name = this.EditName,
            Code = this.CodeDocument.Text,
            UpdatedAt = DateTime.Now
        };

        _scriptRepository.Save(newScript);

        WeakReferenceMessenger.Default.Send(new SavedScriptMessage(newScript));
    }

    [RelayCommand]
    private async Task Test()
    {
        this.LogDocument = new TextDocument("実行中...しばらくお待ちください");
        var result = await _scriptRunner.RunAsync(new ScriptRunnerContext(this.CodeDocument.Text, this.testData));

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
