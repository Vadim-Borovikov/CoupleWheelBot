using CoupleWheelBot.ImageProcessing;
using GoogleSheetsManager.Documents;

namespace CoupleWheelBot;

internal sealed class FileManager
{
    private readonly Manager _documentsManager;
    private readonly IImageProcessor _imageProcessor;
    private readonly string _id;

    public FileManager(Bot bot, Manager documentsManager,
        IImageProcessor imageProcessor)
    {
        _documentsManager = documentsManager;
        _imageProcessor = imageProcessor;
        _id = bot.Config.GoogleSheetId;
    }

    public async Task<byte[]?> DownloadAsync()
    {
        using (MemoryStream stream = new())
        {
            await _documentsManager.DownloadAsync(_id, PdfMime, stream);
            byte[]? converted = _imageProcessor.ConvertToPng(stream.ToArray());
            byte[]? cropped = converted is null ? null : _imageProcessor.CropContent(converted);
            return cropped is null ? null : _imageProcessor.Pad(cropped, PadPixels, PadPixels, PadPixels,
                PadPixels + BottomExtraPadPixels);
        }
    }

    private const string PdfMime = "application/pdf";

    private const int PadPixels = 30;
    private const int BottomExtraPadPixels = 130;
}