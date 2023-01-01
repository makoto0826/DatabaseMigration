using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<MainWindowViewModel>();
    }
}
