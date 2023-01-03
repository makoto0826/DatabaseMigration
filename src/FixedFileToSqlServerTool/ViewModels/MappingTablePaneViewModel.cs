using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using ICSharpCode.AvalonEdit.Document;
using LiteDB;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTablePaneViewModel : IPaneViewModel
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private string filePath;

    [ObservableProperty]
    private TableWidgetViewModel selectedTable;

    [ObservableProperty]
    private TextDocument logDocument;

    public ObjectId Id { get; }

    private readonly MappingTableDefinitionRepository _mappingTableDefinitionRepository;

    private readonly MappingTableWidgetViewModel _mappingTableWidget;

    public ObservableCollection<ScriptWidgetViewModel> Scripts { get; }

    public ObservableCollection<TableWidgetViewModel> Tables { get; }

    public MappingTablePaneViewModel(
        MappingTableWidgetViewModel mappingTableWidget,
        ObservableCollection<ScriptWidgetViewModel> scripts,
        ObservableCollection<TableWidgetViewModel> tables,
        MappingTableDefinitionRepository mappingTableDefinitionRepository
    )
    {
        _mappingTableWidget = mappingTableWidget;
        _mappingTableDefinitionRepository = mappingTableDefinitionRepository;

        this.Title = mappingTableWidget.Table.Name;
        this.Id = mappingTableWidget.Table.Id;
        this.Scripts = scripts;
        this.Tables = tables;
        this.LogDocument = new();
    }

    partial void OnIsSelectedChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Save()
    {
        var newMappingTable = _mappingTableWidget.Table with
        {

        };

        _mappingTableDefinitionRepository.Save(newMappingTable);
        WeakReferenceMessenger.Default.Send(new MappingTableDefinitionMessage(newMappingTable));
    }

    [RelayCommand]
    private void Open() { }

    [RelayCommand]
    private void Execute() { }

    [RelayCommand]
    private void Test() { }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
