using CoupleWheelBot.ImageProcessing;

namespace CoupleWheelBot;

internal sealed class FileManager
{
    private readonly GoogleSheetsManager.Documents.Manager _documentsManager;
    private readonly IImageProcessor _imageProcessor;
    private readonly string _id;

    public FileManager(Bot bot, GoogleSheetsManager.Documents.Manager documentsManager,
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
            return cropped is null ? null : _imageProcessor.Pad(cropped, Pad, Pad, Pad, Pad + BottomExtraPad);
        }
    }

    private const string PdfMime = "application/pdf";
    private const int Pad = 30;
    private const int BottomExtraPad = 130;
}