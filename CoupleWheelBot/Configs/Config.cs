using AbstractBot.Configs;
using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Configs;

[PublicAPI]
public class Config : ConfigWithSheets<Texts>
{
    [Required]
    [MinLength(1)]
    public string GoogleSheetId { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleTitleLogs { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleTitle { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRangeLogs { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleRange { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string ChartConfigTemplate { get; init; } = null!;

    [Required]
    public Uri VideoUrl { get; init; } = null!;

    [Required]
    public Uri PollUrl { get; init; } = null!;

    [Required]
    public Uri ProjectUrl { get; init; } = null!;

    [Required]
    public Uri ChannelUrl { get; init; } = null!;

    [Required]
    public Uri OtherTestUrl { get; init; } = null!;

    public byte QuestionsNumber => (byte) Texts.CoupleQuestions.Count;
}