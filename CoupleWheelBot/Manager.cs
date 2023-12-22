using ceTe.DynamicPDF.Rasterizer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Telegram.Bot.Types;

namespace CoupleWheelBot;

internal sealed class Manager
{
    private readonly Bot _bot;
    private readonly GoogleSheetsManager.Documents.Manager _documentsManager;
    private readonly string _id;

    public Manager(Bot bot, GoogleSheetsManager.Documents.Manager documentsManager)
    {
        _bot = bot;
        _documentsManager = documentsManager;
        _id = bot.Config.GoogleSheetId;
    }

    public async Task DownloadAsync(Chat chat)
    {
        byte[] pdfData = await GetPdfDataAsync();
        byte[]? pngData = GetPngData(pdfData);
        await SendPngAsync(chat, pngData);
    }

    private async Task<byte[]> GetPdfDataAsync()
    {
        using (MemoryStream stream = new())
        {
            await _documentsManager.DownloadAsync(_id, PdfMime, stream);
            return stream.ToArray();
        }
    }

    private static byte[]? GetPngData(byte[] pdfData)
    {
        byte[]? converted = Convert(pdfData);
        return converted is null ? null : CropPng(converted);
    }

    private static byte[]? Convert(byte[] pdfData)
    {
        using (InputPdf pdf = new(pdfData))
        {
            using (PdfRasterizer rasterizer = new(pdf))
            {
                byte[][]? table = rasterizer.Draw(ImageFormat.Png, ImageSize.Dpi96);
                return table is null || (table.Length == 0)? null : table[0];
            }
        }
    }

    private static byte[] CropPng(byte[] pngData)
    {
        using (Image image = Image.Load(pngData))
        {
            image.Mutate(i => i.Crop(ContentRectangle)
                               .EntropyCrop()
                               .Pad(i.GetCurrentSize().Width + Pad, i.GetCurrentSize().Height + Pad,
                                   SixLabors.ImageSharp.Color.White));

            Size bottomPadding = new(0, BottomExtraPad);
            ResizeOptions options = new()
            {
                PadColor = SixLabors.ImageSharp.Color.White,
                Mode = ResizeMode.BoxPad,
                Size = Size.Add(image.Size, bottomPadding),
                Position = AnchorPositionMode.Top
            };

            image.Mutate(i => i.Resize(options));

            using (MemoryStream memoryStream = new())
            {
                image.SaveAsPng(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    private async Task SendPngAsync(Chat chat, byte[]? pngData)
    {
        if (pngData is null)
        {
            await _bot.SendTextMessageAsync(chat, "Не вышло");
            return;
        }

        using (MemoryStream stream = new(pngData))
        {
            InputFile photo = InputFile.FromStream(stream);
            await _bot.SendPhotoAsync(chat, photo);
        }
    }

    private const string PdfMime = "application/pdf";
    private static readonly Rectangle ContentRectangle = new(0, 60, 600, 220);
    private const int Pad = 20;
    private const int BottomExtraPad = 40;
}