using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTableWidgetViewModel
{
    [ObservableProperty]
    private MappingTableDefinition table;

    public MappingTableWidgetViewModel(MappingTableDefinition table)
    {
        this.table = table;
    }
}
