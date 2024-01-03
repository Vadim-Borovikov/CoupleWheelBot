using AbstractBot;
using AbstractBot.Configs;
using AbstractBot.Operations;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations;

internal abstract class OperationShare : Operation<MessageTemplate>
{
    protected OperationShare(Bot bot, MessageTemplate messageTemplateReply,
        KeyboardProvider? keyboardProviderReply = null)
        : base(bot)
    {
        _messageTemplateReply = messageTemplateReply;
        _keyboardProviderReply = keyboardProviderReply;
    }

    protected override async Task ExecuteAsync(MessageTemplate data, Message message, User sender)
    {
        Message messageToShare = await data.SendAsync(Bot, message.Chat);
        await _messageTemplateReply.SendAsync(Bot, message.Chat, _keyboardProviderReply,
            replyToMessageId: messageToShare.MessageId);
    }

    private readonly MessageTemplate _messageTemplateReply;
    private readonly KeyboardProvider? _keyboardProviderReply;
}