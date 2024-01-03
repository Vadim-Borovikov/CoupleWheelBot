using System.ComponentModel.DataAnnotations;
using AbstractBot.Configs;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace CoupleWheelBot.Configs;

[PublicAPI]
public class Texts : AbstractBot.Configs.Texts
{
    [Required]
    public string DescribeButton { get; init; } = null!;

    [Required]
    public MessageTemplate Describe { get; init; } = null!;

    [Required]
    public string InviteButton { get; init; } = null!;

    [Required]
    public MessageTemplate Invite { get; init; } = null!;

    [Required]
    public string ShareButton { get; init; } = null!;

    [Required]
    public MessageTemplate InviteMessageFormat { get; init; } = null!;

    [Required]
    public MessageTemplate InviteReply { get; init; } = null!;

    [Required]
    public MessageTemplate FinaLShareMessageFormat { get; init; } = null!;

    [Required]
    public MessageTemplate FinaLShareReply { get; init; } = null!;

    [Required]
    public string NextButton { get; init; } = null!;

    [Required]
    public MessageTemplate NameQuestion { get; init; } = null!;

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
    public MessageTemplate ChartPreMessage { get; init; } = null!;

    [Required]
    public MessageTemplate ChartCaption { get; init; } = null!;

    [Required]
    public string TableButton { get; init; } = null!;

    [Required]
    public MessageTemplate TablePreMessage { get; init; } = null!;

    [Required]
    public MessageTemplate TableCaption { get; init; } = null!;

    [Required]
    public string FinalizeButton { get; init; } = null!;

    [Required]
    public MessageTemplate FinalMessageStart { get; init; } = null!;

    [Required]
    public MessageTemplate FinalMessageVideoFormat { get; init; } = null!;

    [Required]
    public MessageTemplate FinalMessageEnd { get; init; } = null!;

    [Required]
    public MessageTemplate ChartStatus { get; init; } = null!;

    [Required]
    public MessageTemplate TableStatus { get; init; } = null!;

    [Required]
    public string PollButton { get; init; } = null!;

    [Required]
    public string ProjectButton { get; init; } = null!;

    [Required]
    public string ChannelButton { get; init; } = null!;

    [Required]
    public string NewTestButton { get; init; } = null!;

    [Required]
    public string FinalShareButton { get; init; } = null!;

    [Required]
    public string OtherTestButton { get; init; } = null!;
}