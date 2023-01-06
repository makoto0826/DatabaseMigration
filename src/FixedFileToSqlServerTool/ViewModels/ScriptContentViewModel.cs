using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using ICSharpCode.AvalonEdit.Document;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class ScriptContentViewModel : IPaneViewModel
{
    public Models.Script Script { get; }

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private string _editName;

    [ObservableProperty]
    private TextDocument _editCodeDocument;

    [ObservableProperty]
    private TextDocument _logDocument;

    [ObservableProperty]
    private string _testData;

    private readonly ScriptRepository _scriptRepository;

    private readonly ScriptRunner _scriptRunner;

    public ScriptContentViewModel(
        Models.Script script,
        ScriptRepository scriptRepository,
        ScriptRunner scriptRunner)
    {
        _scriptRepository = scriptRepository;
        _scriptRunner = scriptRunner;

        this.Script = script;
        this.EditName = script.Name;
        this.EditCodeDocument = new(script.Code);
        this.LogDocument = new();
    }

    partial void OnIsActiveChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));

    [RelayCommand]
    private void Save()
    {
        var newScript = this.Script.Renew(this.EditName, this.EditCodeDocument.Text);
        _scriptRepository.Save(newScript);

        WeakReferenceMessenger.Default.Send(new SavedScriptMessage(newScript));
    }

    [RelayCommand]
    private async Task TestAsync()
    {
        this.LogDocument = new("実行中...しばらくお待ちください");

        try
        {
            var result = await _scriptRunner.RunAsync(this.EditCodeDocument.Text, this.TestData);

            this.LogDocument = new($"""
実行結果
{result?.ToString() ?? ""}
""");
        }
        catch (ModelException ex)
        {
            this.LogDocument = new(ex.Message);
        }
    }
}
