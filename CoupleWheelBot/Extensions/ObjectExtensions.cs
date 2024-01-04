using CoupleWheelBot.Records;
using GoogleSheetsManager.Extensions;
using GryphonUtilities.Extensions;

namespace CoupleWheelBot.Extensions;

internal static class ObjectExtensions
{
    public static List<byte>? ToBytes(this object? o)
    {
        if (o is IEnumerable<byte> l)
        {
            return l.ToList();
        }
        return o?.ToString()?.Split(Record.ElementsSeparator).Select(s => s.ToByte()).RemoveNulls().ToList();
    }

    public static List<decimal>? ToDecimals(this object? o)
    {
        if (o is IEnumerable<decimal> l)
        {
            return l.ToList();
        }
        return o?.ToString()?.Split(Record.ElementsSeparator).Select(s => s.ToDecimal()).RemoveNulls().ToList();
    }
}