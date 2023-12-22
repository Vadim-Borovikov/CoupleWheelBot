using AbstractBot.Bots;
using AbstractBot.Configs;
using CoupleWheelBot.Configs;
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

    public Bot(Config config) : base(config) { }
}