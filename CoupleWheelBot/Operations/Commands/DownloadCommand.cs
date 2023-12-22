using AbstractBot.Bots;
using AbstractBot.Operations.Commands;
using Telegram.Bot.Types;

namespace CoupleWheelBot.Operations.Commands;

internal sealed class DownloadCommand : CommandSimple
{
    public DownloadCommand(BotBasic bot, Manager manager) : base(bot, "download", "скачать") => _manager = manager;

    protected override Task ExecuteAsync(Message message, User sender) => _manager.DownloadAsync(message.Chat);

    private readonly Manager _manager;
}