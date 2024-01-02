using AbstractBot;
using AbstractBot.Configs;
using AbstractBot.Operations;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal sealed class DescribeTest : OperationSimple
{
    public DescribeTest(Bot bot) : base(bot)
    {
        _messageTemplate = bot.Config.Texts.Describe;
        _keyboardProvider = CoupleWheelBot.Bot.CreateSimpleKeyboard<StartTest>(bot.Config.Texts.InviteButton);
    }

    protected override bool IsInvokingBy(Message message, User sender) => false;

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore) => true;

    protected override Task ExecuteAsync(Message message, User sender)
    {
        return _messageTemplate.SendAsync(Bot, message.Chat, _keyboardProvider);
    }

    private readonly MessageTemplate _messageTemplate;
    private readonly KeyboardProvider _keyboardProvider;
}