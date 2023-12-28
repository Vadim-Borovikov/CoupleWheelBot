using CoupleWheelBot.Contexts;

namespace CoupleWheelBot.Operations.Infos;

internal sealed class AcceptNameInfo
{
    public readonly Partner Context;
    public readonly string Text;

    public AcceptNameInfo(Partner context, string text)
    {
        Context = context;
        Text = text;
    }
}