using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<IPaneViewModel> DocumentPanes { get; } = new();

    public ObservableCollection<MappingTableDefinition> MappingTables { get; } = new();

    public ObservableCollection<TableDefinition> Tables { get; } = new();

    public ObservableCollection<Script> Scripts { get; } = new();

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
    }

    private void HandleClosePane(object _, ClosedPaneMessage message)
    {
        this.DocumentPanes.Remove(message.Value);
    }

    private void HandleSavedScript(object _, SavedScriptMessage message)
    {
        var storedScirpt = this.Scripts.FirstOrDefault(x => x.Id == message.Value.Id);

        if(storedScirpt is null)
        {
            this.Scripts.Add(message.Value);
        }
        else
        {
            storedScirpt.Name = message.Value.Name;
        }
    }

    [RelayCommand]
    private void Loaded()
    {
        var tables = _tableDefinitionRepository.FindAll();
        this.Tables.AddRange(tables);

        var scripts = _scriptRepository.FindAll();
        this.Scripts.AddRange(scripts);
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

        }
    }

    [RelayCommand]
    private void AddMappingTable()
    {
        var table = MappingTableDefinition.Create($"新規マッピングテーブル{mappingCount++}");
        this.MappingTables.Add(table);

        var vm = new MappingTablePaneViewModel(table);
        this.DocumentPanes.Add(vm);
    }

    [RelayCommand]
    private void AddScript()
    {
        var script = Script.Create($"新規スクリプト{scriptCount++}");
        var vm = new ScriptPaneViewModel(script,_scriptRepository);
        this.DocumentPanes.Add(vm);
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    public void DeleteMappingTable(MappingTableDefinition table)
    {

    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void DeleteScript(Script script)
    {

    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    private void OpenMappingTable(MappingTableDefinition table)
    {
        var vmList = this.DocumentPanes.OfType<MappingTablePaneViewModel>().ToList();

        foreach (var vm in vmList)
        {
            vm.IsActive = false;
        }

        var hitVm = vmList.FirstOrDefault(x => x.Id == table.Id);

        if (hitVm is null)
        {
            this.DocumentPanes.Add(new MappingTablePaneViewModel(table)
            {
                IsActive = true
            });
        }
        else
        {
            hitVm.IsActive = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void OpenScript(Script script)
    {
        var vmList = this.DocumentPanes.OfType<ScriptPaneViewModel>().ToList();

        foreach (var vm in vmList)
        {
            vm.IsActive = false;
        }

        var hitVm = vmList.FirstOrDefault(x => x.Id == script.Id);

        if (hitVm is null)
        {
            this.DocumentPanes.Add(new ScriptPaneViewModel(script,_scriptRepository)
            {
                IsActive = true
            });
        }
        else
        {
            hitVm.IsActive = true;
        }
    }

    private bool CheckMappingTable(MappingTableDefinition? table) => table is not null;

    private bool CheckScript(Script? script) => script is not null;
}
