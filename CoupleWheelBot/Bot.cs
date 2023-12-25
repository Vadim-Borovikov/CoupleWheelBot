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

namespace CoupleWheelBot;

public sealed class Bot : BotWithSheets<Config, Configs.Texts, Data, StartData>
{
    public Bot(Config config) : base(config)
    {
        IImageProcessor imageProcessor = new ImageProcessor();
        DialogManager dialogManager = new(this);
        // Manager manager = new(this, DocumentsManager, imageProcessor);
        Operations.Add(new AnswerCommand(this, dialogManager));
        Operations.Add(new AcceptName(this, dialogManager));
        Operations.Add(new AcceptEstimate(this, dialogManager));
    }

    protected override void AfterLoad()
    {
        Contexts.Clear();
        Contexts.AddAll(SaveManager.Data.AnswerContexts);
    }

    protected override void BeforeSave()
    {
        SaveManager.Data.AnswerContexts = FilterContextsByValueType<Answer>();
    }

    private Dictionary<long, T> FilterContextsByValueType<T>() where T : Context
    {
        return Contexts.FilterByValueType<long, Context, T>();
    }

    public void UpdateContextFor(long id, Context context)
    {
        Contexts[id] = context;
        SaveManager.Save();
    }

    internal const string QuerySeparator = " ";
}