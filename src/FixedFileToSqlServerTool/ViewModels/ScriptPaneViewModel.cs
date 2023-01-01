using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Models;
using FixedFileToSqlServerTool.ViewModels.Messages;
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

    private readonly Models.Script _script;

    public ScriptPaneViewModel(Models.Script script)
    {
        _script = script;
        this.Name = _script.Name;
        this.Id = _script.Id;
        this.codeDocument = new TextDocument(_script.Code);
        this.logDocument = new TextDocument();
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));

    [RelayCommand]
    private void Save()
    {
        _script.Name = this.Name;
        _script.Code = this.CodeDocument.Text;
        _script.UpdatedAt = DateTime.Now;
        WeakReferenceMessenger.Default.Send(new SavingScriptMessage(_script));
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
