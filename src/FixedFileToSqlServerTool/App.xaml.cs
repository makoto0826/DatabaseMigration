using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FixedFileToSqlServerTool.Models;
using FixedFileToSqlServerTool.ViewModels;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FixedFileToSqlServerTool;

public partial class App : Application
{
    public App()
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("LiteDBConnectionString");

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService>(
                    new DialogService(
                        new DialogManager(viewLocator: new ViewLocator()),
                            viewModelFactory: x => Ioc.Default.GetService(x)))
                .AddSingleton(x => new LiteDatabase(connectionString))
                .AddSingleton(x => new ScriptRepository(x.GetRequiredService<LiteDatabase>()))
                .AddSingleton(x => new DatabaseSettingRepository(x.GetRequiredService<LiteDatabase>()))
                .AddSingleton(x => new TableRepository(x.GetRequiredService<LiteDatabase>()))
                .AddSingleton(x => new MappingTableRepository(x.GetRequiredService<LiteDatabase>()))
                .AddSingleton(x => new ScriptRunner())
                .AddTransient<DatabaseSettingDialogViewModel>()
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider()
        ); ;
    }
}
