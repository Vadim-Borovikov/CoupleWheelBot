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
    public List<byte?> Opinions { get; init; } = null!;

    internal int NextIndex => Opinions.IndexOf(null);

    internal bool Done => Opinions.All(e => e.HasValue);

    [UsedImplicitly]
    public Partner() { }

    public Partner(string name, byte questionsNumber)
    {
        UserName = name;
        Opinions = Enumerable.Repeat((byte?)null, questionsNumber).ToList();
    }
}