using AbstractBot.Operations;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class StartTest : OperationSimple
{
    public StartTest(Bot bot, DialogManager dialogManager) : base(bot) => _dialogManager = dialogManager;

    protected override bool IsInvokingBy(Message message, User sender) => false;

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore) => true;

    protected override Task ExecuteAsync(Message message, User sender)
    {
        return _dialogManager.StartTestAsync(message.Chat, sender.Id);
    }

    private readonly DialogManager _dialogManager;
}