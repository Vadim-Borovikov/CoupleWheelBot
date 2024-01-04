using GoogleSheetsManager.Documents;

namespace CoupleWheelBot.Records;

internal sealed class Manager
{
    public Manager(Sheet sheet, string range)
    {
        _sheet = sheet;
        _range = range;
        _additionalSavers = new List<Action<Record, IDictionary<string, object?>>>
        {
            Record.Save
        };
    }

    public async Task LoadAsync()
    {
        _records = await _sheet.LoadAsync<Record>(_range);
    }

    public void Add(Record record) => _records.Insert(0, record);

    public Task SaveAsync()
    {
        _records = _records.OrderByDescending(r => r.Date).ThenByDescending(r => r.Time).ToList();
        return _sheet.SaveAsync(_range, _records, additionalSavers: _additionalSavers);
    }

    private readonly Sheet _sheet;
    private readonly string _range;
    private List<Record> _records = new();
    private readonly List<Action<Record, IDictionary<string, object?>>> _additionalSavers;
}