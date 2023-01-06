using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
using ICSharpCode.AvalonEdit.Document;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTableContentPaneViewModel : IPaneViewModel
{
    public ObservableCollection<TableWidgetViewModel> Tables { get; }

    public MappingTableWidgetViewModel MappingTableWidget { get; }

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private TextDocument logDocument;

    [ObservableProperty]
    private TextDocument testDataDocument;

    [ObservableProperty]
    private DataTable testDataTable;

    private readonly MigrationDataCreator _migrationDataCreator;

    private readonly MappingTableRepository _mappingTableRepository;

    public MappingTableContentPaneViewModel(
        MappingTableWidgetViewModel mappingTableWidget,
        IEnumerable<TableWidgetViewModel> tables,
        MigrationDataCreator migrationDataCreator,
        MappingTableRepository mappingTableRepository
    )
    {
        _migrationDataCreator = migrationDataCreator;
        _mappingTableRepository = mappingTableRepository;

        this.MappingTableWidget = mappingTableWidget;
        this.Tables = new ObservableCollection<TableWidgetViewModel>(tables);
        this.TestDataDocument = new();
        this.LogDocument = new();
        this.TestDataTable = _migrationDataCreator.CreateEmpty(mappingTableWidget.Table);
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
        var newMappingTable = this.MappingTableWidget.ToMappingTable();
        _mappingTableRepository.Save(newMappingTable);
        WeakReferenceMessenger.Default.Send(new SavedMappingTableMessage(newMappingTable));
    }

    [RelayCommand]
    private async Task Test()
    {
        try
        {
            var mappingTable = this.MappingTableWidget.ToMappingTable();
            using var memStream = new MemoryStream();
            memStream.Write(Encoding.GetEncoding(mappingTable.Encoding).GetBytes(this.TestDataDocument.Text));
            memStream.Position = 0;

            var dataTable = await _migrationDataCreator.CreateAsync(mappingTable, memStream);
            this.TestDataTable = dataTable;
            this.LogDocument = new TextDocument();
        }
        catch (Exception ex)
        {
            this.LogDocument = new TextDocument(ex.Message);
        }
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
