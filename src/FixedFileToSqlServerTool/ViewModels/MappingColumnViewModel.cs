using CommunityToolkit.Mvvm.ComponentModel;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingColumnViewModel
{
    public MappingColumn Column { get; }

    public List<Script> GenerationScripts { get; }

    public List<Script> ConvertScripts { get; }

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

    public MappingColumnViewModel(MappingColumn mappingColumn, IEnumerable<Script> scripts)
    {
        this.IsGeneration = mappingColumn.IsGeneration;
        this.StartPosition = mappingColumn.Source?.StartPosition;
        this.EndPosition = mappingColumn.Source?.EndPosition;
        this.Destination = mappingColumn.Destination;
        this.GenerationScript = mappingColumn.GenerationScript;
        this.ConvertScript = mappingColumn.ConvertScript;
        this.Column = mappingColumn;
        this.GenerationScripts = new List<Script>(scripts);
        this.ConvertScripts = new List<Script>(scripts);
    }
}
