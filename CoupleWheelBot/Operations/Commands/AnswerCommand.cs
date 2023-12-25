using AbstractBot.Operations.Commands;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations.Commands;

internal sealed class AnswerCommand : CommandSimple
{
    public AnswerCommand(Bot bot, DialogManager manager)
        : base(bot, "answer", bot.Config.Texts.AnswerCommandDescription)
    {
        _manager = manager;
    }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        return _manager.NextStepAsync(message.Chat, sender.Id);
    }

    private readonly DialogManager _manager;
}