using CoupleWheelBot.Contexts;

namespace CoupleWheelBot.Operations.Infos;

internal sealed class AcceptOpinionInfo
{
    public readonly Partner Context;
    public readonly byte Index;
    public readonly byte Opinion;

    private AcceptOpinionInfo(Partner context, byte index, byte opinion)
    {
        Context = context;
        Index = index;
        Opinion = opinion;
    }

    public static AcceptOpinionInfo? From(Partner context, string value)
    {
        string[] parts = value.Split(Bot.QuerySeparator);
        if (parts.Length != 2)
        {
            return null;
        }

        if (!byte.TryParse(parts[0], out byte index))
        {
            return null;
        }

        if (!byte.TryParse(parts[1], out byte opinion))
        {
            return null;
        }

        return new AcceptOpinionInfo(context, index, opinion);
    }
}