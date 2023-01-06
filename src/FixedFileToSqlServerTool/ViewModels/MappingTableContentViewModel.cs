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
using Microsoft.CodeAnalysis.Scripting;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTableContentViewModel : IPaneViewModel
{
    public List<Table> Tables { get; }

    public MappingTable MappingTable { get; }

    public ObservableCollection<MappingColumn> Columns { get; } = new();

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private string _editName;

    [ObservableProperty]
    private string _selectedTableName;

    [ObservableProperty]
    private TextDocument _logDocument;

    [ObservableProperty]
    private TextDocument _testDataDocument;

    [ObservableProperty]
    private DataTable _testDataTable;

    private readonly DataTableCreator _dataTableCreator;

    private readonly MappingTableRepository _mappingTableRepository;

    public MappingTableContentViewModel(
        MappingTable mappingTable,
        IEnumerable<Table> tables,
        DataTableCreator dataTableCreator,
        MappingTableRepository mappingTableRepository
    )
    {
        _dataTableCreator = dataTableCreator;
        _mappingTableRepository = mappingTableRepository;

        this.MappingTable = mappingTable;
        this.EditName = mappingTable.Name;
        this.SelectedTableName = mappingTable.TableName;
        this.Tables = new(tables);
        this.TestDataDocument = new();
        this.LogDocument = new();
        this.TestDataTable = _dataTableCreator.CreateEmpty(mappingTable);
    }

    partial void OnIsActiveChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    /*
    public void Renew(MappingTable table)
    {
        this.Name = table.Name;
        this.TableName = table.TableName;
        this.Table = table;
        this.Columns = new ObservableCollection<MappingColumnWidgetViewModel>(
            table.Columns
                .Select(x => new MappingColumnWidgetViewModel(x, _scripts))
                .ToList());
    }

    public MappingTable ToMappingTable() =>
        this.Table with
        {
            Name = this.Name,
            TableName = this.TableName,
            Columns = this.Columns.Select(x => new MappingColumn
            {
                IsGeneration = x.IsGeneration,
                Source = x.StartPosition.HasValue && x.EndPosition.HasValue ? new FixedColumn
                {
                    StartPosition = x.StartPosition.Value,
                    EndPosition = x.EndPosition.Value
                } : null,
                Destination = x.Destination with { },
                GenerationScript = x.GenerationScript,
                ConvertScript = x.ConvertScript
            })
            .ToList()
        };
}
    */

    [RelayCommand]
    private void TableSelectionChanged(Table table)
    {
        /*
        this.Columns.AddRange(
        table.Columns.Select(x =>
                new MappingColumnWidgetViewModel(MappingColumn.Create(x), _scripts)
            )
        );
        */
    }

    [RelayCommand]
    private void Save()
    {
        // var newMappingTable = this.MappingTableWidget.ToMappingTable();
        // this.MappingTableWidget.Renew(newMappingTable);

        // _mappingTableRepository.Save(newMappingTable);
        // WeakReferenceMessenger.Default.Send(new SavedMappingTableMessage(newMappingTable));
    }

    [RelayCommand]
    private async Task TestAsync()
    {
        /*
        try
        {
            var mappingTable = this.MappingTableWidget.ToMappingTable();
            using var memStream = new MemoryStream();
            memStream.Write(Encoding.GetEncoding(mappingTable.Encoding).GetBytes(this.TestDataDocument.Text));
            memStream.Position = 0;

            var dataTable = await _dataTableCreator.CreateAsync(mappingTable, memStream);
            this.TestDataTable = dataTable;
            this.LogDocument = new();
        }
        catch (Exception ex)
        {
            this.LogDocument = new(ex.Message);
        }
        */
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
