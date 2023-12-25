namespace CoupleWheelBot.Save;

public class CoupleCondition
{
    public Dictionary<long, PartnerOpinion> Opinions { get; init; } = new();

    public bool Done => (Opinions.Count == Amount) && Opinions.Values.All(o => o.Done);

    private const byte Amount = 2;
}
