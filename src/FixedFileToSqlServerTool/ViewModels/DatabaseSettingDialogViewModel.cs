using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FixedFileToSqlServerTool.Models;
using HanumanInstitute.MvvmDialogs;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class DatabaseSettingDialogViewModel : ObservableObject, IModalDialogViewModel, ICloseable
{
    public event EventHandler? RequestClose;

    public bool? DialogResult => _dialogResult;

    private bool? _dialogResult;

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
    private async Task Test()
    {
        try
        {
            this.IsRunning = true;
            using var connection = this.CreateDatabaseSetting().CreateConnection();
            await connection.OpenAsync();

            _dialogService.ShowMessageBox(this, text: "接続のテストに成功しました", title: "接続テスト成功");
        }
        catch (Exception ex)
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
        _dialogResult = true;
        _databaseSettingRepository.Save(this.CreateDatabaseSetting());
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Cancel()
    {
        _dialogResult = false;
        this.RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private DatabaseSetting CreateDatabaseSetting() =>
        new DatabaseSetting
        {
            Server = this.Server,
            Port = this.Port,
            UserId = this.UserId,
            Password = this.Password,
            Database = this.Database,
        };
}
