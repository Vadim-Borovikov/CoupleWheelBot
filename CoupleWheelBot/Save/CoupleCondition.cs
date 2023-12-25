using JetBrains.Annotations;

namespace CoupleWheelBot.Save;

public sealed class CoupleCondition
{
    [UsedImplicitly]
    public Dictionary<long, PartnerOpinion> Opinions { get; init; } = new();

    internal bool Done => (Opinions.Count == Amount) && Opinions.Values.All(o => o.Done);

    private const byte Amount = 2;
}
