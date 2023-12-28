using System.ComponentModel.DataAnnotations;
using AbstractBot.Configs;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Configs;

[PublicAPI]
public class Question
{
    [Required]
    public string Title { get; init; } = null!;

    [Required]
    public string Description { get; init; } = null!;

    [Required]
    public string Low { get; init; } = null!;

    [Required]
    public string High { get; init; } = null!;

    public MessageTemplate GetMessageTemplate(MessageTemplate format) => format.Format(Title, Description, Low, High);
}