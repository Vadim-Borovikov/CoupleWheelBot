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
    public MessageTemplate QuestionsStart { get; init; } = null!;

    [Required]
    public MessageTemplate InviteMessageFormat { get; init; } = null!;

    [Required]
    public MessageTemplate InviteError { get; init; } = null!;

    [Required]
    public MessageTemplate DownloadError { get; init; } = null!;

    [Required]
    public MessageTemplate CoupleQuestionFormat { get; init; } = null!;

    [Required]
    public List<Question> CoupleQuestions { get; init; } = null!;

    [Required]
    public MessageTemplate WaitingForPartner { get; init; } = null!;

    [Required]
    public MessageTemplate FinalMessage { get; init; } = null!;
}