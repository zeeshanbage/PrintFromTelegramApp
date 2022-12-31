using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramPrinterWPF.Logic;
using TelegramPrinterWPF.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramPrinterWPF.Source
{
    internal class TelegramClient
    {
        public readonly TelegramBotClient BotClient;
        private DocumentPrinter _documentPrinter;
        private MainWindow MainWindow;
        private Queue<DocFile> FileForProccess= new Queue<DocFile>();
        public TelegramClient(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            BotClient = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramBotToken"]);
            _documentPrinter = new DocumentPrinter(mainWindow);
          
        }

        private async Task SendMessage(Message message, string messageText, ITelegramBotClient botClient,
            CancellationToken cancellationToken)
        {
            
                await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: messageText, parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

        private async Task<DocFile> DownloadFile(Message message, ITelegramBotClient botClient)
        {
            Debug.WriteLine(JsonSerializer.Serialize(message.Document));
            Telegram.Bot.Types.File file;
            string filename;
            if (message.Type == MessageType.Photo)
            {
                file = await botClient.GetFileAsync(message.Photo[message.Photo.Length - 1].FileId);
                filename = file.FilePath.Split('/').Last();
            }
            else
            {
                file = await botClient.GetFileAsync(message.Document.FileId);
                filename = message.Document.FileName;

            }
            string pattern = @"\s+";
            string filenameTrim = Regex.Replace(filename, pattern, "");
            var DownloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];
            var filepath = Path.Combine(DownloadFolder, message.From?.Username + "_" + filenameTrim);
            var fs = new FileStream(filepath, FileMode.Create);
            if (file.FilePath != null) await botClient.DownloadFileAsync(file.FilePath, fs);
            Debug.WriteLine($"{AppConstants.FileDownloaded} {filenameTrim} filepath {filepath}");
            MainWindow.Dispatcher?.Invoke(() =>
            MainWindow.Telegram_Logs.Items.Add($"{AppConstants.FileDownloaded} {filenameTrim} path {filepath}"));
            fs.Close();
            await fs.DisposeAsync();
            return new DocFile(filepath, message.From?.Username);
        }





        public async Task GetMessagesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (message.Type == MessageType.Document || message.Type == MessageType.Photo)
            {

                var downloadFile = await DownloadFile(message, botClient);
                App.Current.Dispatcher.Invoke((System.Action)delegate
                {
                    MainWindow.DownloadedFiles.Add(downloadFile);
                });
                LocalStorage.WriteToJsonFile("DownloadedFiles.json", downloadFile);
                FileForProccess.Enqueue(downloadFile);
                if (AppConstants.ReadyToPrint)
                {
                    Task.Run(() => HandlePrint(message, botClient, cancellationToken));
                }

                if (message.Text is not null)
                {
                    var messageText = $"*Hello, {message.Chat.FirstName} {message.Chat.LastName}*" +
                                    $"\n*I am {botClient.GetMeAsync().Result.FirstName}*" +
                                    $"\nSend me PDF,SDocx or Image to Take Print.\n\n";

                    await SendMessage(message, messageText, botClient, cancellationToken);
                }
            }
        }

        private async Task HandlePrint(Message message, ITelegramBotClient botClient,CancellationToken cancellationToken)
        {
            while(FileForProccess.Any())
            {
                AppConstants.ReadyToPrint = false;
                var printDoc = FileForProccess.Dequeue();
                _documentPrinter.print(printDoc);
            }
            var messageText = $"* Thank you {message.Chat.FirstName} {message.Chat.LastName}*" +
                                $"\n*Keep Using {botClient.GetMeAsync().Result.FirstName} For Taking Prints*";
            await SendMessage(message, messageText, botClient, cancellationToken);
            AppConstants.ReadyToPrint = true;
        }

        public static void StopTelegramBot(CancellationTokenSource cts)
        {
            cts.Cancel();
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
            });
            return Task.CompletedTask;
        }
    }
}