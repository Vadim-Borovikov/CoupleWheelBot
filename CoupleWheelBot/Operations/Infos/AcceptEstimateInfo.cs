namespace CoupleWheelBot.Operations.Infos;

internal sealed class AcceptEstimateInfo
{
    public readonly Contexts.Answer Context;
    public readonly byte Index;
    public readonly byte Estimate;

    public AcceptEstimateInfo(Contexts.Answer context, byte index, byte estimate)
    {
        Context = context;
        Index = index;
        Estimate = estimate;
    }

    public static AcceptEstimateInfo? From(Contexts.Answer? context, string value)
    {
        if (context is null)
        {
            return null;
        }

        string[] parts = value.Split(Bot.QuerySeparator);
        if (parts.Length != 2)
        {
            return null;
        }

        if (!byte.TryParse(parts[0], out byte index))
        {
            return null;
        }

        if (!byte.TryParse(parts[1], out byte estimate))
        {
            return null;
        }

        return new AcceptEstimateInfo(context, index, estimate);
    }
}