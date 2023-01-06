using CommunityToolkit.Mvvm.ComponentModel;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class ScriptTreeNodeViewModel
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    private Models.Script _script;

    public string Name => _script.Name;

    public ScriptTreeNodeViewModel(Models.Script script) => _script = script;
}
