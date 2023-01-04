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
    private MappingTable table;

    private readonly IEnumerable<Script> _scripts;

    public MappingTableWidgetViewModel(MappingTable table, IEnumerable<Script> scripts)
    {
        _scripts = scripts;
        this.name = table.Name;
        this.table = table;
        this.Columns = new ObservableCollection<MappingColumnWidgetViewModel>(
            table.Columns
                .Select(x => new MappingColumnWidgetViewModel(x, _scripts))
                .ToList());
    }

    public void ChangeTable(Table table)
    {
        this.Name = table.Name;
        this.Columns.Clear();

        this.Columns.AddRange(
            table.Columns.Select(x =>
                new MappingColumnWidgetViewModel(MappingColumn.Create(x), _scripts)
            )
        );
    }
}
