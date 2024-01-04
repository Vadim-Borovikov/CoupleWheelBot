using CoupleWheelBot.Contexts;

namespace CoupleWheelBot.Operations.Infos;

internal sealed class AcceptNameInfo
{
    public readonly Partner Context;
    public readonly string Name;
    public readonly string? Username;

    public AcceptNameInfo(Partner context, string name, string? username)
    {
        Context = context;
        Name = name;
        Username = username;
    }
}