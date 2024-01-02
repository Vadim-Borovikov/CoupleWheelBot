using AbstractBot.Operations;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Operations.Infos;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class ShowTable : Operation<ShowTableInfo>
{
    public ShowTable(Bot bot) : base(bot) => _bot = bot;

    protected override bool IsInvokingBy(Message message, User sender, out ShowTableInfo? info)
    {
        info = null;
        return false;
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore,
        out ShowTableInfo? info)
    {
        Partner? context = Bot.TryGetContext<Partner>(sender.Id);
        info = context is null ? null : new ShowTableInfo(context.CoupleId);
        return info is not null;
    }

    protected override Task ExecuteAsync(ShowTableInfo data, Message message, User sender)
    {
        return _bot.ShowTableAsync(data.CoupleId, message.Chat);
    }

    private readonly Bot _bot;
}