using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using ICSharpCode.AvalonEdit.Document;

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

    [ObservableProperty]
    private TextDocument _logDocument;

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

    private readonly MigrationHandler _migrationHanlder;

    private readonly IDialogService _dialogService;

    public MigrationDialogViewModel(
        IEnumerable<MappingTable> mappingTables,
        DatabaseSetting databaseSetting,
        MigrationHandler migrationHandler,
        IDialogService dialogService)
    {
        _databaseSetting = databaseSetting;
        _migrationHanlder = migrationHandler;
        _dialogService = dialogService;

        this.LogDocument = new();
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
        this.IsRunning = true;
        this.LogDocument = new TextDocument("マイグレーション実行中です。しばらくお待ちください");

        try
        {
            await _migrationHanlder.HandleAsync(
                this.FilePath,
                this.SelectedMappingTable,
                _databaseSetting
            );
        }
        catch (ModelException ex)
        {
            this.LogDocument = new TextDocument(ex.Message);
        }
        finally
        {
            this.IsRunning = false;
        }

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
