using ceTe.DynamicPDF.Rasterizer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace CoupleWheelBot.ImageProcessing;

internal sealed class ImageProcessor : IImageProcessor
{
    public byte[]? ConvertToPng(byte[] pdf)
    {
        using (InputPdf inputPdf = new(pdf))
        {
            using (PdfRasterizer rasterizer = new(inputPdf))
            {
                byte[][]? table = rasterizer.Draw(ImageFormat.Png, ImageSize);
                return table is null || (table.Length == 0) ? null : table[0];
            }
        }
    }

    public byte[] CropContent(byte[] png)
    {
        using (Image image = Image.Load(png))
        {
            image.Mutate(i => i.Crop(ContentRectangle)
                               .EntropyCrop());

            using (MemoryStream memoryStream = new())
            {
                image.SaveAsPng(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    public byte[] Pad(byte[] png, ushort left, ushort right, ushort top, ushort bottom)
    {
        using (Image content = Image.Load(png))
        {
            // Create an image with white background
            Image<Rgb24> image =
                new(content.Width + left + right, content.Height + top + bottom, SixLabors.ImageSharp.Color.White);
            Point topLeft = new(left, top);
            // ReSharper disable once AccessToDisposedClosure
            image.Mutate(i => i.DrawImage(content, topLeft, 1));

            using (MemoryStream memoryStream = new())
            {
                image.SaveAsPng(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    private static readonly ImageSize ImageSize = ImageSize.Dpi96;
    private static readonly Rectangle ContentRectangle = new(0, 60, 600, 220);
}