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
public partial class MappingTableContentViewModel : IContentViewModel
{
    public MappingTable MappingTable { get; }

    public List<Models.Table> Tables { get; }

    public ObservableCollection<MappingColumnViewModel> EditColumns { get; }

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

    private readonly IEnumerable<Models.Script> _scripts;

    private readonly DataTableCreator _dataTableCreator;

    private readonly MappingTableRepository _mappingTableRepository;

    public MappingTableContentViewModel(
        MappingTable mappingTable,
        IEnumerable<Models.Table> tables,
        IEnumerable<Models.Script> scripts,
        DataTableCreator dataTableCreator,
        MappingTableRepository mappingTableRepository
    )
    {
        _scripts = scripts;
        _dataTableCreator = dataTableCreator;
        _mappingTableRepository = mappingTableRepository;
        _scripts = scripts;

        this.MappingTable = mappingTable;
        this.EditName = mappingTable.Name;
        this.SelectedTableName = mappingTable.TableName;
        this.Tables = new(tables);
        this.TestDataDocument = new();
        this.LogDocument = new();
        this.TestDataTable = _dataTableCreator.CreateEmpty(mappingTable);

        this.EditColumns = new ObservableCollection<MappingColumnViewModel>(
            mappingTable.Columns
                .Select(x => new MappingColumnViewModel(x, scripts))
                .ToList());
    }

    partial void OnIsActiveChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsActiveMessage(this));

    [RelayCommand]
    private void TableSelectionChanged(Models.Table table)
    {
        this.EditColumns.Clear();
        this.EditColumns.AddRange(
            table.Columns.Select(x => new MappingColumnViewModel(MappingColumn.Create(x), _scripts))
        );
    }

    [RelayCommand]
    private void Save()
    {
        var newMappingTable = this.ToMappingTable();
        _mappingTableRepository.Save(newMappingTable);
        WeakReferenceMessenger.Default.Send(new SavedMappingTableMessage(newMappingTable));
    }

    [RelayCommand]
    private async Task TestAsync()
    {
        this.LogDocument = new("実行中...しばらくお待ちください");

        try
        {
            var mappingTable = this.ToMappingTable();
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
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));

    private MappingTable ToMappingTable() =>
        this.MappingTable with
        {
            Name = this.EditName,
            TableName = this.SelectedTableName,
            Columns = this.EditColumns.Select(x => new MappingColumn
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
