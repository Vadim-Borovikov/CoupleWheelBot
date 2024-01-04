using AbstractBot;
using JetBrains.Annotations;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Contexts;

public sealed class Partner : Context
{
    [UsedImplicitly]
    public string Name { get; set; } = null!;
    [UsedImplicitly]
    public string? Username { get; set; }
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
        Name = name;
        Opinions = new List<byte>();
    }
}