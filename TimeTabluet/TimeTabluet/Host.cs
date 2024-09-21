using Telegram.Bot;
using Telegram.Bot.Types;

public class Host
{
    public Action<ITelegramBotClient, Update>? OnMessage;
    private TelegramBotClient _bot;

    public Host(string token)
    {
        _bot = new TelegramBotClient(token);
    }

    public void Start()
    {
        _bot.StartReceiving(updateHandler, ErrorHandler);

        //Рисунок Ascii
        Console.WriteLine(" _______  _                   _______         _      _       \r\n|__   __|(_)                 |__   __|       | |    | |      \r\n   | |    _  _ __ ___    ___    | |     __ _ | |__  | |  ___ \r\n   | |   | || '_ ` _ \\  / _ \\   | |    / _` || '_ \\ | | / _ \\\r\n   | |   | || | | | | ||  __/   | |   | (_| || |_) || ||  __/\r\n   |_|   |_||_| |_| |_| \\___|   |_|    \\__,_||_.__/ |_| \\___|\r\n                                                             \r\n                                                             \r\n");
        Console.WriteLine("Бот Запущен");

    }

    private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine("Ошибка: " + exception.Message);
        await Task.CompletedTask;
    }

    private async Task updateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        Console.WriteLine($"Пришло сообщение: {update.Message?.Text ?? "[Не текст]"}");
        OnMessage?.Invoke(client, update);
        await Task.CompletedTask;
    }
}