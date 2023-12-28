using System.Globalization;
using QuickChart;

namespace CoupleWheelBot;

internal sealed class ChartProvider
{
    public ChartProvider(string configTemplate)
    {
        _configTemplate = configTemplate;
        _chart = new Chart();
    }

    public byte[] GetChart(IEnumerable<decimal> data, IEnumerable<string> labels)
    {
        string allData = JoinWithQuotes(data.Select(d => d.ToString(CultureInfo.InvariantCulture)));
        string allLabels = JoinWithQuotes(labels.Select(l => l.Substring(0, 3)));

        _chart.Config = string.Format(_configTemplate, allData, allLabels);

        return _chart.ToByteArray();
    }

    private static string JoinWithQuotes(IEnumerable<string> values) => string.Join(',', values.Select(s => $"'{s}'"));

    private readonly Chart _chart;
    private readonly string _configTemplate;
}