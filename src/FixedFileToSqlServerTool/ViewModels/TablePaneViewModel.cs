using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class TablePaneViewModel : IPaneViewModel
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isDirty;

    [ObservableProperty]
    private TableDefinition table;

    public int Id { get; }

    private readonly TableWidgetViewModel _tableWidget;

    public TablePaneViewModel(TableWidgetViewModel tableWidget)
    {
        _tableWidget = tableWidget;
        this.Title = tableWidget.Table.Name;
        this.Id = tableWidget.Table.Id;
        this.table = tableWidget.Table;
    }

    partial void OnIsSelectedChanged(bool _) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
