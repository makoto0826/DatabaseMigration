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
         new("固定長ファイル","txt")
    };

    public event EventHandler? RequestClose;

    public bool? DialogResult { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunnable))]
    private string _filePath;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunnable))]
    private bool _isRunning;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunnable))]
    private MappingTable? _selectedMappingTable;

    public bool IsRunnable
    {
        get
        {
            if (this.IsRunning)
            {
                return false;
            }

            return !String.IsNullOrEmpty(this.FilePath) && this.SelectedMappingTable != null;
        }
    }

    public List<MappingTable> MappingTables { get; }

    private readonly DatabaseSetting _databaseSetting;

    private readonly IDialogService _dialogService;

    public MigrationDialogViewModel(
        IEnumerable<MappingTable> mappingTables,
        DatabaseSetting databaseSetting,
        IDialogService dialogService)
    {
        _databaseSetting = databaseSetting;
        _dialogService = dialogService;

        this.MappingTables = new(mappingTables);
    }

    [RelayCommand]
    private void Open()
    {
        this.FilePath = _dialogService.ShowOpenFileDialog(this, new OpenFileDialogSettings
        {
            AllowMultiple = false,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Filters = _filters
        });
    }

    [RelayCommand]
    private async Task RunAsync()
    {
        this.DialogResult = true;
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        this.DialogResult = false;
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
