using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using LiteDB;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class MappingTablePaneViewModel : IPaneViewModel
{
    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isDirty;

    public ObjectId Id { get; }

    private readonly MappingTableWidgetViewModel _mappingTableWidget;

    public MappingTablePaneViewModel(MappingTableWidgetViewModel mappingTableWidget)
    {
        _mappingTableWidget = mappingTableWidget;
        this.Title = mappingTableWidget.Table.Name;
        this.Id = mappingTableWidget.Table.Id;
    }

    partial void OnIsSelectedChanged(bool _) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
