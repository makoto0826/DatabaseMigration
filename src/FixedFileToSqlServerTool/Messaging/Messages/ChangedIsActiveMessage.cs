using CommunityToolkit.Mvvm.Messaging.Messages;
using FixedFileToSqlServerTool.ViewModels;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class ChangedIsActiveMessage : ValueChangedMessage<IContentViewModel>
{
    public ChangedIsActiveMessage(IContentViewModel value) : base(value) { }
}
