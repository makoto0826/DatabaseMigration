using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class TableContentPaneViewModel : IPaneViewModel
{
    [ObservableProperty]
    private bool isSelected;

    public TableWidgetViewModel TableWidget { get; }

    public int Id { get; }

    public TableContentPaneViewModel(TableWidgetViewModel tableWidget)
    {
        this.Id = tableWidget.Table.Id;
        this.TableWidget = tableWidget;
    }

    partial void OnIsSelectedChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
