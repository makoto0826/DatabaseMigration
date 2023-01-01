using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Models;
using FixedFileToSqlServerTool.ViewModels.Messages;
using LiteDB;

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

    public ObjectId Id { get; }

    private readonly MappingTableDefinition _table;

    public MappingTablePaneViewModel(MappingTableDefinition table)
    {
        _table = table;
        this.Name = _table.Name;
        this.Id = _table.Id;
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
