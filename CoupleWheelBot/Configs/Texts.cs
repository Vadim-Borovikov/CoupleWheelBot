using System.ComponentModel.DataAnnotations;
using AbstractBot.Configs;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Configs;

[PublicAPI]
public class Texts : AbstractBot.Configs.Texts
{
    [Required]
    [MinLength(1)]
    public string AnswerCommandDescription { get; init; } = null!;

    [Required]
    public MessageTemplate NameQuestion { get; init; } = null!;

    [Required]
    public MessageTemplate QuestionsEnded { get; init; } = null!;

    [Required]
    public List<MessageTemplate> CoupleQuestions { get; init; } = null!;
}