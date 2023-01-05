using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;
using MessageBoxButton = HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton;

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

    private readonly MappingTableRepository _mappingTableRepository;

    private readonly TableRepository _tableRepository;

    private readonly ScriptRepository _scriptRepository;

    private readonly IDialogService _dialogService;

    public MainWindowViewModel(
        DatabaseSettingRepository databaseSettingRepository,
        MappingTableRepository mappingTableRepository,
        TableRepository tableRepository,
        ScriptRepository scriptRepository,
        IDialogService dialogService)
    {
        _databaseSettingRepository = databaseSettingRepository;
        _mappingTableRepository = mappingTableRepository;
        _tableRepository = tableRepository;
        _scriptRepository = scriptRepository;
        _dialogService = dialogService;

        WeakReferenceMessenger.Default.Register<ClosedPaneMessage>(this, this.HandleClosePane);
        WeakReferenceMessenger.Default.Register<SavedScriptMessage>(this, this.HandleSavedScript);
        WeakReferenceMessenger.Default.Register<SavedMappingTableMessage>(this, this.HandleSavedMappingTable);
        WeakReferenceMessenger.Default.Register<ChangedIsSelectedPaneMessage>(this, this.HandleIsSelectedChanged);
    }

    private void HandleIsSelectedChanged(object _, ChangedIsSelectedPaneMessage mesage)
    {
        if (mesage.Value is ScriptContentPaneViewModel scriptVm)
        {
            foreach (var scriptWidget in this.Scripts)
            {
                scriptWidget.IsSelected = scriptWidget.Script.Id == scriptVm.ScriptWidget.Script.Id;
            }
        }
        else if (mesage.Value is MappingTableContentPaneViewModel mappingTableVm)
        {
            foreach (var mappingTableWidget in this.MappingTables)
            {
                mappingTableWidget.IsSelected = mappingTableWidget.Table.Id == mappingTableVm.MappingTableWidget.Table.Id;
            }
        }
        else if (mesage.Value is TableContentPaneViewModel tableVm)
        {
            foreach (var tableWidget in this.Tables)
            {
                tableWidget.IsSelected = tableWidget.Table.Id == tableVm.TableWidget.Table.Id;
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

    private void HandleSavedMappingTable(object _, SavedMappingTableMessage message)
    {
        var storedMappingTableWidget = this.MappingTables.FirstOrDefault(x => x.Table.Id == message.Value.Id);

        if (storedMappingTableWidget is null)
        {
            this.MappingTables.Add(new(message.Value, this.Scripts.Select(x => x.Script)));
        }
        else
        {
            storedMappingTableWidget.Table = message.Value;
        }
    }

    [RelayCommand]
    private void Loaded()
    {
        this.Tables.AddRange(_tableRepository.FindAll().Select(x => new TableWidgetViewModel(x)));
        this.Scripts.AddRange(_scriptRepository.FindAll().Select(x => new ScriptWidgetViewModel(x)));
        this.MappingTables.AddRange(_mappingTableRepository.FindAll().Select(x => new MappingTableWidgetViewModel(x, this.Scripts.Select(x => x.Script))));
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
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
            var repository = new MetadataRepository(connection);
            var tables = await repository.GetTableDefinitionsAsync();

            _tableRepository.Save(tables);
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
        var table = MappingTable.Create($"新規マッピングテーブル{mappingCount++}");
        var vm = new MappingTableContentPaneViewModel(
            new(table, this.Scripts.Select(x => x.Script)),
            this.Tables,
            Ioc.Default.GetRequiredService<MigrationDataCreator>(),
            _mappingTableRepository
        );

        this.Documents.Add(vm);
    }

    [RelayCommand]
    private void AddScript()
    {
        var script = Script.Create($"新規スクリプト{scriptCount++}");
        var vm = new ScriptContentPaneViewModel(new(script), _scriptRepository, Ioc.Default.GetRequiredService<ScriptRunner>());
        this.Documents.Add(vm);
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    public void DeleteMappingTable(MappingTableWidgetViewModel mappingTableWidget)
    {
        var isOk = _dialogService.ShowMessageBox(this, text: $"{mappingTableWidget.Table.Name}を削除しますか?", title: "マッピングテーブル削除", MessageBoxButton.YesNo);

        if (!isOk ?? false)
        {
            return;
        }

        _mappingTableRepository.Delete(mappingTableWidget.Table);

        this.MappingTables.Remove(mappingTableWidget);
        var vm = this.Documents.OfType<MappingTableContentPaneViewModel>().FirstOrDefault(x => x.MappingTableWidget.Table.Id == mappingTableWidget.Table.Id);

        if (vm is not null)
        {
            this.Documents.Remove(vm);
        }
    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void DeleteScript(ScriptWidgetViewModel scriptWidget)
    {
        var isOk = _dialogService.ShowMessageBox(this, text: $"{scriptWidget.Script.Name}を削除しますか?", title: "スクリプト削除", MessageBoxButton.YesNo);

        if (!isOk ?? false)
        {
            return;
        }

        _scriptRepository.Delete(scriptWidget.Script);

        this.Scripts.Remove(scriptWidget);
        var vm = this.Documents.OfType<ScriptContentPaneViewModel>().FirstOrDefault(x => x.ScriptWidget.Script.Id == scriptWidget.Script.Id);

        if (vm is not null)
        {
            this.Documents.Remove(vm);
        }
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    private void OpenMappingTable(MappingTableWidgetViewModel mappingTableWidget)
    {
        var vmList = this.Documents.OfType<MappingTableContentPaneViewModel>().ToList();
        var hitVm = vmList.FirstOrDefault(x => x.MappingTableWidget.Table.Id == mappingTableWidget.Table.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new MappingTableContentPaneViewModel(
                mappingTableWidget,
                this.Tables,
                 Ioc.Default.GetRequiredService<MigrationDataCreator>(),
                _mappingTableRepository)
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
        var vmList = this.Documents.OfType<TableContentPaneViewModel>().ToList();
        var hitVm = vmList.FirstOrDefault(x => x.TableWidget.Table.Id == tableWidget.Table.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new TableContentPaneViewModel(tableWidget)
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
        var vmList = this.Documents.OfType<ScriptContentPaneViewModel>().ToList();
        var hitVm = vmList.FirstOrDefault(x => x.ScriptWidget.Script.Id == scriptWidget.Script.Id);

        if (hitVm is null)
        {
            this.Documents.Add(new ScriptContentPaneViewModel(
                scriptWidget,
                _scriptRepository,
                Ioc.Default.GetRequiredService<ScriptRunner>())
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
