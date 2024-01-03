using System.Globalization;
using CoupleWheelBot.ImageProcessing;
using QuickChart;

namespace CoupleWheelBot;

internal sealed class ChartProvider
{
    public ChartProvider(string configTemplate, IImageProcessor imageProcessor)
    {
        _configTemplate = configTemplate;
        _imageProcessor = imageProcessor;
        _chart = new Chart
        {
            Width = Width,
            Height = Height
        };
    }

    public byte[]? GetChart(IEnumerable<decimal> data, IEnumerable<IEnumerable<string>> labels)
    {
        string allData = JoinWithQuotes(data.Select(d => d.ToString(CultureInfo.InvariantCulture)));
        string allLabels = string.Join(',', labels.Select(l => $"[{JoinWithQuotes(l)}]"));

        _chart.Config = string.Format(_configTemplate, allData, allLabels);

        byte[]? generated = _chart.ToByteArray();
        return generated is null ? null : _imageProcessor.Pad(generated, Pad, Pad, Pad, Pad);
    }

    private static string JoinWithQuotes(IEnumerable<string> values) => string.Join(',', values.Select(s => $"'{s}'"));

    private readonly Chart _chart;
    private readonly string _configTemplate;
    private readonly IImageProcessor _imageProcessor;

    private const int Width = 1000;
    private const int Height = 600;
    private const int Pad = 30;
}