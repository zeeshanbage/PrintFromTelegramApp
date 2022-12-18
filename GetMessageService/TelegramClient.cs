using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace GetMessageService
{
    internal class TelegramClient
    {
        private readonly ILogger<Worker> _logger;
        private readonly TelegramBotClient BotClient;
        private DocumentPrinter _documentPrinter;
        public TelegramClient(ILogger<Worker> logger)
        {
            _logger = logger;
            BotClient = new TelegramBotClient("5744763753:AAFV98ovmx_LGfFPeFGYXH3zjBOqw-NLTPU");
        }
        public async Task TelegramBotClientAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource cts = new();

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            BotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken
            );
            var me = await BotClient.GetMeAsync();
            _logger.LogInformation($"Start listening for @{me.Username}");
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
                    _documentPrinter = new DocumentPrinter();
                    _documentPrinter.print(filePath);
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

                _logger.LogInformation(ErrorMessage);
                return Task.CompletedTask;
            }
        }

        private async Task SendMessage(Message message, ITelegramBotClient botClient,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                $"Received a '{message.Text}' message in chat {message.Chat.Id} from {message.Chat.Username}.");
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Thank you '{message.Chat.FirstName}' for messaging your message is {message.Text}.",
                cancellationToken: cancellationToken);
        }

        private async Task<string> DownloadFile(Message message, ITelegramBotClient botClient)
        {
            _logger.LogInformation(JsonSerializer.Serialize(message.Document));
            var file = await botClient.GetFileAsync(message.Document.FileId);
            var filepath = "./DowloadedFiles/" + message.From?.Username + "_" + message.Document.FileName;
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            var fs = new FileStream(filepath, FileMode.Create);
            if (file.FilePath != null) await botClient.DownloadFileAsync(file.FilePath, fs);
            _logger.LogInformation($"file downloaded {message.Document.FileName} path {filepath}");
            fs.Close();
            await fs.DisposeAsync();
            return filepath;
        }
    }
}