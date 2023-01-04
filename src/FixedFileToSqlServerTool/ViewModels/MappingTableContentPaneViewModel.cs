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
public partial class MappingTableContentPaneViewModel : IPaneViewModel
{
    public ObservableCollection<TableWidgetViewModel> Tables { get; }

    public MappingTableWidgetViewModel MappingTableWidget { get; }

    public ObjectId Id { get; }

    [ObservableProperty]
    private string editName;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private TextDocument logDocument;

    private readonly MappingTableRepository _mappingTableRepository;

    public MappingTableContentPaneViewModel(
        MappingTableWidgetViewModel mappingTableWidget,
        IEnumerable<TableWidgetViewModel> tables,
        MappingTableRepository mappingTableRepository
    )
    {
        _mappingTableRepository = mappingTableRepository;

        this.editName = mappingTableWidget.Table.Name;
        this.Id = mappingTableWidget.Table.Id;
        this.MappingTableWidget = mappingTableWidget;
        this.Tables = new ObservableCollection<TableWidgetViewModel>(tables);
        this.LogDocument = new();
    }

    partial void OnIsSelectedChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void TableSelectionChanged(TableWidgetViewModel tableWidget)
    {
        this.MappingTableWidget.ChangeTable(tableWidget.Table);
    }

    [RelayCommand]
    private void Save()
    {
        var newMappingTable = this.MappingTableWidget.Table with
        {
            Name = this.EditName,
            TableName = this.MappingTableWidget.Name,

        };

        _mappingTableRepository.Save(newMappingTable);
        WeakReferenceMessenger.Default.Send(new SavedMappingTableMessage(newMappingTable));
    }

    [RelayCommand]
    private void Execute() { }

    [RelayCommand]
    private void Test() { }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
