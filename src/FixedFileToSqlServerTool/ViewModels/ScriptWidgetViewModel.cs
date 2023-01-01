using CommunityToolkit.Mvvm.ComponentModel;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class ScriptWidgetViewModel
{
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private Models.Script script;

    public ScriptWidgetViewModel(Models.Script script)
    {
        this.script = script;
    }
}
