using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class MappingTableDefinitionMessage : ValueChangedMessage<MappingTable>
{
    public MappingTableDefinitionMessage(MappingTable value) : base(value) { }
}
