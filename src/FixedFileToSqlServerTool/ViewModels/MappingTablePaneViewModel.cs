using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Models;
using FixedFileToSqlServerTool.ViewModels.Messages;

namespace FixedFileToSqlServerTool.ViewModels;

public partial class MappingTablePaneViewModel : ObservableObject, IPaneViewModel
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private bool isActive;

    [ObservableProperty]
    private bool isDirty;

    public string ContentId => nameof(MappingTablePaneViewModel);

    public MappingTable Table { get; }

    public MappingTablePaneViewModel(MappingTable table)
    {
        this.Table = table;
        this.Name = this.Table.Name;
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
