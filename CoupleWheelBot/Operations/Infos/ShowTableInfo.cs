namespace CoupleWheelBot.Operations.Infos;

internal sealed class ShowTableInfo
{
    public readonly Guid CoupleId;

    public ShowTableInfo(Guid coupleId) => CoupleId = coupleId;
}