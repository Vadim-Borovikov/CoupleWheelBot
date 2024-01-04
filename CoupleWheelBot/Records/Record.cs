using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Save;
using GoogleSheetsManager;
using GryphonUtilities.Extensions;
using GryphonUtilities.Time;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Records;

internal sealed class Record
{
    [Required]
    [MinLength(1)]
    [UsedImplicitly]
    [SheetField("Бот")]
    public string BotName = null!;

    [Required]
    [UsedImplicitly]
    [SheetField("Дата", "{0:dd.MM.yyyy}")]
    public DateOnly Date;

    [Required]
    [UsedImplicitly]
    [SheetField("Время", "{0:T}")]
    public TimeOnly Time;

    [Required]
    [MinLength(1)]
    [UsedImplicitly]
    [SheetField("Пользователь")]
    public string User = null!;

    [Required]
    [MinLength(1)]
    [UsedImplicitly]
    [SheetField("Партнёр")]
    public string Partner = null!;

    [Required]
    [MinLength(1)]
    [UsedImplicitly]
    [SheetField(UserOpinionsCaption)]
    public List<byte> UserOpinions = null!;

    [Required]
    [MinLength(1)]
    [UsedImplicitly]
    [SheetField(PartnerOpinionsCaption)]
    public List<byte> PartnerOpinions = null!;

    [Required]
    [MinLength(1)]
    [UsedImplicitly]
    [SheetField(AverageCaption)]
    public List<decimal> Average = null!;

    [UsedImplicitly]
    public Record() { }

    public static Record Create(Bot bot, Data saveData, long userId, long partnerId, List<decimal> average)
    {
        DateTimeFull now = bot.Clock.Now();
        Partner userData = saveData.PartnerContexts[userId];
        Partner partnerData = saveData.PartnerContexts[partnerId];

        return new Record
        {
            BotName = $"@{(bot.User?.Username).Denull()}",
            Date = now.DateOnly,
            Time = now.TimeOnly,
            User = GetUserDescription(userId, userData),
            Partner = GetUserDescription(partnerId, partnerData),
            UserOpinions = userData.Opinions,
            PartnerOpinions = partnerData.Opinions,
            Average = average
        };
    }

    public static void Save(Record record, IDictionary<string, object?> valueSet)
    {
        valueSet[UserOpinionsCaption] = string.Join(ElementsSeparator, record.UserOpinions);
        valueSet[PartnerOpinionsCaption] = string.Join(ElementsSeparator, record.PartnerOpinions);
        valueSet[AverageCaption] = string.Join(ElementsSeparator, record.Average);
    }

    private static string GetUserDescription(long id, Partner data)
    {
        string result = data.Name;
        if (!string.IsNullOrWhiteSpace(data.Username))
        {
            result += $" @{data.Username}";
        }
        result += $" ({id})";
        return result;
    }

    private const string UserOpinionsCaption = "Оценки пользователя";
    private const string PartnerOpinionsCaption = "Оценки партнёра";
    private const string AverageCaption = "Среднее";

    public const char ElementsSeparator = ';';
}