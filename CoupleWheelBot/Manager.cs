using CoupleWheelBot.ImageProcessing;
using Telegram.Bot.Types;

namespace CoupleWheelBot;

internal sealed class Manager
{
    private readonly Bot _bot;
    private readonly GoogleSheetsManager.Documents.Manager _documentsManager;
    private readonly IImageProcessor _imageProcessor;
    private readonly string _id;

    public Manager(Bot bot, GoogleSheetsManager.Documents.Manager documentsManager, IImageProcessor imageProcessor)
    {
        _bot = bot;
        _documentsManager = documentsManager;
        _imageProcessor = imageProcessor;
        _id = bot.Config.GoogleSheetId;
    }

    public async Task DownloadAsync(Chat chat)
    {
        using (MemoryStream stream = new())
        {
            await _documentsManager.DownloadAsync(_id, PdfMime, stream);
            byte[]? converted = _imageProcessor.ConvertToPng(stream.ToArray());
            byte[]? cropped = converted is null ? null : _imageProcessor.CropContent(converted);
            byte[]? padded =
                cropped is null ? null : _imageProcessor.Pad(cropped, Pad, Pad, Pad, Pad + BottomExtraPad);
            await SendPngAsync(chat, padded);
        }
    }

    private Task SendPngAsync(Chat chat, byte[]? png)
    {
        if (png is null)
        {
            return _bot.SendTextMessageAsync(chat, "Не вышло");
        }

        using (MemoryStream stream = new(png))
        {
            InputFile photo = InputFile.FromStream(stream);
            return _bot.SendPhotoAsync(chat, photo);
        }
    }

    private const string PdfMime = "application/pdf";
    private const int Pad = 30;
    private const int BottomExtraPad = 130;
}