using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MainWindowViewModel
{
    public ObservableCollection<IPaneViewModel> Documents { get; } = new();

    public ObservableCollection<MappingTableWidgetViewModel> MappingTables { get; } = new();

    public ObservableCollection<TableDefinition> Tables { get; } = new();

    public ObservableCollection<ScriptWidgetViewModel> Scripts { get; } = new();

    private int mappingCount = 1;

    private int scriptCount = 1;

    private readonly DatabaseSettingRepository _databaseSettingRepository;

    private readonly TableDefinitionRepository _tableDefinitionRepository;

    private readonly ScriptRepository _scriptRepository;

    private readonly IDialogService _dialogService;

    public MainWindowViewModel(
        DatabaseSettingRepository databaseSettingRepository,
        TableDefinitionRepository tableDefinitionRepository,
        ScriptRepository scriptRepository,
        IDialogService dialogService)
    {
        _databaseSettingRepository = databaseSettingRepository;
        _tableDefinitionRepository = tableDefinitionRepository;
        _scriptRepository = scriptRepository;
        _dialogService = dialogService;

        WeakReferenceMessenger.Default.Register<ClosedPaneMessage>(this, this.HandleClosePane);
        WeakReferenceMessenger.Default.Register<SavedScriptMessage>(this, this.HandleSavedScript);
        WeakReferenceMessenger.Default.Register<ChangedIsSelectedPaneMessage>(this, this.HandleIsSelectedChanged);
    }

    private void HandleIsSelectedChanged(object _, ChangedIsSelectedPaneMessage mesage)
    {
        if (mesage.Value is ScriptPaneViewModel scriptVm)
        {
            foreach (var scriptWidget in this.Scripts)
            {
                scriptWidget.IsSelected = scriptWidget.Script.Id == scriptVm.Id;
            }
        }
        else if (mesage.Value is MappingTablePaneViewModel mappingTableVm)
        {
            foreach (var mappingTableWidget in this.MappingTables)
            {
                mappingTableWidget.IsSelected = mappingTableWidget.Table.Id == mappingTableVm.Id;
            }
        }
        else
        {

        }
    }

    private void HandleClosePane(object _, ClosedPaneMessage message)
    {
        this.Documents.Remove(message.Value);
    }

    private void HandleSavedScript(object _, SavedScriptMessage message)
    {
        var storedScirptWidget = this.Scripts.FirstOrDefault(x => x.Script.Id == message.Value.Id);

        if (storedScirptWidget is null)
        {
            this.Scripts.Add(new(message.Value));
        }
        else
        {
            storedScirptWidget.Script = message.Value;
        }
    }

    [RelayCommand]
    private void Loaded()
    {
        var tables = _tableDefinitionRepository.FindAll();
        this.Tables.AddRange(tables);

        var scripts = _scriptRepository.FindAll();
        this.Scripts.AddRange(scripts.Select(x => new ScriptWidgetViewModel(x)));
    }

    [RelayCommand]
    private void ShowDatabaseSettingDialog()
    {
        var vm = _dialogService.CreateViewModel<DatabaseSettingDialogViewModel>();
        _dialogService.ShowDialog(this, vm);
    }

    [RelayCommand]
    private async Task RefreshTable()
    {
        var setting = _databaseSettingRepository.Get();

        if (setting is null)
        {
            _dialogService.ShowMessageBox(this, text: "データベースの設定情報がありません", title: "エラー");
            return;
        }

        using var connection = setting.CreateConnection();

        try
        {
            await connection.OpenAsync();
            var repository = new SqlServerMetadataRepository(connection);
            var tables = await repository.GetTableDefinitionsAsync();

            _tableDefinitionRepository.Save(tables);
            this.Tables.Clear();
            this.Tables.AddRange(tables);
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox(this, text: ex.Message, title: "エラー");
        }
    }

    [RelayCommand]
    private void AddMappingTable()
    {
        var table = MappingTableDefinition.Create($"新規マッピングテーブル{mappingCount++}");
        this.MappingTables.Add(new(table));

        var vm = new MappingTablePaneViewModel(new(table));
        this.Documents.Add(vm);
    }

    [RelayCommand]
    private void AddScript()
    {
        var script = Script.Create($"新規スクリプト{scriptCount++}");
        var vm = new ScriptPaneViewModel(new(script), _scriptRepository);
        this.Documents.Add(vm);
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTableWidget))]
    public void DeleteMappingTable(MappingTableWidgetViewModel mappingTableWidget)
    {

    }

    [RelayCommand(CanExecute = nameof(CheckScriptWidget))]
    private void DeleteScript(ScriptWidgetViewModel scriptWidget)
    {

    }

    [RelayCommand(CanExecute = nameof(CheckMappingTableWidget))]
    private void OpenMappingTable(MappingTableWidgetViewModel mappingTableWidget)
    {
        var vmList = this.Documents.OfType<MappingTablePaneViewModel>().ToList();

        foreach (var vm in vmList)
        {
            vm.IsSelected = false;
        }

        var hitVm = vmList.FirstOrDefault(x => x.Id == mappingTableWidget.Table.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new MappingTablePaneViewModel(mappingTableWidget)
            {
                IsSelected = true
            });
        }
        else
        {
            hitVm.IsSelected = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CheckScriptWidget))]
    private void OpenScript(ScriptWidgetViewModel scriptWidget)
    {
        var vmList = this.Documents.OfType<ScriptPaneViewModel>().ToList();

        foreach (var vm in vmList)
        {
            vm.IsSelected = false;
        }

        var hitVm = vmList.FirstOrDefault(x => x.Id == scriptWidget.Script.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new ScriptPaneViewModel(scriptWidget, _scriptRepository)
            {
                IsSelected = true
            });
        }
        else
        {
            hitVm.IsSelected = true;
        }
    }

    private bool CheckMappingTableWidget(MappingTableWidgetViewModel? mappingTableWidget) => mappingTableWidget is not null;

    private bool CheckScriptWidget(ScriptWidgetViewModel? scriptWidget) => scriptWidget is not null;
}
