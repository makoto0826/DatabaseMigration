using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FixedFileToSqlServerTool.Messaging.Messages;

public class SavedScriptMessage : ValueChangedMessage<Models.Script>
{
    public SavedScriptMessage(Models.Script value) : base(value) { }
}
