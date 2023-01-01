using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FixedFileToSqlServerTool.ViewModels.Messages;

public class SavingScriptMessage : ValueChangedMessage<Models.Script>
{
    public SavingScriptMessage(Models.Script value) : base(value) { }
}
