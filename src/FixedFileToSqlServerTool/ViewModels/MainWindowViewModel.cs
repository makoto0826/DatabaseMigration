using System.Collections.ObjectModel;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Hanlders;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MainWindowViewModel
{
    public ObservableCollection<IPaneViewModel> Documents { get; } = new();

    public ObservableCollection<MappingTableWidgetViewModel> MappingTables { get; } = new();

    public ObservableCollection<TableWidgetViewModel> Tables { get; } = new();

    public ObservableCollection<ScriptWidgetViewModel> Scripts { get; } = new();

    private int mappingCount = 1;

    private int scriptCount = 1;

    private readonly DatabaseSettingRepository _databaseSettingRepository;

    private readonly MappingTableDefinitionRepository _mappingTableDefinitionRepository;

    private readonly TableDefinitionRepository _tableDefinitionRepository;

    private readonly ScriptRepository _scriptRepository;

    private readonly IDialogService _dialogService;

    public MainWindowViewModel(
        DatabaseSettingRepository databaseSettingRepository,
        MappingTableDefinitionRepository mappingTableDefinitionRepository,
        TableDefinitionRepository tableDefinitionRepository,
        ScriptRepository scriptRepository,
        IDialogService dialogService)
    {
        _databaseSettingRepository = databaseSettingRepository;
        _mappingTableDefinitionRepository = mappingTableDefinitionRepository;
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
        else if (mesage.Value is TablePaneViewModel tableVm)
        {
            foreach (var tableWidget in this.Tables)
            {
                tableWidget.IsSelected = tableWidget.Table.Id == tableVm.Id;
            }
        }
    }

    private void HandleClosePane(object _, ClosedPaneMessage message) => this.Documents.Remove(message.Value);

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
        var mappingTables = _mappingTableDefinitionRepository.FindAll();
        this.MappingTables.AddRange(mappingTables.Select(x => new MappingTableWidgetViewModel(x)));

        var tables = _tableDefinitionRepository.FindAll();
        this.Tables.AddRange(tables.Select(x => new TableWidgetViewModel(x)));

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
            this.Tables.AddRange(tables.Select(x => new TableWidgetViewModel(x)));
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
        var vm = new MappingTablePaneViewModel(
            new(table),
            _mappingTableDefinitionRepository,
            _tableDefinitionRepository,
            _scriptRepository
        );

        this.Documents.Add(vm);
    }

    [RelayCommand]
    private void AddScript()
    {
        var script = Script.Create($"新規スクリプト{scriptCount++}");
        var vm = new ScriptPaneViewModel(
            new(script),
            _scriptRepository,
            Ioc.Default.GetRequiredService<ScriptHandler>()
        );

        this.Documents.Add(vm);
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    public void DeleteMappingTable(MappingTableWidgetViewModel mappingTableWidget)
    {
        var isOk = _dialogService.ShowMessageBox(this,
            text: $"{mappingTableWidget.Table.Name}を削除しますか?",
            title: "マッピングテーブル削除",
            MessageBoxButton.YesNo);

        if (!isOk ?? false)
        {
            return;
        }

        _mappingTableDefinitionRepository.Delete(mappingTableWidget.Table);

        this.MappingTables.Remove(mappingTableWidget);
        var vm = this.Documents.OfType<MappingTablePaneViewModel>().FirstOrDefault(x => x.Id == mappingTableWidget.Table.Id);

        if (vm is not null)
        {
            this.Documents.Remove(vm);
        }
    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void DeleteScript(ScriptWidgetViewModel scriptWidget)
    {
        var isOk = _dialogService.ShowMessageBox(this,
            text: $"{scriptWidget.Script.Name}を削除しますか?",
            title: "スクリプト削除",
            MessageBoxButton.YesNo);

        if (!isOk ?? false)
        {
            return;
        }

        _scriptRepository.Delete(scriptWidget.Script);

        this.Scripts.Remove(scriptWidget);
        var vm = this.Documents.OfType<ScriptPaneViewModel>().FirstOrDefault(x => x.Id == scriptWidget.Script.Id);

        if (vm is not null)
        {
            this.Documents.Remove(vm);
        }
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    private void OpenMappingTable(MappingTableWidgetViewModel mappingTableWidget)
    {
        var vmList = this.Documents.OfType<MappingTablePaneViewModel>().ToList();
        var hitVm = vmList.FirstOrDefault(x => x.Id == mappingTableWidget.Table.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new MappingTablePaneViewModel(
                mappingTableWidget,
                _mappingTableDefinitionRepository,
                _tableDefinitionRepository,
                _scriptRepository)
            {
                IsSelected = true
            });
        }
        else
        {
            hitVm.IsSelected = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CheckTable))]
    private void OpenTable(TableWidgetViewModel tableWidget)
    {
        var vmList = this.Documents.OfType<TablePaneViewModel>().ToList();
        var hitVm = vmList.FirstOrDefault(x => x.Id == tableWidget.Table.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new TablePaneViewModel(tableWidget)
            {
                IsSelected = true
            });
        }
        else
        {
            hitVm.IsSelected = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void OpenScript(ScriptWidgetViewModel scriptWidget)
    {
        var vmList = this.Documents.OfType<ScriptPaneViewModel>().ToList();
        var hitVm = vmList.FirstOrDefault(x => x.Id == scriptWidget.Script.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new ScriptPaneViewModel(
                scriptWidget,
                _scriptRepository,
                Ioc.Default.GetRequiredService<ScriptHandler>())
            {
                IsSelected = true
            });
        }
        else
        {
            hitVm.IsSelected = true;
        }
    }

    private bool CheckMappingTable(MappingTableWidgetViewModel? mappingTableWidget) => mappingTableWidget is not null;

    private bool CheckTable(TableWidgetViewModel? tableWidget) => tableWidget is not null;

    private bool CheckScript(ScriptWidgetViewModel? scriptWidget) => scriptWidget is not null;
}
