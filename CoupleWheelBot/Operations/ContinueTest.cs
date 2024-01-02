using AbstractBot.Operations;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Operations.Infos;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class ContinueTest : Operation<ContinueTestInfo>
{
    public ContinueTest(Bot bot, DialogManager dialogManager) : base(bot) => _dialogManager = dialogManager;

    protected override bool IsInvokingBy(Message message, User sender, out ContinueTestInfo? info)
    {
        info = null;
        return false;
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore,
        out ContinueTestInfo? info)
    {
        Partner? context = Bot.TryGetContext<Partner>(sender.Id);
        info = context is null ? null : new ContinueTestInfo(context);
        return info is not null;
    }

    protected override Task ExecuteAsync(ContinueTestInfo data, Message message, User sender)
    {
        return _dialogManager.NextStepAsync(message.Chat, data.Context);
    }

    private readonly DialogManager _dialogManager;
}