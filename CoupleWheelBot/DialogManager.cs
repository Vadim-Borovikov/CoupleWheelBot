using AbstractBot;
using AbstractBot.Configs;
using CoupleWheelBot.Save;
using GryphonUtilities.Extensions;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoupleWheelBot;

internal sealed class DialogManager
{
    public DialogManager(Bot bot, Dictionary<Guid, CoupleCondition> coupleConditions)
    {
        _bot = bot;
        _coupleConditions = coupleConditions;
    }

    public async Task NextStepAsync(Chat chat, long userId, Guid? guid = null)
    {
        MessageTemplate messageTemplate;
        KeyboardProvider keyboardProvider = KeyboardProvider.Same;
        if (guid is null)
        {
            guid = _bot.CreateCoupleConditionFor(userId);

            await _bot.Config.Texts.QuestionsStart.SendAsync(_bot, chat);

            string link = string.Format(LinkFormat, _bot.User.Denull().Username, guid);
            await _bot.SendTextMessageAsync(chat, link);
        }

        if (_coupleConditions[guid.Value].Opinions.ContainsKey(userId))
        {
            int index = _coupleConditions[guid.Value].Opinions[userId].NextIndex;
            if (index == -1)
            {
                if (_coupleConditions[guid.Value].Done)
                {
                    messageTemplate = _bot.Config.Texts.QuestionsEnded;
                }
                else
                {
                    messageTemplate = _bot.Config.Texts.WaitingForPartner;
                }
            }
            else
            {
                messageTemplate = _bot.Config.Texts.CoupleQuestions[index];
                keyboardProvider = GetEstimateKeyboard((byte) index);
            }
        }
        else
        {
            messageTemplate = _bot.Config.Texts.NameQuestion;
        }

        await messageTemplate.SendAsync(_bot, chat, keyboardProvider);
    }

    public void AcceptName(long userId, Guid guid, string name)
    {
        _coupleConditions[guid].Opinions[userId] = new PartnerOpinion(name, _bot.Config.QuestionsNumber);
        _bot.Save();
    }

    public void AcceptEstimate(long userId, Guid guid, byte index, byte estimate)
    {
        _coupleConditions[guid].Opinions[userId].Estimates[index] = estimate;
        _bot.Save();
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
            CallbackData = $"{nameof(Operations.AcceptEstimate)}{index}{Bot.QuerySeparator}{estimate}"
        };
    }

    private readonly Bot _bot;
    private readonly Dictionary<Guid, CoupleCondition> _coupleConditions;

    private const int ButtonsTotal = 10;
    private const int ButtonsPerRaw = 5;
    private const string LinkFormat = "https://t.me/{0}?start={1}";
}