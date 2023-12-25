namespace CoupleWheelBot.Operations.Infos;

internal sealed class AcceptNameInfo
{
    public readonly Contexts.Answer Context;
    public readonly string Text;

    public AcceptNameInfo(Contexts.Answer context, string text)
    {
        Context = context;
        Text = text;
    }
}