using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class TableWidgetViewModel
{
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private Table table;

    public TableWidgetViewModel(Table table)
    {
        this.table = table;
    }
}
