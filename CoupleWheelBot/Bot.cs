using AbstractBot.Bots;
using AbstractBot.Configs;
using CoupleWheelBot.Configs;
using CoupleWheelBot.ImageProcessing;
using CoupleWheelBot.Operations.Commands;
using CoupleWheelBot.Operations.Infos;
using CoupleWheelBot.Save;

namespace CoupleWheelBot;

public sealed class Bot : BotWithSheets<Config, Texts, Data, StartData>
{
    [Flags]
    internal enum AccessType
    {
        Default = 1,
        Config = 2,

        Admin = Default | Config // 3
    }

    public Bot(Config config) : base(config)
    {
        IImageProcessor imageProcessor = new ImageProcessor();
        Manager manager = new(this, DocumentsManager, imageProcessor);
        Operations.Add(new DownloadCommand(this, manager));
    }
}