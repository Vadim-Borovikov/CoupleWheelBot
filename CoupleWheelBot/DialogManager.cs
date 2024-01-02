using AbstractBot;
using AbstractBot.Configs;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Operations;
using GryphonUtilities.Extensions;
using GryphonUtilities.Helpers;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoupleWheelBot;

internal sealed class DialogManager
{
    public DialogManager(Bot bot, CouplesManager couplesManager)
    {
        _bot = bot;
        _couplesManager = couplesManager;

        string inviteTextFromat = Text.FormatLines(_bot.Config.Texts.InviteMessageFormat, LinkFormat);
        _inviteUrlFormat = string.Format(ShareLinkFormat, inviteTextFromat);
    }

    public Task StartTestAsync(Chat chat, long userId)
    {
        Partner context = _bot.CreatePartnerContext(userId);
        KeyboardProvider keyboard = CreateInviteKeyboard(_bot.Config.Texts.ShareButton, _inviteUrlFormat,
            (_bot.User?.Username).Denull(), context.CoupleId, _bot.Config.Texts.NextButton);
        return _bot.Config.Texts.Invite.SendAsync(_bot, chat, keyboard);
    }

    public async Task NextStepAsync(Chat chat, Partner context)
    {
        MessageTemplate messageTemplate;
        KeyboardProvider keyboardProvider = KeyboardProvider.Same;

        if (string.IsNullOrWhiteSpace(context.UserName))
        {
            messageTemplate = _bot.Config.Texts.NameQuestion;
        }
        else
        {
            int answersAmount = context.Opinions.Count;
            if (answersAmount < _bot.Config.Texts.CoupleQuestions.Count)
            {
                messageTemplate = _bot.Config.Texts.CoupleQuestions[answersAmount]
                                      .GetMessageTemplate(_bot.Config.Texts.CoupleQuestionFormat);
                keyboardProvider = GetEstimateKeyboard((byte) answersAmount);
            }
            else
            {
                if (_couplesManager.IsDone(context.CoupleId))
                {
                    await _bot.ShowChartAsync(context.CoupleId);
                    return;
                }

                messageTemplate = _bot.Config.Texts.WaitingForPartner;
            }
        }

        await messageTemplate.SendAsync(_bot, chat, keyboardProvider);
    }

    public void AcceptName(Partner context, string name)
    {
        context.UserName = name;
        _bot.Save();
    }

    public bool AcceptOpinion(Partner context, byte index, byte opinion)
    {
        if (context.Opinions.Count != index)
        {
            return false;
        }

        context.Opinions.Add(opinion);
        _bot.Save();
        return true;
    }

    public async Task ShowChartAsync(Guid guid, byte[]? chartPng)
    {
        KeyboardProvider keyboardProvider = Bot.CreateSimpleKeyboard<ShowTable>(_bot.Config.Texts.TableButton);
        foreach (Chat chat in _couplesManager.GetChatsWith(guid))
        {
            await _bot.Config.Texts.ChartPreMessage.SendAsync(_bot, chat);
            await SendPngAsync(chat, chartPng, keyboardProvider, _bot.Config.Texts.ChartCaption.EscapeIfNeeded());
        }
    }

    public async Task ShowTableAsync(Chat chat, byte[]? tablePng)
    {
        KeyboardProvider keyboardProvider = Bot.CreateSimpleKeyboard<Finalize>(_bot.Config.Texts.FinalizeButton);
        await _bot.Config.Texts.TablePreMessage.SendAsync(_bot, chat);
        await SendPngAsync(chat, tablePng, keyboardProvider, _bot.Config.Texts.TableCaption.EscapeIfNeeded());
    }

    public async Task FinalizeAsync(Chat chat)
    {
        await _bot.Config.Texts.FinalMessageStart.SendAsync(_bot, chat);
        await _bot.Config.Texts.FinalMessageVideoFormat.Format(_bot.Config.VideoUrl).SendAsync(_bot, chat);

        KeyboardProvider keyboardProvider = CreateFinalizeKeyboard();
        await _bot.Config.Texts.FinalMessageEnd.SendAsync(_bot, chat, keyboardProvider);
    }

    private Task SendPngAsync(Chat chat, byte[]? png, KeyboardProvider? keyboardProvider = null,
        string? caption = null)
    {
        if (png is null)
        {
            return _bot.Config.Texts.DownloadError.SendAsync(_bot, chat);
        }

        using (MemoryStream stream = new(png))
        {
            InputFile photo = InputFile.FromStream(stream);
            return _bot.SendPhotoAsync(chat, photo, keyboardProvider, caption: caption,
                parseMode: ParseMode.MarkdownV2);
        }
    }

    private static InlineKeyboardMarkup GetEstimateKeyboard(byte index)
    {
        IEnumerable<int> numbers = Enumerable.Range(1, ButtonsTotal);
        IEnumerable<InlineKeyboardButton> buttons = numbers.Select(n => GetEstimateButton(index, (byte) n));
        IEnumerable<IEnumerable<InlineKeyboardButton>> keyboard = buttons.Batch(ButtonsPerRaw);
        return new InlineKeyboardMarkup(keyboard);
    }

    private static InlineKeyboardButton GetEstimateButton(byte index, byte estimate)
    {
        return new InlineKeyboardButton(estimate.ToString())
        {
            CallbackData = $"{nameof(Operations.AcceptOpinion)}{index}{Bot.QuerySeparator}{estimate}",
        };
    }

    private static KeyboardProvider CreateInviteKeyboard(string shareButtonCaption, string urlFormat, string botName,
        Guid coupleId, string nextButtonCaption)
    {
        Uri uri = new(string.Format(urlFormat, botName, coupleId));
        InlineKeyboardButton invite = Bot.CreateUriButton(shareButtonCaption, uri);

        InlineKeyboardButton next = Bot.CreateCallbackButton<ContinueTest>(nextButtonCaption);

        return new InlineKeyboardMarkup(new[] { invite, next }.Batch(1));
    }

    private KeyboardProvider CreateFinalizeKeyboard()
    {
        InlineKeyboardButton poll = Bot.CreateUriButton(_bot.Config.Texts.PollButton, _bot.Config.PollUrl);
        InlineKeyboardButton project = Bot.CreateUriButton(_bot.Config.Texts.ProjectButton, _bot.Config.ProjectUrl);
        InlineKeyboardButton channel = Bot.CreateUriButton(_bot.Config.Texts.ChannelButton, _bot.Config.ChannelUrl);
        InlineKeyboardButton newTest = Bot.CreateCallbackButton<StartTest>(_bot.Config.Texts.NewTestButton);
        InlineKeyboardButton otherTest =
            Bot.CreateUriButton(_bot.Config.Texts.OtherTestButton, _bot.Config.OtherTestUrl);

        return new InlineKeyboardMarkup(new[] { poll, project, channel, newTest, otherTest }.Batch(1));
    }

    private readonly Bot _bot;

    private readonly CouplesManager _couplesManager;
    private readonly string _inviteUrlFormat;

    private const int ButtonsTotal = 10;
    private const int ButtonsPerRaw = 5;
    private const string LinkFormat = "https://t.me/{0}?start={1}";
    private const string ShareLinkFormat = "https://t.me/share/url?text={0}";
}