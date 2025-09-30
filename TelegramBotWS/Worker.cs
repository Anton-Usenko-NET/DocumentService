namespace TelegramBotWS;
using Documents;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class Worker : BackgroundService
{
    private readonly CancellationTokenSource cts = new();
    private readonly string token = Environment.GetEnvironmentVariable("BOT_TOKEN");

    private readonly ILogger<Worker> _logger;
    private TelegramBotClient bot;
    private User me;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        if (token is null)
            _logger.LogError("Telegram bot token is null");
        bot = new TelegramBotClient(token, cancellationToken: cts.Token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        me = await bot.GetMe();
        await bot.DeleteWebhook();
        await bot.DropPendingUpdates();

        bot.OnError += OnError;
        bot.OnMessage += OnMessage;
        bot.OnUpdate += OnUpdate;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation($"@{me.Username} �������� {DateTimeOffset.Now}");
        }
    }

    private async Task OnError(Exception exception, HandleErrorSource source)
    {
        Console.WriteLine(exception);
        await Task.Delay(2000, cts.Token);
    }

    private async Task OnMessage(Message msg, UpdateType type)
    {
        if (msg.Text is not { } text)
        {
            Console.WriteLine($"�������� ����������� ���� {msg.Type}");
        }
        else if (text.StartsWith('/'))
        {
            var space = text.IndexOf(' ');
            if (space < 0) space = text.Length;
            var command = text[..space].ToLower();

            if (command.LastIndexOf('@') is > 0 and int at)
            {
                if (command[(at + 1)..].Equals(me.Username, StringComparison.OrdinalIgnoreCase))
                    command = command[..at];
                else
                    return;
            }

            await OnCommand(command, text[space..].TrimStart(), msg);
        }
        else
        {
            await OnTextMessage(msg);
        }
    }

    private async Task OnTextMessage(Message msg)
    {
        Console.WriteLine($"�������� �����: '{msg.Text}' � ��� {msg.Chat}");
        await OnCommand("/start", "", msg);
    }

    private async Task OnCommand(string command, string args, Message msg)
    {
        Console.WriteLine($"�������� �������: {command} {args}");
        DriveFileManager doc;
        Dictionary<string, string> map;
        string tempCopyId;
        string url;

        string words1 = "�������", words2 = "���", words3 = "���������", words4 = "��������";

        switch (command)
        {
            case "/start":
                await bot.SendMessage(msg.Chat, """
                <b><u>���� ����</u></b>:
                /templates � ��������� �� ������� ��������� � �����
                /create � �������� �� ���������

                /create_contract_1 � ������ (�������)
                /create_contract_2 � ������ (���)
                /create_contract_3 � ������ (���������)
                /create_contract_4 � ������ (��������)

                /create_expence_invoice � ��������� ��������
                /create_invoice_for_payment � ������� �� ������
                /create_ttn � ���
                """, parseMode: ParseMode.Html, linkPreviewOptions: true,
                    replyMarkup: new ReplyKeyboardRemove());
                break;

            case "/templates":
                await bot.SendMessage(msg.Chat, """
                <b><u>�������</u></b>:

                ������ ������� (<a href="https://docs.google.com/document/d/1PbJyJ1PzEUc6kdAF2yUiF4TPPCZYKusd7TdCE4gdNA0/edit?tab=t.0">url</a>)
                ������ ��� (<a href="https://docs.google.com/document/d/1wdy7If4VLSU4lqM7r0NaAmpSdP9oVBUOg8TdSbYw3T4/edit?tab=t.0">url</a>)
                ������ ��������� (<a href="https://docs.google.com/document/d/1pSLFaUuBM-9DDDiEIvw9EL-unsS5MRQbvgJ9OOBxQqk/edit?tab=t.0">url</a>)
                ������ �������� (<a href="https://docs.google.com/document/d/1vIJXsr85Kn4mx4JU9YW5F23Uf21gUyaHtnLx6ODAmNU/edit?tab=t.0">url</a>)

                ��������� �������� (<a href="https://docs.google.com/spreadsheets/d/1hPygv5_LQPx_1RmJmPZau1apdioMfvrQRmEYVosj9Gg/edit?gid=55539154#gid=55539154">url</a>)
                ��� (<a href="https://docs.google.com/spreadsheets/d/19HdepTKLBBSXnxlwL9asShw9kiSkf3TJnImiUC05L1s/edit?gid=583253378#gid=583253378">url</a>)
                ������� �� ������ (<a href="https://docs.google.com/spreadsheets/d/1BOOzbt-UBX0OvKMdjy1eSSEaurbM4O2txDK0F2ebhyg/edit?gid=561611553#gid=561611553">url</a>)

                ��� (<a href="https://docs.google.com/document/d/1xnX3KMYord1psCHvb7fupfyy1IKyWE9tOoyEHC95L-M/edit?tab=t.0">url</a>)
                """, parseMode: ParseMode.Html, linkPreviewOptions: true,
                     replyMarkup: new ReplyKeyboardRemove());
                break;

            case "/create":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>",
                    parseMode: ParseMode.Html, linkPreviewOptions: true,
                    replyMarkup: new ReplyKeyboardRemove());

                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);

                string doc1 = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract, words1);
                string doc2 = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract2, words2);
                string doc3 = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract3, words3);
                string doc4 = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract4, words4);

                string name = map.GetValueOrDefault("{{����� ���}}");
                if (name.Equals("{{����}}")) name = "��� " + DateTime.Now;
                string ttn = doc.CopyAndUpdateRelationSheet2(map, DriveFileManager.TemplateTTNSheetId, DriveFileManager.TemplateTTNSheetGrid, name);

                name = map.GetValueOrDefault("{{����� ��������� ��������}}");
                if (name.Equals("{{����}}")) name = "��������� �������� " + DateTime.Now;
                string expInvoice = doc.CopyAndUpdateRelationSheet2(map, DriveFileManager.TemplateExpenceInvoiceSheetId, DriveFileManager.TemplateExpenceInvoiceSheetGrid, name);

                name = map.GetValueOrDefault("{{����� ������� �� ������}}");
                if (name.Equals("{{����}}")) name = "������� �� ������ " + DateTime.Now;
                string invForPayment = doc.CopyAndUpdateRelationSheet2(map, DriveFileManager.TemplateInvoiceForPaymentSheetId, DriveFileManager.TemplateInvoiceForPaymentSheetGrid, name);

                string result = $"""
                <b><u>���������</u></b>:

                ������ ������� (<a href="https://docs.google.com/document/d/{doc1}/edit">url</a>)
                ������ ��� (<a href="https://docs.google.com/document/d/{doc2}/edit">url</a>)
                ������ ��������� (<a href="https://docs.google.com/document/d/{doc3}/edit">url</a>)
                ������ �������� (<a href="https://docs.google.com/document/d/{doc4}/edit">url</a>)

                ��� (<a href="https://docs.google.com/spreadsheets/d/{ttn}/edit">url</a>)
                ��������� �������� (<a href="https://docs.google.com/spreadsheets/d/{expInvoice}/edit">url</a>)
                ������� �� ������ (<a href="https://docs.google.com/spreadsheets/d/{invForPayment}/edit">url</a>)
                """;

                await bot.SendMessage(msg.Chat, result, parseMode: ParseMode.Html,
                    linkPreviewOptions: true, replyMarkup: new ReplyKeyboardRemove());
                break;

            case "/create_contract_1":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>", parseMode: ParseMode.Html);
                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);
                tempCopyId = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract, words1);
                url = $"https://docs.google.com/document/d/{tempCopyId}/edit";
                await bot.SendMessage(msg.Chat, url);
                break;

            case "/create_contract_2":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>", parseMode: ParseMode.Html);
                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);
                tempCopyId = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract2, words2);
                url = $"https://docs.google.com/document/d/{tempCopyId}/edit";
                await bot.SendMessage(msg.Chat, url);
                break;

            case "/create_contract_3":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>", parseMode: ParseMode.Html);
                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);
                tempCopyId = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract3, words3);
                url = $"https://docs.google.com/document/d/{tempCopyId}/edit";
                await bot.SendMessage(msg.Chat, url);
                break;

            case "/create_contract_4":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>", parseMode: ParseMode.Html);
                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);
                tempCopyId = doc.CopyAndUpdateRelation2(map, DriveFileManager.TemplateContract4, words4);
                url = $"https://docs.google.com/document/d/{tempCopyId}/edit";
                await bot.SendMessage(msg.Chat, url);
                break;

            case "/create_expence_invoice":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>", parseMode: ParseMode.Html);
                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);
                string nameExp = map.GetValueOrDefault("{{����� ��������� ��������}}");
                if (nameExp.Equals("{{����}}")) nameExp = "��������� �������� " + DateTime.Now;
                tempCopyId = doc.CopyAndUpdateRelationSheet2(map, DriveFileManager.TemplateExpenceInvoiceSheetId, DriveFileManager.TemplateExpenceInvoiceSheetGrid, nameExp);
                url = $"https://docs.google.com/spreadsheets/d/{tempCopyId}/edit";
                await bot.SendMessage(msg.Chat, url);
                break;

            case "/create_invoice_for_payment":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>", parseMode: ParseMode.Html);
                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);
                string nameInv = map.GetValueOrDefault("{{����� ������� �� ������}}");
                if (nameInv.Equals("{{����}}")) nameInv = "������� �� ������ " + DateTime.Now;
                tempCopyId = doc.CopyAndUpdateRelationSheet2(map, DriveFileManager.TemplateInvoiceForPaymentSheetId, DriveFileManager.TemplateInvoiceForPaymentSheetGrid, nameInv);
                url = $"https://docs.google.com/spreadsheets/d/{tempCopyId}/edit";
                await bot.SendMessage(msg.Chat, url);
                break;

            case "/create_ttn":
                await bot.SendMessage(msg.Chat, "<i>���������, ���� �����...</i>", parseMode: ParseMode.Html);
                doc = new DriveFileManager();
                map = doc.GetDictionaryDataFromDoc(DriveFileManager.TemplateData);
                prepareMap(map);
                string nameTTN = map.GetValueOrDefault("{{����� ���}}");
                if (nameTTN.Equals("{{����}}")) nameTTN = "��� " + DateTime.Now;
                tempCopyId = doc.CopyAndUpdateRelationSheet2(map, DriveFileManager.TemplateTTNSheetId, DriveFileManager.TemplateTTNSheetGrid, nameTTN);
                url = $"https://docs.google.com/spreadsheets/d/{tempCopyId}/edit";
                await bot.SendMessage(msg.Chat, url);
                break;
        }
    }

    private void prepareMap(Dictionary<string, string> map)
    {
        map["{{day}}"] = DateTime.Now.Day.ToString();
        map["{{month}}"] = DateTime.Now.Month.ToString();
        map["{{year}}"] = DateTime.Now.Year.ToString();
        map["{{month_word}}"] = Dates.GetMonth(DateTime.Now);

        if (map.ContainsKey("{{summ_words}}") && map["{{summ_words}}"] == "{{����}}")
        {
            if (map.ContainsKey("{{�������}}") && map.ContainsKey("{{����}}"))
            {
                double amount = Convert.ToDouble(map["{{�������}}"]);
                double price = Convert.ToDouble(map["{{����}}"].Replace(".", ","));
                map["{{summ_words}}"] = UaDateAndMoneyConverter.CurrencyToTxtFull(amount * price, false);
            }
        }

        map["{{����}}"] = map["{{����}}"].Replace(',', '.');

        if (map.GetValueOrDefault("{{������� �������}}") == "{{����}}")
        {
            map["{{������� �������}}"] = UaDateAndMoneyConverter.NumericalToTxt(Convert.ToDouble(map["{{�������}}"]), false);
        }

        foreach (var k in map.Keys)
        {
            if (k.Contains("[") && map[k] == "{{����}}")
            {
                string mainKey = k.Remove(k.IndexOf("[")) + "}}";
                if (map.ContainsKey(mainKey))
                {
                    string mainValue = map[mainKey].Replace(".", ",");
                    double mainValueDouble = Convert.ToDouble(mainValue);
                    string key2 = k[(k.IndexOf("[") + 1)..].Replace("]", "").Replace("}}", "");

                    if (key2.Equals("�����_�������"))
                    {
                        map[k] = UaDateAndMoneyConverter.CurrencyToTxtFull(mainValueDouble, false);
                    }
                }
            }
        }
    }

    private async Task OnUpdate(Update update)
    {
        switch (update)
        {
            case { CallbackQuery: { } callbackQuery }: await OnCallbackQuery(callbackQuery); break;
            case { PollAnswer: { } pollAnswer }: await OnPollAnswer(pollAnswer); break;
            default: Console.WriteLine($"�������� ���������: {update.Type}"); break;
        };
    }

    async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        await bot.AnswerCallbackQuery(callbackQuery.Id, $"You selected {callbackQuery.Data}");
        await bot.SendMessage(callbackQuery.Message!.Chat, $"Received callback from inline button {callbackQuery.Data}");
    }

    async Task OnPollAnswer(PollAnswer pollAnswer)
    {
        if (pollAnswer.User != null)
            await bot.SendMessage(pollAnswer.User.Id, $"You voted for option(s) id [{string.Join(',', pollAnswer.OptionIds)}]");
    }
}
