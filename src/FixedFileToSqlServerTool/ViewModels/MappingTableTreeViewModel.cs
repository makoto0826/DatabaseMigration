using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTableTreeViewModel
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    private MappingTable _mappingTable;

    public string Name => _mappingTable.Name;

    public MappingTableTreeViewModel(MappingTable mappingTable) => _mappingTable = mappingTable;
}
