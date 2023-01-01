using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class TableWidgetViewModel
{
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private TableDefinition table;

    public TableWidgetViewModel(TableDefinition table)
    {
        this.table = table;
    }
}
