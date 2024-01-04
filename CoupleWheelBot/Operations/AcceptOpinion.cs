using AbstractBot.Bots;
using AbstractBot.Operations;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Operations.Infos;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class AcceptOpinion : Operation<AcceptOpinionInfo>
{
    public AcceptOpinion(BotBasic bot, DialogManager manager) : base(bot) => _manager = manager;

    protected override bool IsInvokingBy(Message message, User sender, out AcceptOpinionInfo? info)
    {
        info = null;
        return false;
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore,
        out AcceptOpinionInfo? info)
    {
        Partner? context = Bot.TryGetContext<Partner>(sender.Id);
        info = string.IsNullOrWhiteSpace(context?.Name)
            ? null
            : AcceptOpinionInfo.From(context, callbackQueryDataCore);
        return info is not null;
    }

    protected override Task ExecuteAsync(AcceptOpinionInfo info, Message message, User sender)
    {
        bool accepted = _manager.AcceptOpinion(info.Context, info.Index, info.Opinion);
        return accepted ? _manager.NextStepAsync(message.Chat, info.Context) : Task.CompletedTask;
    }

    private readonly DialogManager _manager;
}