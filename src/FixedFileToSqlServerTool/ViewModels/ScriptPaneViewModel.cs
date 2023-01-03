using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Hanlders;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using ICSharpCode.AvalonEdit.Document;
using LiteDB;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class ScriptPaneViewModel : IPaneViewModel
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private TextDocument codeDocument;

    [ObservableProperty]
    private TextDocument logDocument;

    [ObservableProperty]
    private string testData;

    public ObjectId Id { get; }

    private readonly ScriptWidgetViewModel _scriptWidget;

    private readonly ScriptRepository _scriptRepository;

    private readonly ScriptHandler _scriptHandler;

    public ScriptPaneViewModel(
        ScriptWidgetViewModel scriptWidget,
        ScriptRepository scriptRepository,
        ScriptHandler scriptHandler)
    {
        _scriptWidget = scriptWidget;
        _scriptRepository = scriptRepository;
        _scriptHandler = scriptHandler;

        this.Title = _scriptWidget.Script.Name;
        this.Id = _scriptWidget.Script.Id;
        this.codeDocument = new TextDocument(_scriptWidget.Script.Code);
        this.logDocument = new TextDocument();
    }

    partial void OnIsSelectedChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));

    [RelayCommand]
    private void Save()
    {
        var newScript = _scriptWidget.Script with
        {
            Name = this.Title,
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
        var result = await _scriptHandler.HandleAsync(new ScriptHandlerContext(this.CodeDocument.Text, this.testData));

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
