using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace TelegramConsole
{
    internal class TelegramClient
    {
        public static async Task TelegramBotClientAsync()
        {
            var botClient = new TelegramBotClient("5744763753:AAFV98ovmx_LGfFPeFGYXH3zjBOqw-NLTPU");
            CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            // Send cancellation request to stop bot
            cts.Cancel();


            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
                CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                if (message.Document is not null)
                {
                    var filePath = await DownloadFile(message, botClient);
                    //new PrintingExample(filePath);
                }

                // Echo if received message text
                if (message.Text is not null)
                {
                    await SendMessage(message, botClient, cancellationToken);
                }
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
                CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
        }

        private static async Task SendMessage(Message message, ITelegramBotClient botClient,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(
                $"Received a '{message.Text}' message in chat {message.Chat.Id} from {message.Chat.Username}.");
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Thank you '{message.Chat.FirstName}' for messaging.",
                cancellationToken: cancellationToken);
        }

        private static async Task<string> DownloadFile(Message message, ITelegramBotClient botClient)
        {
            Console.WriteLine(JsonSerializer.Serialize(message.Document));
            var file = await botClient.GetFileAsync(message.Document.FileId);
            var filepath = "./DowloadedFiles/" + message.From?.Username + "_" + message.Document.FileName;
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            var fs = new FileStream(filepath, FileMode.Create);
            if (file.FilePath != null) await botClient.DownloadFileAsync(file.FilePath, fs);
            Console.WriteLine($"file downloaded {message.Document.FileName}");
            fs.Close();
            await fs.DisposeAsync();
            return filepath;
        }
    }
}