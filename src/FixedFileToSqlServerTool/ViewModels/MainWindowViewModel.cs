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
    public ObservableCollection<IContentViewModel> Documents { get; } = new();

    public ObservableCollection<MappingTableTreeViewModel> MappingTables { get; } = new();

    public ObservableCollection<TableTreeNodeViewModel> Tables { get; } = new();

    public ObservableCollection<ScriptTreeNodeViewModel> Scripts { get; } = new();

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

        WeakReferenceMessenger.Default.Register<ClosedMessage>(this, this.HandleClosedMessage);
        WeakReferenceMessenger.Default.Register<SavedScriptMessage>(this, this.HandleSavedScriptMessage);
        WeakReferenceMessenger.Default.Register<SavedMappingTableMessage>(this, this.HandleSavedMappingTableMessage);
        WeakReferenceMessenger.Default.Register<ChangedIsActiveMessage>(this, this.HandleIsActiveMessage);
    }

    private void HandleIsActiveMessage(object _, ChangedIsActiveMessage mesage)
    {
        if (mesage.Value is ScriptContentViewModel scriptVm)
        {
            foreach (var node in this.Scripts)
            {
                node.IsSelected = node.Script.Id == scriptVm.Script.Id;
            }
        }
        else if (mesage.Value is MappingTableContentViewModel mappingTableVm)
        {
            foreach (var node in this.MappingTables)
            {
                node.IsSelected = node.MappingTable.Id == mappingTableVm.MappingTable.Id;
            }
        }
        else if (mesage.Value is TableContentViewModel tableVm)
        {
            foreach (var node in this.Tables)
            {
                node.IsSelected = node.Table.Id == tableVm.Table.Id;
            }
        }
    }

    private void HandleClosedMessage(object _, ClosedMessage message) => this.Documents.Remove(message.Value);

    private void HandleSavedScriptMessage(object _, SavedScriptMessage message)
    {
        var node = this.Scripts.FirstOrDefault(x => x.Script.Id == message.Value.Id);

        if (node is null)
        {
            this.Scripts.Add(new(message.Value));
        }
        else
        {
            node.Script = message.Value;
        }
    }

    private void HandleSavedMappingTableMessage(object _, SavedMappingTableMessage message)
    {
        var node = this.MappingTables.FirstOrDefault(x => x.MappingTable.Id == message.Value.Id);

        if (node is null)
        {
            this.MappingTables.Add(new(message.Value));
        }
        else
        {
            node.MappingTable = message.Value;
        }
    }

    [RelayCommand]
    private void Loaded()
    {
        this.Tables.AddRange(_tableRepository.FindAll().Select(x => new TableTreeNodeViewModel(x)));
        this.Scripts.AddRange(_scriptRepository.FindAll().Select(x => new ScriptTreeNodeViewModel(x)));
        this.MappingTables.AddRange(_mappingTableRepository.FindAll().Select(x => new MappingTableTreeViewModel(x)));
    }

    [RelayCommand]
    private void ShowMigrationDialog()
    {
        var databaseSetting = _databaseSettingRepository.Get();

        if (databaseSetting is null)
        {
            _dialogService.ShowMessageBox(this, text: "データベースの設定情報がありません", title: "エラー");
            return;
        }

        var vm = new MigrationDialogViewModel(
            this.MappingTables.Select(x => x.MappingTable),
            databaseSetting,
            Ioc.Default.GetRequiredService<MigrationHandler>(),
            _dialogService
        );

        _dialogService.ShowDialog(this, vm);
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
            this.Tables.AddRange(tables.Select(x => new TableTreeNodeViewModel(x)));
        }
        catch (Exception ex)
        {
            _dialogService.ShowMessageBox(this, text: ex.Message, title: "エラー");
        }
    }

    [RelayCommand]
    private void AddMappingTable()
    {
        var mappingTable = MappingTable.Create($"新規マッピングテーブル{mappingCount++}");
        var vm = new MappingTableContentViewModel(
            mappingTable,
            this.Tables.Select(x => x.Table),
            this.Scripts.Select(x => x.Script),
            Ioc.Default.GetRequiredService<DataTableCreator>(),
            _mappingTableRepository
        );

        this.Documents.Add(vm);
    }

    [RelayCommand]
    private void AddScript()
    {
        var script = Script.Create($"新規スクリプト{scriptCount++}");
        var vm = new ScriptContentViewModel(script, _scriptRepository, Ioc.Default.GetRequiredService<ScriptRunner>());
        this.Documents.Add(vm);
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    public void DeleteMappingTable(MappingTableTreeViewModel node)
    {
        var isOk = _dialogService.ShowMessageBox(this, text: $"{node.MappingTable.Name}を削除しますか?", title: "マッピングテーブル削除", MessageBoxButton.YesNo);

        if (!isOk ?? false)
        {
            return;
        }

        _mappingTableRepository.Delete(node.MappingTable);

        this.MappingTables.Remove(node);
        var vm = this.Documents.OfType<MappingTableContentViewModel>().FirstOrDefault(x => x.MappingTable.Id == node.MappingTable.Id);

        if (vm is not null)
        {
            this.Documents.Remove(vm);
        }
    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void DeleteScript(ScriptTreeNodeViewModel node)
    {
        var isOk = _dialogService.ShowMessageBox(this, text: $"{node.Script.Name}を削除しますか?", title: "スクリプト削除", MessageBoxButton.YesNo);

        if (!isOk ?? false)
        {
            return;
        }

        _scriptRepository.Delete(node.Script);

        this.Scripts.Remove(node);
        var vm = this.Documents.OfType<ScriptContentViewModel>().FirstOrDefault(x => x.Script.Id == node.Script.Id);

        if (vm is not null)
        {
            this.Documents.Remove(vm);
        }
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    private void OpenMappingTable(MappingTableTreeViewModel node)
    {
        var vm = this.Documents.OfType<MappingTableContentViewModel>().FirstOrDefault(x => x.MappingTable.Id == node.MappingTable.Id);

        if (vm is null)
        {
            this.Documents.Add(new MappingTableContentViewModel(
                node.MappingTable,
                this.Tables.Select(x => x.Table),
                this.Scripts.Select(x => x.Script),
                 Ioc.Default.GetRequiredService<DataTableCreator>(),
                _mappingTableRepository
            )
            {
                IsActive = true
            });
        }
        else
        {
            vm.IsActive = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CheckTable))]
    private void OpenTable(TableTreeNodeViewModel node)
    {
        var vm = this.Documents.OfType<TableContentViewModel>().FirstOrDefault(x => x.Table.Id == node.Table.Id);

        if (vm is null)
        {
            this.Documents.Add(new TableContentViewModel(node.Table) { IsActive = true });
        }
        else
        {
            vm.IsActive = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void OpenScript(ScriptTreeNodeViewModel node)
    {
        var vm = this.Documents.OfType<ScriptContentViewModel>().FirstOrDefault(x => x.Script.Id == node.Script.Id);

        if (vm is null)
        {
            this.Documents.Add(new ScriptContentViewModel(
                node.Script,
                _scriptRepository,
                Ioc.Default.GetRequiredService<ScriptRunner>())
            {
                IsActive = true
            });
        }
        else
        {
            vm.IsActive = true;
        }
    }

    private bool CheckMappingTable(MappingTableTreeViewModel node) => node is not null;

    private bool CheckTable(TableTreeNodeViewModel node) => node is not null;

    private bool CheckScript(ScriptTreeNodeViewModel node) => node is not null;
}
