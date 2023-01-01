using CommunityToolkit.Mvvm.ComponentModel;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class ScriptWidgetViewModel : ObservableObject
{
    [ObservableProperty]
    private Models.Script script;

    public ScriptWidgetViewModel(Models.Script script)
    {
        this.script = script;
    }
}
