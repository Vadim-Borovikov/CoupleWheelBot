using AbstractBot;
using AbstractBot.Configs;
using CoupleWheelBot.Contexts;
using MoreLinq.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoupleWheelBot;

internal sealed class DialogManager
{
    public DialogManager(Bot bot) => _bot = bot;

    public Task NextStepAsync(Chat chat, long userId, Answer? context = null)
    {
        MessageTemplate messageTemplate;
        KeyboardProvider keyboardProvider = KeyboardProvider.Same;
        if (context is null)
        {
            messageTemplate = _bot.Config.Texts.NameQuestion;
            context = new Answer(userId, _bot.Config.QuestionsNumber);
            _bot.UpdateContextFor(userId, context);
        }
        else
        {
            int index = context.CoupleCondition.Views[userId].Estimates.IndexOf(null);
            if (index == -1)
            {
                messageTemplate = _bot.Config.Texts.QuestionsEnded;
            }
            else
            {
                messageTemplate = _bot.Config.Texts.CoupleQuestions[index];
                keyboardProvider = GetEstimateKeyboard((byte) index);
            }
        }

        return messageTemplate.SendAsync(_bot, chat, keyboardProvider);
    }

    public void AcceptName(long userId, Answer context, string name)
    {
        context.CoupleCondition.Views[userId].UserName = name;
        _bot.UpdateContextFor(userId, context);
    }

    public void AcceptEstimate(long userId, Answer context, byte index, byte estimate)
    {
        context.CoupleCondition.Views[userId].Estimates[index] = estimate;
        _bot.UpdateContextFor(userId, context);
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
            CallbackData = $"{nameof(AcceptEstimate)}{index}{Bot.QuerySeparator}{estimate}"
        };
    }

    private readonly Bot _bot;

    private const int ButtonsTotal = 10;
    private const int ButtonsPerRaw = 5;
}