using CommunityToolkit.Mvvm.ComponentModel;
using ICSharpCode.AvalonEdit.Document;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class ScriptWidgetViewModel
{
    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private TextDocument codeDocument;

    [ObservableProperty]
    private Models.Script script;

    public ScriptWidgetViewModel(Models.Script script)
    {
        this.script = script;
        this.Name = this.Script.Name;
        this.CodeDocument = new(this.Script.Code);
    }

    public void Renew(Models.Script script)
    {
        this.Script = script;
        this.Name = this.Script.Name;
        this.CodeDocument = new(this.Script.Code);
    }

    public Models.Script ToScript() =>
        this.Script with
        {
            Name = this.Name,
            Code = this.CodeDocument.Text,
            UpdatedAt = DateTime.Now
        };
}
