using System.Net.Http.Headers;
using AbstractBot;
using AbstractBot.Configs;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Operations;
using CoupleWheelBot.Records;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoupleWheelBot;

internal sealed class DialogManager
{
    public DialogManager(Bot bot, CouplesManager couplesManager, Manager recordsManager)
    {
        _bot = bot;
        _couplesManager = couplesManager;
        _recordsManager = recordsManager;
    }

    public Task StartTestAsync(Chat chat, long userId)
    {
        Partner context = _bot.CreatePartnerContext(userId);
        KeyboardProvider keyboard = CreateInviteKeyboard(_bot.Config.Texts.ShareButton, context.CoupleId);
        return _bot.Config.Texts.Invite.SendAsync(_bot, chat, keyboard);
    }

    public async Task NextStepAsync(Chat chat, Partner context, User sender)
    {
        if (string.IsNullOrWhiteSpace(context.Name))
        {
            UpdateName(context, sender.FirstName, sender.Username);
        }

        MessageTemplate messageTemplate;
        KeyboardProvider keyboardProvider = KeyboardProvider.Same;
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

        await messageTemplate.SendAsync(_bot, chat, keyboardProvider);
    }

    private void UpdateName(Partner context, string name, string? username)
    {
        context.Name = name;
        context.Username = username;
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

    public async Task ShowChartAndSaveRecordAsync(Guid guid, ChartProvider chartProvider, IEnumerable<decimal> data,
        IEnumerable<IEnumerable<string>> labels, Record? record)
    {
        List<Chat> chats = _couplesManager.GetChatsWith(guid).ToList();
        Dictionary<long, StatusMessage> statusMessages = new();

        foreach (Chat chat in chats)
        {
            await _bot.Config.Texts.ChartPreMessage.SendAsync(_bot, chat);
            statusMessages[chat.Id] = await StatusMessage.CreateAsync(_bot, chat, _bot.Config.Texts.ChartStatus,
                _bot.Config.Texts.StatusMessageEndFormat);
        }

        if (record is not null)
        {
            _recordsManager.Add(record);
            await _recordsManager.SaveAsync();
        }

        byte[]? chartPng = chartProvider.GetChart(data, labels);
        KeyboardProvider keyboardProvider = Bot.CreateSimpleKeyboard<ShowTable>(_bot.Config.Texts.TableButton);
        foreach (Chat chat in chats)
        {
            await statusMessages[chat.Id].DisposeAsync();
            await SendPngAsync(chat, chartPng, keyboardProvider, _bot.Config.Texts.ChartCaption.EscapeIfNeeded());
        }
    }

    public Task ShowTableAsync(Chat chat, byte[]? tablePng)
    {
        KeyboardProvider keyboardProvider = Bot.CreateSimpleKeyboard<ShowInfo>(_bot.Config.Texts.InfoButton);
        return SendPngAsync(chat, tablePng, keyboardProvider, _bot.Config.Texts.TableCaption.EscapeIfNeeded());
    }

    public Task ShowInfo(Chat chat)
    {
        KeyboardProvider keyboardProvider = Bot.CreateSimpleKeyboard<Finalize>(_bot.Config.Texts.FinalizeButton);
        return _bot.Config.Texts.InfoMessage.SendAsync(_bot, chat, keyboardProvider);
    }

    public Task FinalizeAsync(Chat chat)
    {
        KeyboardProvider keyboardProvider = CreateFinalizeKeyboard();
        return _bot.Config.Texts.FinalMessage.SendAsync(_bot, chat, keyboardProvider);
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

    private static KeyboardProvider CreateInviteKeyboard(string caption, Guid coupleId)
    {
        InlineKeyboardButton button = Bot.CreateCallbackButton<InvitePartner>(caption, coupleId);
        return new InlineKeyboardMarkup(button);
    }

    private KeyboardProvider CreateFinalizeKeyboard()
    {
        InlineKeyboardButton poll = Bot.CreateUriButton(_bot.Config.Texts.PollButton, _bot.Config.PollUrl);
        InlineKeyboardButton project = Bot.CreateUriButton(_bot.Config.Texts.ProjectButton, _bot.Config.ProjectUrl);
        InlineKeyboardButton newTest = Bot.CreateCallbackButton<StartTest>(_bot.Config.Texts.NewTestButton);
        InlineKeyboardButton finalShare =
            Bot.CreateCallbackButton<ShareWithFriend>(_bot.Config.Texts.FinalShareButton);
        InlineKeyboardButton chatButton = Bot.CreateUriButton(_bot.Config.Texts.ChatButton, _bot.Config.ChatUrl);

        return new InlineKeyboardMarkup(new[] { poll, project, newTest, finalShare, chatButton }.Batch(1));
    }

    public const string LinkFormat = "https://t.me/{0}?start={1}";

    private readonly Bot _bot;

    private readonly CouplesManager _couplesManager;
    private readonly Manager _recordsManager;

    private const int ButtonsTotal = 10;
    private const int ButtonsPerRaw = 5;
}