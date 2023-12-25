namespace CoupleWheelBot.Save;

public class PartnerOpinion
{
    public string UserName { get; set; }
    public List<byte?> Estimates { get; init; }

    public int NextIndex => Estimates.IndexOf(null);

    public bool Done => Estimates.All(e => e.HasValue);

    public PartnerOpinion() { }

    public PartnerOpinion(string name, byte questionsNumber)
    {
        UserName = name;
        Estimates = Enumerable.Repeat((byte?) null, questionsNumber).ToList();
    }
}