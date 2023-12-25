namespace CoupleWheelBot;

public class PartnerView
{
    public string UserName { get; set; }
    public List<byte?> Estimates { get; init; }

    public static PartnerView Create(byte questionsNumber)
    {
        return new PartnerView
        {
            Estimates = Enumerable.Repeat((byte?) null, questionsNumber).ToList()
        };
    }
}