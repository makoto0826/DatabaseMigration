using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class ChangedIsAcitvePaneMessage : ValueChangedMessage<IPaneViewModel>
{
    public ChangedIsAcitvePaneMessage(IPaneViewModel value) : base(value) { }
}
