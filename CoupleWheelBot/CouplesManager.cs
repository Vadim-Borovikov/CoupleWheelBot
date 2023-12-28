using CoupleWheelBot.Contexts;

namespace CoupleWheelBot;

internal sealed class CouplesManager
{
    public CouplesManager(Dictionary<long, Partner> contexts) => _contexts = contexts;

    public IEnumerable<long> GetUserIdsWith(Guid coupleId)
    {
        return _contexts.Where(p => p.Value.CoupleId == coupleId).Select(p => p.Key);
    }

    public bool IsDone(Guid coupleId)
    {
        List<Partner> contexts = _contexts.Values.Where(c => c.CoupleId == coupleId).ToList();
        return (contexts.Count == PartnersInCouple) && contexts.All(c => c.Done);
    }

    private const byte PartnersInCouple = 2;
    private readonly Dictionary<long, Partner> _contexts;
}