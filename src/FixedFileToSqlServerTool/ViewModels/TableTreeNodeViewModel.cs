using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class TableTreeNodeViewModel
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    private Table _table;

    public string Name => _table.Name;

    public TableTreeNodeViewModel(Table table) => _table = table;
}
