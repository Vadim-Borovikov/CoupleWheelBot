using JetBrains.Annotations;

namespace CoupleWheelBot.Save;

public sealed class PartnerOpinion
{
    [UsedImplicitly]
    public string UserName { get; set; }
    [UsedImplicitly]
    public List<byte?> Estimates { get; init; }

    internal int NextIndex => Estimates.IndexOf(null);

    internal bool Done => Estimates.All(e => e.HasValue);

    public PartnerOpinion(string name, byte questionsNumber)
    {
        UserName = name;
        Estimates = Enumerable.Repeat((byte?) null, questionsNumber).ToList();
    }
}