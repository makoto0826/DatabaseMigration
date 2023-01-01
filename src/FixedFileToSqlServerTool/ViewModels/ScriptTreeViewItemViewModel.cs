using CommunityToolkit.Mvvm.ComponentModel;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class ScriptTreeViewItemViewModel : ObservableObject
{
    [ObservableProperty]
    private Models.Script script;

    public ScriptTreeViewItemViewModel(Models.Script script)
    {
        this.script = script;
    }
}
