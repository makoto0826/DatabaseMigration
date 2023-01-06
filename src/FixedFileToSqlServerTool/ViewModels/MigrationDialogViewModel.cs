using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class MigrationDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    private static readonly List<FileFilter> _filters = new List<FileFilter>
    {
         new("固定長ファイル","*.txt")
    };

    public event EventHandler? RequestClose;

    public bool? DialogResult { get; private set; }

    [ObservableProperty]
    private string filePath;

    private readonly IDialogService _dialogService;

    public MigrationDialogViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    [RelayCommand]
    private void Open()
    {
        var filePath = _dialogService.ShowOpenFileDialog(this, new OpenFileDialogSettings
        {
            AllowMultiple = false,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Filters = _filters
        });

    }

    [RelayCommand]
    private void Cancel()
    {
        this.DialogResult = false;
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
