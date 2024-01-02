using CoupleWheelBot.Contexts;

namespace CoupleWheelBot.Operations.Infos;

internal sealed class ContinueTestInfo
{
    public readonly Partner Context;

    public ContinueTestInfo(Partner context) => Context = context;
}