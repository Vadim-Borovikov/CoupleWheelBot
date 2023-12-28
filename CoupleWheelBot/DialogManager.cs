using AbstractBot;
using AbstractBot.Configs;
using CoupleWheelBot.Contexts;
using GryphonUtilities.Extensions;
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
    }

    public async Task NextStepAsync(Chat chat, long userId, Partner? context = null)
    {
        MessageTemplate messageTemplate;
        KeyboardProvider keyboardProvider = KeyboardProvider.Same;
        if (context is null)
        {
            context = _bot.CreatePartnerContext(userId);

            await _bot.Config.Texts.QuestionsStart.SendAsync(_bot, chat);

            string link = string.Format(LinkFormat, _bot.User.Denull().Username, context.CoupleId);
            await _bot.Config.Texts.InviteMessageFormat.Format(link).SendAsync(_bot, chat);
        }

        if (string.IsNullOrWhiteSpace(context.UserName))
        {
            messageTemplate = _bot.Config.Texts.NameQuestion;
        }
        else
        {
            int index = context.NextIndex;
            if (index == -1)
            {
                if (_couplesManager.IsDone(context.CoupleId))
                {
                    await _bot.OnConditionReadyAsync(context.CoupleId);
                    return;
                }

                messageTemplate = _bot.Config.Texts.WaitingForPartner;
            }
            else
            {
                messageTemplate = _bot.Config.Texts.CoupleQuestions[index];
                keyboardProvider = GetEstimateKeyboard((byte) index);
            }
        }

        await messageTemplate.SendAsync(_bot, chat, keyboardProvider);
    }

    public void AcceptName(Partner context, string name)
    {
        context.UserName = name;
        _bot.Save();
    }

    public void AcceptEstimate(Partner context, byte index, byte opinion)
    {
        context.Opinions[index] = opinion;
        _bot.Save();
    }

    public async Task FinalizeCommunicationAsync(Guid guid, byte[]? png)
    {
        IEnumerable<Chat> chats = _couplesManager.GetUserIdsWith(guid).Select(GetPrivateChat);
        foreach (Chat chat in chats)
        {
            await SendPngAsync(chat, png);
            await _bot.Config.Texts.FinalMessage.SendAsync(_bot, chat);
        }
    }

    private Task SendPngAsync(Chat chat, byte[]? png)
    {
        if (png is null)
        {
            return _bot.Config.Texts.DownloadError.SendAsync(_bot, chat);
        }

        using (MemoryStream stream = new(png))
        {
            InputFile photo = InputFile.FromStream(stream);
            return _bot.SendPhotoAsync(chat, photo);
        }
    }

    private static Chat GetPrivateChat(long userId)
    {
        return new Chat
        {
            Id = userId,
            Type = ChatType.Private
        };
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
            CallbackData = $"{nameof(Operations.AcceptOpinion)}{index}{Bot.QuerySeparator}{estimate}"
        };
    }

    private readonly Bot _bot;

    private readonly CouplesManager _couplesManager;
    // private readonly Dictionary<Guid, CoupleCondition> _coupleConditions;

    private const int ButtonsTotal = 10;
    private const int ButtonsPerRaw = 5;
    private const string LinkFormat = "https://t.me/{0}?start={1}";
}