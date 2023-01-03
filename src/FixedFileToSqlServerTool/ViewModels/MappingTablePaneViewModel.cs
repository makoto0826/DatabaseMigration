using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FixedFileToSqlServerTool.Messaging.Messages;
using FixedFileToSqlServerTool.Models;
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

    private readonly MappingTableDefinitionRepository _mappingTableDefinitionRepository;

    private readonly TableDefinitionRepository _tableDefinitionRepository;

    private readonly ScriptRepository _scriptRepository;

    public MappingTablePaneViewModel(
        MappingTableWidgetViewModel mappingTableWidget,
        MappingTableDefinitionRepository mappingTableDefinitionRepository,
        TableDefinitionRepository tableDefinitionRepository,
        ScriptRepository scriptRepository
    )
    {
        _mappingTableWidget = mappingTableWidget;
        _mappingTableDefinitionRepository = mappingTableDefinitionRepository;
        _tableDefinitionRepository = tableDefinitionRepository;
        _scriptRepository = scriptRepository;

        this.Title = mappingTableWidget.Table.Name;
        this.Id = mappingTableWidget.Table.Id;
    }

    partial void OnIsSelectedChanged(bool value) => WeakReferenceMessenger.Default.Send(new ChangedIsSelectedPaneMessage(this));

    [RelayCommand]
    private void Save()
    {
        var newMappingTable = _mappingTableWidget.Table with
        {

        };

        _mappingTableDefinitionRepository.Save(newMappingTable);
        WeakReferenceMessenger.Default.Send(new MappingTableDefinitionMessage(newMappingTable));
    }

    [RelayCommand]
    private void Close() => WeakReferenceMessenger.Default.Send(new ClosedPaneMessage(this));
}
