using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.ViewModels;

[INotifyPropertyChanged]
public partial class TableContentViewModel : IContentViewModel
{
    public Table Table { get; }

    [ObservableProperty]
    private bool _isActive;

    public TableContentViewModel(Table table) => this.Table = table;

    partial void OnIsActiveChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsActiveMessage(this));

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
