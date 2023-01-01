using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Infrastructures;
using FixedFileToSqlServerTool.Models;
using FixedFileToSqlServerTool.ViewModels.Messages;
using HanumanInstitute.MvvmDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<IPaneViewModel> DocumentPanes { get; } = new();

    public ObservableCollection<MappingTable> Tables { get; } = new();

    public ObservableCollection<Script> Scripts { get; } = new();

    [ObservableProperty]
    private bool isLoading;

    private int mappingCount = 1;

    private int scriptCount = 1;

    private readonly ScriptRepository _scriptRepository;

    private readonly IDialogService _dialogService;

    public MainWindowViewModel(ScriptRepository scriptRepository, IDialogService dialogService)
    {
        _scriptRepository = scriptRepository;
        _dialogService = dialogService;

        WeakReferenceMessenger.Default.Register<ClosedPaneMessage>(this, this.HandleClosePane);
        WeakReferenceMessenger.Default.Register<SavingScriptMessage>(this, this.HandleSaveScript);
    }

    private void HandleClosePane(object _, ClosedPaneMessage message) => this.DocumentPanes.Remove(message.Value);

    private void HandleSaveScript(object _, SavingScriptMessage message) => _scriptRepository.Save(message.Value);

    [RelayCommand]
    private void Loaded()
    {
        this.IsLoading = true;
        var scripts = _scriptRepository.FindAll();

        foreach (var script in scripts)
        {
            this.Scripts.Add(script);
        }

        this.IsLoading = false;
    }

    [RelayCommand]
    private void ShowDatabaseSettingDialog()
    {
        var vm = _dialogService.CreateViewModel<DatabaseSettingDialogViewModel>();
        _dialogService.ShowDialog(this, vm);
    }

    [RelayCommand]
    private void AddMappingTable()
    {
        var vm = new MappingTablePaneViewModel(MappingTable.Create($"新規マッピングテーブル{mappingCount++}"));
        this.DocumentPanes.Add(vm);
        this.Tables.Add(vm.Table);
    }

    [RelayCommand]
    private void AddScript()
    {
        var script = Script.Create($"新規スクリプト{scriptCount++}");
        this.Scripts.Add(script);

        var vm = new ScriptPaneViewModel(script);
        this.DocumentPanes.Add(vm);

        _scriptRepository.Save(script);
    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    public void DeleteMappingTable(MappingTable table)
    {

    }

    [RelayCommand(CanExecute = nameof(CheckScript))]
    private void DeleteScript(Script script)
    {

    }

    [RelayCommand(CanExecute = nameof(CheckMappingTable))]
    private void OpenMappingTable(MappingTable table)
    {
        var vmList = this.DocumentPanes.OfType<MappingTablePaneViewModel>().ToList();

        foreach (var vm in vmList)
        {
            vm.IsActive = false;
        }

        var hitVm = vmList.FirstOrDefault(x => x.Table.Id == table.Id);

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
            this.DocumentPanes.Add(new ScriptPaneViewModel(script)
            {
                IsActive = true
            });
        }
        else
        {
            hitVm.IsActive = true;
        }
    }

    private bool CheckMappingTable(MappingTable? table) => table is not null;

    private bool CheckScript(Script? script) => script is not null;
}
