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
using GoogleSheetsManager.Documents;
using Telegram.Bot.Types;

namespace CoupleWheelBot;

public sealed class Bot : BotWithSheets<Config, Texts, Data, StartData>
{
    public Bot(Config config) : base(config)
    {
        IImageProcessor imageProcessor = new ImageProcessor();
        _dialogManager = new DialogManager(this, SaveManager.Data.CoupleConditions);
        _fileManager = new FileManager(this, DocumentsManager, imageProcessor);

        AnswerCommand answerCommand = new(this, _dialogManager);

        Operations.Add(answerCommand);
        Operations.Add(new AcceptName(this, _dialogManager));
        Operations.Add(new AcceptEstimate(this, _dialogManager));

        Start.Format($"/{answerCommand.BotCommand.Command}");


        GoogleSheetsManager.Documents.Document document = DocumentsManager.GetOrAdd(Config.GoogleSheetId);
        Sheet sheet = document.GetOrAddSheet(Config.GoogleTitle);
        _sheetManager = new SheetManager(Config.GoogleRange, sheet);
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

    internal Guid CreateCoupleConditionFor(long userId)
    {
        Guid guid = Guid.NewGuid();
        SaveManager.Data.CoupleConditions[guid] = new CoupleCondition();

        Answer context = new() { Guid = guid };
        UpdateContextFor(userId, context);

        return guid;
    }

    internal async Task OnConditionReadyAsync(Guid guid)
    {
        await _sheetManager.UploadDataAsync(SaveManager.Data.CoupleConditions[guid]);
        byte[]? png = await _fileManager.DownloadAsync();
        await _dialogManager.FinalizeCommunicationAsync(guid, png);
    }

    internal void Save() => SaveManager.Save();

    private void UpdateContextFor(long id, Context context)
    {
        Contexts[id] = context;
        SaveManager.Save();
    }

    private readonly DialogManager _dialogManager;
    private readonly SheetManager _sheetManager;
    private readonly FileManager _fileManager;

    internal const string QuerySeparator = " ";
}