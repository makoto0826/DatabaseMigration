using CommunityToolkit.Mvvm.ComponentModel;
using HanumanInstitute.MvvmDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

public class DatabaseSettingDialogViewModel : ObservableObject, IModalDialogViewModel
{
    public bool? DialogResult => true;
}
