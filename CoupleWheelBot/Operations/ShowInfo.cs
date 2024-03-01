using AbstractBot.Operations;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class ShowInfo : OperationSimple
{
    public ShowInfo(Bot bot, DialogManager dialogManager) : base(bot) => _dialogManager = dialogManager;

    protected override bool IsInvokingBy(Message message, User sender) => false;
    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore) => true;

    protected override Task ExecuteAsync(Message message, User sender) => _dialogManager.ShowInfo(message.Chat);

    private readonly DialogManager _dialogManager;
}