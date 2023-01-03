using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.Models;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class MappingTableDefinitionMessage : ValueChangedMessage<MappingTableDefinition>
{
    public MappingTableDefinitionMessage(MappingTableDefinition value) : base(value) { }
}
