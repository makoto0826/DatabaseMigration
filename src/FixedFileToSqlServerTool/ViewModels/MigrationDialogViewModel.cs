using System.Data;
using System.IO;
using System.Windows;
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

    [ObservableProperty]
    private DataTable _targetDataTable;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private Visibility _fileVisibility;

    [ObservableProperty]
    private Visibility _runVisibility;

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

    private readonly Database _database;

    private readonly DataTableCreator _dataTableCreator;

    private readonly IDialogService _dialogService;

    public MigrationDialogViewModel(
        IEnumerable<MappingTable> mappingTables,
        Database database,
        DataTableCreator dataTableCreator,
        IDialogService dialogService)
    {
        _database = database;
        _dataTableCreator = dataTableCreator;
        _dialogService = dialogService;

        this.FileVisibility = Visibility.Visible;
        this.RunVisibility = Visibility.Collapsed;
        this.MappingTables = new(mappingTables);
        this.Title = "マイグレーション - ファイル選択";
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
    private async Task LoadAsync()
    {
        this.IsRunning = true;

        try
        {
            using var stream = new FileStream(this.FilePath, FileMode.Open);
            this.TargetDataTable = await _dataTableCreator.CreateFromStreamAsync(this.SelectedMappingTable, stream);
            this.FileVisibility = Visibility.Collapsed;
            this.RunVisibility = Visibility.Visible;
            this.Title = $"マイグレーション - テーブル {this.SelectedMappingTable.TableName} 実行";
        }
        catch (ModelException ex)
        {
            _dialogService.ShowMessageBox(this, text: ex.Message, title: "マイグレーション処理");
            return;
        }
        finally
        {
            this.IsRunning = false;
        }
    }

    [RelayCommand]
    private async Task RunAsync()
    {
        this.IsRunning = true;

        try
        {
            await _database.WriteAsync(this.TargetDataTable);
            _dialogService.ShowMessageBox(this, text: "マイグレーションに成功しました", title: "マイグレーション処理");
        }
        catch (ModelException ex)
        {
            _dialogService.ShowMessageBox(this, text: ex.Message, title: "マイグレーション処理");
            return;
        }
        finally
        {
            this.IsRunning = false;
        }

        this.DialogResult = true;
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Back()
    {
        this.Title = "マイグレーション - ファイル選択";
        this.FileVisibility = Visibility.Visible;
        this.RunVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void Cancel()
    {
        this.DialogResult = false;
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }
}
