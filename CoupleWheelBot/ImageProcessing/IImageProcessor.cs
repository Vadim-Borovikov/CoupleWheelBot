namespace CoupleWheelBot.ImageProcessing;

internal interface IImageProcessor
{
    byte[]? ConvertToPng(byte[] pdf);
    byte[]? CropContent(byte[] png);
    byte[]? Pad(byte[] png, ushort left, ushort right, ushort top, ushort bottom);
}
