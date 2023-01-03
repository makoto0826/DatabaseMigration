using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingColumnWidgetViewModel
{
    [ObservableProperty]
    private bool isGeneration;

    [ObservableProperty]
    public int startPosition;

    [ObservableProperty]
    public int endPosition;

    [ObservableProperty]
    public ColumnDefinition destination;

    [ObservableProperty]
    public Script? generationScript;

    [ObservableProperty]
    public Script? convertScript;
}
