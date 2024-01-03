using System.Diagnostics.CodeAnalysis;
using PDFtoImage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace CoupleWheelBot.ImageProcessing;

internal sealed class ImageProcessor : IImageProcessor
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public byte[] ConvertToPng(byte[] pdf)
    {
        using (MemoryStream stream = new())
        {
            Conversion.SavePng(stream, pdf);
            return stream.ToArray();
        }
    }

    public byte[] CropContent(byte[] png)
    {
        using (Image image = Image.Load(png))
        {
            image.Mutate(i => i.EntropyCrop());

            using (MemoryStream stream = new())
            {
                image.SaveAsPng(stream);
                return stream.ToArray();
            }
        }
    }

    public byte[] Pad(byte[] png, ushort left, ushort right, ushort top, ushort bottom)
    {
        using (Image content = Image.Load(png))
        {
            // Create an image with white background
            Image<Rgb24> image = new(content.Width + left + right, content.Height + top + bottom, Color.White);
            Point topLeft = new(left, top);
            // ReSharper disable once AccessToDisposedClosure
            image.Mutate(i => i.DrawImage(content, topLeft, 1));

            using (MemoryStream stream = new())
            {
                image.SaveAsPng(stream);
                return stream.ToArray();
            }
        }
    }
}