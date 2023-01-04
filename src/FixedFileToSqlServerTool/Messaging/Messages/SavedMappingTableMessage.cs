using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class SavedMappingTableMessage : ValueChangedMessage<MappingTable>
{
    public SavedMappingTableMessage(MappingTable value) : base(value) { }
}
