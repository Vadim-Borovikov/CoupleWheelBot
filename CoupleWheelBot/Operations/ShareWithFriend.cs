using AbstractBot.Configs;
using GryphonUtilities.Extensions;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class ShareWithFriend : OperationShare
{
    public ShareWithFriend(Bot bot) : base(bot, bot.Config.Texts.FinaLShareReply) => _bot = bot;

    protected override bool IsInvokingBy(Message message, User sender, out MessageTemplate? data)
    {
        data = null;
        return false;
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore,
        out MessageTemplate? data)
    {
        data = _bot.Config.Texts.FinaLShareMessageFormat.Format((_bot.User?.Username).Denull());
        return true;
    }

    private readonly Bot _bot;
}