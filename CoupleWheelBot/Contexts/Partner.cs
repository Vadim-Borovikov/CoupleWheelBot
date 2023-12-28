using AbstractBot;
using JetBrains.Annotations;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Contexts;

public sealed class Partner : Context
{
    [UsedImplicitly]
    public string UserName { get; set; } = null!;
    [UsedImplicitly]
    public bool IsInitiator { get; set; }
    [UsedImplicitly]
    public Guid CoupleId { get; init; }
    [UsedImplicitly]
    public List<byte> Opinions { get; init; } = null!;

    [UsedImplicitly]
    public Partner() { }

    public Partner(string name)
    {
        UserName = name;
        Opinions = new List<byte>();
    }
}