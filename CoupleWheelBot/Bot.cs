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
        CouplesManager couplesManager = new(SaveManager.Data.PartnerContexts);

        IImageProcessor imageProcessor = new ImageProcessor();
        _dialogManager = new DialogManager(this, couplesManager);
        _fileManager = new FileManager(this, DocumentsManager, imageProcessor);

        AnswerCommand answerCommand = new(this, _dialogManager);

        Operations.Add(answerCommand);
        Operations.Add(new AcceptName(this, _dialogManager));
        Operations.Add(new AcceptOpinion(this, _dialogManager));

        Start.Format($"/{answerCommand.BotCommand.Command}");


        GoogleSheetsManager.Documents.Document document = DocumentsManager.GetOrAdd(Config.GoogleSheetId);
        Sheet sheet = document.GetOrAddSheet(Config.GoogleTitle);
        _sheetManager = new SheetManager(Config.GoogleRange, sheet);
    }

    protected override void AfterLoad()
    {
        Contexts.Clear();
        Contexts.AddAll(SaveManager.Data.PartnerContexts);
    }

    protected override void BeforeSave()
    {
        SaveManager.Data.PartnerContexts.Clear();

        Dictionary<long, Partner> contexts = Contexts.FilterByValueType<long, Context, Partner>();
        SaveManager.Data.PartnerContexts.AddAll(contexts);
    }

    protected override Task OnStartCommand(StartData info, Message message, User sender)
    {
        if (SaveManager.Data.PartnerContexts.Values.All(c => c.CoupleId != info.CoupleId))
        {
            return Config.Texts.InviteError.SendAsync(this, message.Chat);
        }

        Partner context = CreatePartnerContext(sender.Id, info.CoupleId);
        return _dialogManager.NextStepAsync(message.Chat, sender.Id, context);
    }

    internal Partner CreatePartnerContext(long userId, Guid? coupleId = null)
    {
        Partner context = new()
        {
            CoupleId = coupleId ?? Guid.NewGuid(),
            IsInitiator = coupleId is null,
            Opinions = Enumerable.Repeat((byte?) null, Config.QuestionsNumber).ToList()
        };
        UpdateContextFor(userId, context);
        return context;
    }

    internal async Task OnConditionReadyAsync(Guid guid)
    {
        await _sheetManager.UploadDataAsync(SaveManager.Data
                                                       .PartnerContexts
                                                       .Values
                                                       .Where(c => c.CoupleId == guid)
                                                       .OrderByDescending(c => c.IsInitiator));
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