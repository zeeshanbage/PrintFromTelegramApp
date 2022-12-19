using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Telegram.Bot.Types.Message;

namespace TelegramPrinterWPF.Source
{
    internal class TelegramClient
    {
        public readonly TelegramBotClient BotClient;
        private DocumentPrinter _documentPrinter;
        private MainWindow MainWindow;
        public TelegramClient()
        {
            BotClient = new TelegramBotClient("5744763753:AAFV98ovmx_LGfFPeFGYXH3zjBOqw-NLTPU");
        }
        public TelegramClient(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
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
            Debug.WriteLine($"Start listening for @{me.Username}");

        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            if (message.Document is not null)
            {
                var filePath = await DownloadFile(message, botClient);
                _documentPrinter = new DocumentPrinter();
                _documentPrinter.printWithSpire(filePath);
            }
            // Echo if received message text
            if (message.Text is not null)
            {
                await SendMessage(message, botClient, cancellationToken);
            }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Debug.WriteLine(ErrorMessage);
            MainWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.Telegram_Logs.Items.Add(ErrorMessage);
                MainWindow.Close();
            });
            return Task.CompletedTask;
        }
        private async Task SendMessage(Message message, ITelegramBotClient botClient,
            CancellationToken cancellationToken)
        {
            Debug.WriteLine(
                $"Received a '{message.Text}' message in chat {message.Chat.Id} from {message.Chat.Username}.");
            MainWindow.Dispatcher.Invoke(() =>
            {
                MainWindow.Telegram_Logs.Items.Add($"Received a '{message.Text}' message from {message.Chat.Username}.");
            });
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Thank you '{message.Chat.FirstName}' for messaging your message is {message.Text}.",
                cancellationToken: cancellationToken);
        }

        private async Task<string> DownloadFile(Message message, ITelegramBotClient botClient)
        {
            Debug.WriteLine(JsonSerializer.Serialize(message.Document));
            var file = await botClient.GetFileAsync(message.Document.FileId);
            var filepath = "./DowloadedFiles/" + message.From?.Username + "_" + message.Document.FileName;

            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filepath));
            var fs = new FileStream(filepath, FileMode.Create);
            if (file.FilePath != null) await botClient.DownloadFileAsync(file.FilePath, fs);
            Debug.WriteLine($"file downloaded {message.Document.FileName} path {filepath}");
            MainWindow.Dispatcher?.Invoke(() =>
            MainWindow.Telegram_Logs.Items.Add($"file downloaded {message.Document.FileName} path {filepath}")
            );
            fs.Close();
            await fs.DisposeAsync();
            return filepath;
        }
    }
}