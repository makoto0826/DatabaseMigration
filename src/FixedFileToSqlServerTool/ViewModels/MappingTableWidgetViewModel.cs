using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTableWidgetViewModel
{
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private MappingTable table;

    public ObservableCollection<MappingColumnWidgetViewModel> Columns { get; }

    public MappingTableWidgetViewModel(MappingTable table)
    {
        this.table = table;
        this.Columns = new ObservableCollection<MappingColumnWidgetViewModel>(
            table.Columns
                .Select(x => new MappingColumnWidgetViewModel(x))
                .ToList());
    }
}
