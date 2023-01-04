using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingColumnWidgetViewModel
{
    [ObservableProperty]
    private bool isGeneration;

    [ObservableProperty]
    private int? startPosition;

    [ObservableProperty]
    private int? endPosition;

    [ObservableProperty]
    private Column destination;

    [ObservableProperty]
    private Script? generationScript;

    [ObservableProperty]
    private Script? convertScript;

    public MappingColumnWidgetViewModel(MappingColumn mappingColumn)
    {
        this.IsGeneration = mappingColumn.IsGeneration;
        this.StartPosition = mappingColumn.Source?.StartPosition;
        this.EndPosition = mappingColumn.Source?.EndPosition;
        this.Destination = mappingColumn.Destination;
        this.GenerationScript = mappingColumn.GenerationScript;
        this.ConvertScript = mappingColumn.ConvertScript;
    }
}
