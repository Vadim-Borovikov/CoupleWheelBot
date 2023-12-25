﻿using CoupleWheelBot.Save;
using GoogleSheetsManager.Documents;

namespace CoupleWheelBot;

internal sealed class SheetManager
{
    public SheetManager(string range, Sheet sheet)
    {
        _range = range;
        _sheet = sheet;
    }

    public async Task UploadDataAsync(CoupleCondition condition)
    {
        List<IList<object>> values = condition.Opinions.Values.Select(GetData).ToList();
        values = Transpose(values);
        await _sheet.SaveRawAsync(_range, values);
    }

    private static IList<object> GetData(PartnerOpinion opinion)
    {
        List<object> result = new()
        {
            opinion.UserName,
            ""
        };
        result.AddRange(opinion.Estimates.Select(e => (object) (e ?? 0)));
        return result;
    }

    private static List<IList<object>> Transpose(IList<IList<object>> data)
    {
        int rowCount = data[0].Count;
        int colCount = data.Count;

        List<IList<object>> result = new();
        for (int i = 0; i < rowCount; ++i)
        {
            result.Add(new List<object>());
            for (int j = 0; j < colCount; ++j)
            {
                result[i].Add(data[j][i]);
            }
        }
        return result;
    }

    private readonly string _range;
    private readonly Sheet _sheet;
}