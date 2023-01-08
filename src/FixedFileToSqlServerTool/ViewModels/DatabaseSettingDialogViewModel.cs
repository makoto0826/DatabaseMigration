using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class DatabaseSettingDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public event EventHandler? RequestClose;

    public bool? DialogResult { get; private set; }

    [ObservableProperty]
    private string server;

    [ObservableProperty]
    private int? port;

    [ObservableProperty]
    private string userId;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string database;

    [ObservableProperty]
    private bool isRunning;

    private readonly DatabaseSettingRepository _databaseSettingRepository;

    private readonly IDialogService _dialogService;

    public DatabaseSettingDialogViewModel(DatabaseSettingRepository databaseSettingRepository, IDialogService dialogService)
    {
        _databaseSettingRepository = databaseSettingRepository;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private void Loaded()
    {
        var setting = _databaseSettingRepository.Get();

        if (setting is not null)
        {
            this.Server = setting.Server;
            this.Port = setting.Port;
            this.UserId = setting.UserId;
            this.Password = setting.Password;
            this.Database = setting.Database;
        }
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        try
        {
            this.IsRunning = true;
            var database = new Database(this.ToDatabaseSetting());
            await database.ConnectTestAsync();

            _dialogService.ShowMessageBox(this, text: "接続のテストに成功しました", title: "接続テスト成功");
        }
        catch (ModelException ex)
        {
            _dialogService.ShowMessageBox(this, text: ex.Message, title: "接続テスト失敗");
        }
        finally
        {
            this.IsRunning = false;
        }
    }

    [RelayCommand]
    private void Ok()
    {
        this.DialogResult = true;
        _databaseSettingRepository.Save(this.ToDatabaseSetting());
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        this.DialogResult = false;
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private DatabaseSetting ToDatabaseSetting() =>
        new DatabaseSetting
        {
            Server = this.Server,
            Port = this.Port,
            UserId = this.UserId,
            Password = this.Password,
            Database = this.Database,
        };
}
