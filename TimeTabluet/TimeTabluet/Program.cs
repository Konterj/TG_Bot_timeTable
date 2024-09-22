using Telegram.Bot;
using Telegram.Bot.Types;
using FluentDateTime;
using PdfiumViewer;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Drawing;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
internal class Program
{
    private static void Main(string[] args)
    {
        Host g4Bot = new Host("7632555773:AAEUAhxvjkbXHJuE51LgVK2Lj8D7zzanky4");
        g4Bot.Start();
        g4Bot.OnMessage += OnMessage;
        g4Bot.OnMessage += OnSendImage;
        Console.ReadLine();
    }

    private static async void OnSendImage(ITelegramBotClient client, Update update)
    {
        if (update.Message?.Text == "/428")
        {
            // Замена расписания
            string pdfUrl = "https://drive.google.com/uc?export=download&id=1PB8g3ps4esIBH6-N7UH4-2-BKw8OShsc";
            string pdfPath = await DownloadPdfAsync(pdfUrl); // Используем асинхронную версию
            string imagePath = ConvertPdfToImage(pdfPath);

            //Определяем область для обрезки
            Rectangle cropArea = new(0, 0, 545, 209);
            Bitmap croppedImage = CropImage(imagePath, cropArea);
            string croppedImagePath = "C:\\Users\\denzd\\Desktop\\Бот расписание\\TimeTabluet\\TimeTabluet\\Images\\SaveConvertImage\\CroppedImage.png";
            croppedImage.Save(croppedImagePath, ImageFormat.Png);

            using (Stream imageStream = System.IO.File.OpenRead(croppedImagePath))
            {
                await client.SendPhotoAsync(
                    chatId: update.Message.Chat.Id,
                    photo: new InputFileStream(imageStream),
                    caption: "Замена Расписании\nОбычно обновляется в 17:00\n");
            }

            // Отправка изображения в зависимости от дня недели
            await SendDailyImage(client, update.Message.Chat.Id);
        }
    }


    private static async Task<string> DownloadPdfAsync(string url)
    {
        string path = "Images\\SavePdf\\file.pdf";

        using (var client = new WebClient())
        {
            await client.DownloadFileTaskAsync(new Uri(url), path); // Ждем завершения загрузки
        }

        return path; // Возвращаем путь к загруженному файлу
    }

    private static string ConvertPdfToImage(string pdfPath)
    {
        string imagePath = "C:\\Users\\denzd\\Desktop\\Бот расписание\\TimeTabluet\\TimeTabluet\\Images\\SaveConvertImage\\image.png"; // Путь для сохранения изображения

        using (var document = PdfiumViewer.PdfDocument.Load(pdfPath))
        {
            int pageNumber = 0;
            using (var image = document.Render(pageNumber, 2048, 2048, PdfRenderFlags.ForPrinting))
            {
                image.Save(imagePath, ImageFormat.Png); // Сохраняем изображение
            }
        }

        return imagePath; // Возвращаем путь к изображению
    }


    static Bitmap CropImage(string imagePath, Rectangle cropArea)
    {
        using (var originalImage = new Bitmap(imagePath))
        {
            // Создаем новое изображение с нужными размерами
            var croppedImage = new Bitmap(cropArea.Width, cropArea.Height);

            using (var graphics = Graphics.FromImage(croppedImage))
            {
                // Рисуем оригинальное изображение в область обрезки
                graphics.DrawImage(originalImage, new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
                                   cropArea, GraphicsUnit.Pixel);
            }

            return croppedImage;
        }
    }


    private static async Task SendDailyImage(ITelegramBotClient client, long chatId)
    {
        string imagePath;
        switch (DateTime.Now.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                imagePath = "Images/Monday.png";
                break;
            case DayOfWeek.Monday:
                imagePath = "Images/Tuesday.png";
                break;
            case DayOfWeek.Tuesday:
                imagePath = "Images/Wednesday.png";
                break;
            case DayOfWeek.Wednesday:
                imagePath = "Images/Thursday.png";
                break;
            case DayOfWeek.Thursday:
                imagePath = "Images/Friday.png";
                break;
            default:
                imagePath = "Images/Monday.png"; // По умолчанию будет Понедельник
                break;
        }

        using (Stream imageStream = System.IO.File.OpenRead(imagePath))
        {
            await client.SendPhotoAsync(
                chatId: chatId,
                photo: new InputFileStream(imageStream),
                caption: $"Дата: {DateTime.Today:dddd} {DateTime.Today:d}");
        }
    }

    private static async void OnMessage(ITelegramBotClient client, Update update)
    {
        ChatId id = "7632555773";

        if (update.Message?.Text == "/start")
        {
            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 7632555773, "Добро пожаловать в расписание, команды \n/info - информация\n /428 - расписание", replyToMessageId: update.Message?.MessageId);
        }
        else if (update.Message?.Text == "/info")
        {
            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 7632555773, "Данный бот был создан для\nавтоматизации расписания, на данный момент реализован весь функционал который хотел разработчик\nGithub: Konterj\nNickName: Konterj\nРазработчик: Денчик\nЕсли у вас есть предложение что улучшить и что добавить\nприсылайте на почту: den.zdanovich09@list.ru");
        }
        else if(update.Message?.Text == "/428")
        {
            //Null for cheked 
        }
        else
        {
            await client.SendTextMessageAsync(update.Message?.Chat.Id ?? 7632555773, "Извините но это не команды\nкоманды\n/info - информация\n/428 - расписание");
        }
    }
}
