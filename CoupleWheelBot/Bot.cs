using AbstractBot;
using AbstractBot.Bots;
using CoupleWheelBot.Configs;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Extensions;
using CoupleWheelBot.ImageProcessing;
using CoupleWheelBot.Operations;
using CoupleWheelBot.Operations.Commands;
using CoupleWheelBot.Operations.Infos;
using CoupleWheelBot.Save;
using Telegram.Bot.Types;

namespace CoupleWheelBot;

public sealed class Bot : BotWithSheets<Config, Texts, Data, StartData>
{
    public Bot(Config config) : base(config)
    {
        IImageProcessor imageProcessor = new ImageProcessor();
        _dialogManager = new DialogManager(this, SaveManager.Data.CoupleConditions);
        // Manager manager = new(this, DocumentsManager, imageProcessor);

        AnswerCommand answerCommand = new(this, _dialogManager);

        Operations.Add(answerCommand);
        Operations.Add(new AcceptName(this, _dialogManager));
        Operations.Add(new AcceptEstimate(this, _dialogManager));

        Start.Format($"/{answerCommand.BotCommand.Command}");
    }

    protected override void AfterLoad()
    {
        Contexts.Clear();
        Contexts.AddAll(SaveManager.Data.AnswerContexts);
    }

    private void ClearUnattachedConditions()
    {
        SaveManager.Data.CoupleConditions =
            SaveManager.Data.CoupleConditions
                            .Where(p => SaveManager.Data.AnswerContexts.Values.Any(v => v.Guid == p.Key))
                            .ToDictionary(p => p.Key, p => p.Value);
    }

    protected override void BeforeSave()
    {
        SaveManager.Data.AnswerContexts = FilterContextsByValueType<Answer>();
        ClearUnattachedConditions();
    }

    private Dictionary<long, T> FilterContextsByValueType<T>() where T : Context
    {
        return Contexts.FilterByValueType<long, Context, T>();
    }

    protected override Task OnStartCommand(StartData info, Message message, User sender)
    {
        if (!SaveManager.Data.CoupleConditions.ContainsKey(info.Guid))
        {
            return Config.Texts.InviteError.SendAsync(this, message.Chat);
        }

        Answer context = new() { Guid = info.Guid };
        UpdateContextFor(sender.Id, context);
        return _dialogManager.NextStepAsync(message.Chat, sender.Id, context.Guid);
    }

    public Guid CreateCoupleConditionFor(long userId)
    {
        Guid guid = Guid.NewGuid();
        SaveManager.Data.CoupleConditions[guid] = new CoupleCondition();

        Answer context = new() { Guid = guid };
        UpdateContextFor(userId, context);

        return guid;
    }

    public void UpdateContextFor(long id, Context context)
    {
        Contexts[id] = context;
        SaveManager.Save();
    }

    public void Save() => SaveManager.Save();

    private readonly DialogManager _dialogManager;

    internal const string QuerySeparator = " ";
}