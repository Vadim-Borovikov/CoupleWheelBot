using AbstractBot.Bots;
using AbstractBot.Operations;
using CoupleWheelBot.Operations.Infos;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class AcceptEstimate : Operation<AcceptEstimateInfo>
{
    public AcceptEstimate(BotBasic bot, DialogManager manager) : base(bot) => _manager = manager;

    protected override bool IsInvokingBy(Message message, User sender, out AcceptEstimateInfo? info)
    {
        info = null;
        return false;
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore,
        out AcceptEstimateInfo? info)
    {
        Contexts.Answer? context = Bot.TryGetContext<Contexts.Answer>(sender.Id);
        info = AcceptEstimateInfo.From(context, callbackQueryDataCore);
        return info is not null;
    }

    protected override Task ExecuteAsync(AcceptEstimateInfo info, Message message, User sender)
    {
        _manager.AcceptEstimate(sender.Id, info.Context, info.Index, info.Estimate);
        return _manager.NextStepAsync(message.Chat, sender.Id, info.Context);
    }

    private readonly DialogManager _manager;
}