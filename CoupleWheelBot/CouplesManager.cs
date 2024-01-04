using CoupleWheelBot.Contexts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CoupleWheelBot;

internal sealed class CouplesManager
{
    public CouplesManager(Dictionary<long, Partner> contexts, byte questionsAmount)
    {
        _contexts = contexts;
        _questionsAmount = questionsAmount;
    }

    public bool IsDone(Guid coupleId)
    {
        List<Partner> contexts = GetPairsWith(coupleId).Select(p => p.Value).ToList();
        return (contexts.Count == PartnersInCouple) && contexts.All(c => c.Opinions.Count == _questionsAmount);
    }

    public IEnumerable<Chat> GetChatsWith(Guid coupleId)
    {
        return GetPairsWith(coupleId).Select(p => GetPrivateChat(p.Key));
    }

    private IEnumerable<KeyValuePair<long, Partner>> GetPairsWith(Guid coupleId)
    {
         return _contexts.Where(p => p.Value.CoupleId == coupleId);
    }

    private static Chat GetPrivateChat(long userId)
    {
        return new Chat
        {
            Id = userId,
            Type = ChatType.Private
        };
    }

    public const byte PartnersInCouple = 2;

    private readonly Dictionary<long, Partner> _contexts;
    private readonly byte _questionsAmount;
}