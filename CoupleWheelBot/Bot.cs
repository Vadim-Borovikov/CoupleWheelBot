using AbstractBot;
using AbstractBot.Bots;
using CoupleWheelBot.Configs;
using CoupleWheelBot.Contexts;
using CoupleWheelBot.Extensions;
using CoupleWheelBot.ImageProcessing;
using CoupleWheelBot.Operations;
using CoupleWheelBot.Operations.Infos;
using CoupleWheelBot.Records;
using CoupleWheelBot.Save;
using GoogleSheetsManager.Documents;
using GryphonUtilities.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace CoupleWheelBot;

public sealed class Bot : BotWithSheets<Config, Texts, Data, StartData>
{
    protected override KeyboardProvider StartKeyboardProvider { get; }

    public Bot(Config config) : base(config)
    {
        CouplesManager couplesManager = new(SaveManager.Data.PartnerContexts, Config.QuestionsNumber);

        GoogleSheetsManager.Documents.Document document = DocumentsManager.GetOrAdd(Config.GoogleSheetId);
        Dictionary<Type, Func<object?, object?>> additionalConverters = new()
        {
            { typeof(List<byte>), o => o.ToBytes() },
            { typeof(List<decimal>), o => o.ToDecimals() }
        };
        Sheet recordsSheet = document.GetOrAddSheet(Config.GoogleTitleLogs, additionalConverters);
        _recordsManager = new Records.Manager(recordsSheet, Config.GoogleRangeLogs);

        IImageProcessor imageProcessor = new ImageProcessor();
        _dialogManager = new DialogManager(this, couplesManager, _recordsManager);
        _fileManager = new FileManager(this, DocumentsManager, imageProcessor);

        Operations.Add(new DescribeTest(this));
        Operations.Add(new AcceptOpinion(this, _dialogManager));
        Operations.Add(new StartTest(this, _dialogManager));
        Operations.Add(new ContinueTest(this, _dialogManager));
        Operations.Add(new ShowTable(this));
        Operations.Add(new Finalize(this, _dialogManager));

        KeyboardProvider keyboardProvider = CreateSimpleKeyboard<ContinueTest>(config.Texts.NextButton);
        Operations.Add(new InvitePartner(this, keyboardProvider));

        Operations.Add(new ShareWithFriend(this));

        Sheet resultsSheet = document.GetOrAddSheet(Config.GoogleTitle);
        _sheetManager = new SheetManager(Config.GoogleRange, resultsSheet);

        _chartProvider = new ChartProvider(config.ChartConfigTemplate, imageProcessor);

        StartKeyboardProvider = CreateSimpleKeyboard<DescribeTest>(Config.Texts.DescribeButton);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        await _recordsManager.LoadAsync();
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

        if (SaveManager.Data.PartnerContexts.ContainsKey(sender.Id) &&
            (SaveManager.Data.PartnerContexts[sender.Id].CoupleId == info.CoupleId))
        {
            return Task.CompletedTask;
        }

        Partner context = CreatePartnerContext(sender.Id, info.CoupleId);
        return _dialogManager.NextStepAsync(message.Chat, context, sender);
    }

    internal static InlineKeyboardButton CreateCallbackButton<TCallback>(string caption, params object[]? args)
    {
        string data = typeof(TCallback).Name;
        if (args is not null)
        {
            data += string.Join(QuerySeparator, args.Select(o => o.ToString().Denull()));
        }
        return new InlineKeyboardButton(caption)
        {
            CallbackData = data
        };
    }

    internal static InlineKeyboardButton CreateUriButton(string caption, Uri uri)
    {
        return new InlineKeyboardButton(caption)
        {
            Url = uri.AbsoluteUri
        };
    }

    internal static KeyboardProvider CreateSimpleKeyboard<TCallback>(string buttonCaption)
    {
        InlineKeyboardButton button = CreateCallbackButton<TCallback>(buttonCaption);
        return new InlineKeyboardMarkup(button);
    }

    internal Partner CreatePartnerContext(long userId, Guid? coupleId = null)
    {
        Partner context = new()
        {
            CoupleId = coupleId ?? Guid.NewGuid(),
            IsInitiator = coupleId is null,
            Opinions = new List<byte>()
        };
        UpdateContextFor(userId, context);
        return context;
    }

    internal async Task ShowChartAsync(Guid guid)
    {
        List<decimal> data = new();
        List<KeyValuePair<long, Partner>> pairs = SaveManager.Data.GetContextsWith(guid).ToList();
        for (int i = 0; i < Config.QuestionsNumber; ++i)
        {
            data.Add(pairs.Average(p => (decimal) p.Value.Opinions[i]));
        }

        bool isCoupleProper = (pairs.Count == CouplesManager.PartnersInCouple) && pairs[0].Value.IsInitiator
                                                                               && !pairs[1].Value.IsInitiator;
        Record? record =
            isCoupleProper ? Record.Create(this, SaveManager.Data, pairs[0].Key, pairs[1].Key, data) : null;

        await _dialogManager.ShowChartAndSaveRecordAsync(guid, _chartProvider, data,
            Config.Texts.CoupleQuestions.Select(q => q.Title), record);
    }

    internal async Task ShowTableAsync(Guid guid, Chat chat)
    {
        await Config.Texts.TablePreMessage.SendAsync(this, chat);

        byte[]? tablePng;
        await using (await StatusMessage.CreateAsync(this, chat, Config.Texts.TableStatus))
        {
            await _sheetManager.UploadDataAsync(SaveManager.Data.GetContextsWith(guid).Select(p => p.Value));
            tablePng = await _fileManager.DownloadAsync();
        }
        await _dialogManager.ShowTableAsync(chat, tablePng);
    }

    internal void Save() => SaveManager.Save();

    private void UpdateContextFor(long id, Context context)
    {
        Contexts[id] = context;
        SaveManager.Save();
    }

    private readonly Records.Manager _recordsManager;
    private readonly DialogManager _dialogManager;
    private readonly SheetManager _sheetManager;
    private readonly FileManager _fileManager;
    private readonly ChartProvider _chartProvider;

    internal const string QuerySeparator = "_";
}