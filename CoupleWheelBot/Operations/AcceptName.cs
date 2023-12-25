using AbstractBot.Bots;
using AbstractBot.Operations;
using CoupleWheelBot.Operations.Infos;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CoupleWheelBot.Operations;

internal sealed class AcceptName : Operation<AcceptNameInfo>
{
    public AcceptName(BotBasic bot, DialogManager manager) : base(bot) => _manager = manager;

    protected override bool IsInvokingBy(Message message, User sender, out AcceptNameInfo? info)
    {
        info = null;

        Contexts.Answer? context = Bot.TryGetContext<Contexts.Answer>(sender.Id);
        if (context is null)
        {
            return false;
        }

        if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message.Text))
        {
            return false;
        }

        if (message.Text.StartsWith('/'))
        {
            return false;
        }

        info = new AcceptNameInfo(context, message.Text);
        return true;
    }

    protected override Task ExecuteAsync(AcceptNameInfo info, Message message, User sender)
    {
        _manager.AcceptName(sender.Id, info.Context, info.Text);
        return _manager.NextStepAsync(message.Chat, sender.Id, info.Context);
    }

    private readonly DialogManager _manager;
}