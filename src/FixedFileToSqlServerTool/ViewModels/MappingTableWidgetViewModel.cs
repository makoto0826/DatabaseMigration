using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTableWidgetViewModel
{
    public ObservableCollection<MappingColumnWidgetViewModel> Columns { get; }

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string? tableName;

    [ObservableProperty]
    private MappingTable table;

    private readonly IEnumerable<Script> _scripts;

    public MappingTableWidgetViewModel(MappingTable table, IEnumerable<Script> scripts)
    {
        _scripts = scripts;

        this.Name = table.Name;
        this.TableName = table.TableName;
        this.Table = table;
        this.Columns = new ObservableCollection<MappingColumnWidgetViewModel>(
            table.Columns
                .Select(x => new MappingColumnWidgetViewModel(x, _scripts))
                .ToList());
    }

    public void ChangeTable(Table table)
    {
        this.TableName = table.Name;
        this.Columns.Clear();

        this.Columns.AddRange(
            table.Columns.Select(x =>
                new MappingColumnWidgetViewModel(MappingColumn.Create(x), _scripts)
            )
        );
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
