using JetBrains.Annotations;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Save;

public sealed class PartnerOpinion
{
    [UsedImplicitly]
    public string UserName { get; set; } = null!;
    [UsedImplicitly]
    public List<byte?> Estimates { get; init; } = null!;

    internal int NextIndex => Estimates.IndexOf(null);

    internal bool Done => Estimates.All(e => e.HasValue);

    [UsedImplicitly]
    public PartnerOpinion() { }


    public PartnerOpinion(string name, byte questionsNumber)
    {
        UserName = name;
        Estimates = Enumerable.Repeat((byte?) null, questionsNumber).ToList();
    }
}