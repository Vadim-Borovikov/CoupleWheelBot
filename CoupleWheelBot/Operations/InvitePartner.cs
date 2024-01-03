using AbstractBot;
using AbstractBot.Configs;
using GryphonUtilities.Extensions;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class InvitePartner : OperationShare
{
    public InvitePartner(Bot bot, KeyboardProvider replyKeyboardProvider)
        : base(bot, bot.Config.Texts.InviteReply, replyKeyboardProvider) => _bot = bot;

    protected override bool IsInvokingBy(Message message, User sender, out MessageTemplate? data)
    {
        data = null;
        return false;
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore,
        out MessageTemplate? data)
    {
        string link = string.Format(DialogManager.LinkFormat, (_bot.User?.Username).Denull(), callbackQueryDataCore);
        data = _bot.Config.Texts.InviteMessageFormat.Format(link);
        return true;
    }

    private readonly Bot _bot;
}