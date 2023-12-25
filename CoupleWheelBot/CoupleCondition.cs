namespace CoupleWheelBot;

public class CoupleCondition
{
    public readonly Guid Id;

    public Dictionary<long, PartnerView> Views { get; init; }

    public CoupleCondition() { }

    public CoupleCondition(long userId, byte questionsNumber)
    {
        Id = Guid.NewGuid();
        Views = new Dictionary<long, PartnerView>
        {
            { userId, PartnerView.Create(questionsNumber) }
        };
    }
}
