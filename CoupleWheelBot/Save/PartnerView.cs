namespace CoupleWheelBot.Save;

public class PartnerOpinion
{
    public string UserName { get; set; }
    public List<byte?> Estimates { get; init; }

    public PartnerOpinion() { }

    public PartnerOpinion(string name, byte questionsNumber)
    {
        UserName = name;
        Estimates = Enumerable.Repeat((byte?) null, questionsNumber).ToList();
    }
}