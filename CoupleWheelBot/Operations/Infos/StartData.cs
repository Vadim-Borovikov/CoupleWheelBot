using AbstractBot.Operations.Data;

namespace CoupleWheelBot.Operations.Infos;

public sealed class StartData : ICommandData<StartData>
{
    internal readonly Guid CoupleId;

    private StartData(Guid coupleId) => CoupleId = coupleId;

    public static StartData? From(string[] parameters)
    {
        return parameters.Length switch
        {
            1 => new StartData(Guid.Parse(parameters.Single())),
            _ => null
        };
    }
}