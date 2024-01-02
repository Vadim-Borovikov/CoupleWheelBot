using AbstractBot;
using CoupleWheelBot.Contexts;
using JetBrains.Annotations;

namespace CoupleWheelBot.Save;

public sealed class Data : SaveData
{
    [UsedImplicitly]
    public Dictionary<long, Partner> PartnerContexts { get; set; } = new();

    internal IEnumerable<Partner> GetContextsWith(Guid guid)
    {
        return PartnerContexts.Values.Where(c => c.CoupleId == guid).OrderByDescending(c => c.IsInitiator);
    }
}