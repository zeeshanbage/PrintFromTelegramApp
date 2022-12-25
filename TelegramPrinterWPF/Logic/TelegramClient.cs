using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramPrinterWPF.Models;
using Message = Telegram.Bot.Types.Message;

namespace TelegramPrinterWPF.Source
{
    internal class TelegramClient
    {
        public readonly TelegramBotClient BotClient;
        private DocumentPrinter _documentPrinter;
        private MainWindow MainWindow;

        public TelegramClient(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            BotClient = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramBotToken"]);
        }
  

        public static void StopTelegramBot(CancellationTokenSource cts)
        {
            cts.Cancel();
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;

            if (message.Type == MessageType.Document || message.Type== MessageType.Photo)
            {

                var downloadFile = new DocFile(await DownloadFile(message, botClient));



                //Showing Notification 


                //showing Print Window
                _documentPrinter = new DocumentPrinter(MainWindow);
                var userName = message.Chat.FirstName + " " + message.Chat.LastName;
                var printresult = PrintDocument(downloadFile, userName);

            }
            // Echo if received message text
            if (message.Text is not null)
            {
                await SendMessage(message, botClient, cancellationToken);
            }
        }

        private bool? PrintDocument(DocFile downloadFile, string userName)
        {
            bool? takePrint = false;
            PrintWindow printWindow;
            bool printResult = false;
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                printWindow = new PrintWindow(downloadFile, userName);
                takePrint = printWindow.ShowDialog();
                if (takePrint == true)
                {
                    var NoofCopies = printWindow.NoOfCopies.Text != string.Empty ? short.Parse(printWindow.NoOfCopies.Text) : (short)1;
                    switch (downloadFile.Type)
                    {
                        case "pdf":
                            printResult= _documentPrinter.printWithSpire(downloadFile, printWindow.DuplexPrint.IsChecked, NoofCopies);
                            break;
                        case "jpj":
                        case "png":
                            printResult= _documentPrinter.printImage(downloadFile, printWindow.DuplexPrint.IsChecked, NoofCopies);
                            break;
                    }
                }
                Debug.WriteLine("Returned to the Main window");
            });
            return printResult;
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
                MainWindow.Telegram_Logs.Items.Add($"Received a '{message.Text}' message from {message.Chat.FirstName}.");
            });


            var messageText = $"*Hello, {message.Chat.FirstName}*" +
                                $"\n*I am {botClient.GetMeAsync().Result.FirstName}*" +
                                $"\nSend me PDF or Image to Take Print.\n\n";

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: messageText, parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

        private async Task<string> DownloadFile(Message message, ITelegramBotClient botClient)
        {
            Debug.WriteLine(JsonSerializer.Serialize(message.Document));
            Telegram.Bot.Types.File file;
            string filename;
            if(message.Type == MessageType.Photo)
            {
                file = await botClient.GetFileAsync(message.Photo[0].FileId);
                filename = message.Photo[0].FileId + ".jpg";
            }
            else
            {
                file = await botClient.GetFileAsync(message.Document.FileId);
                filename = message.Document.FileName;

            }
            var filepath = ConfigurationManager.AppSettings["DownloadFolder"] + message.From?.Username + "_" + filename;

            var fs = new FileStream(filepath, FileMode.Create);
            if (file.FilePath != null) await botClient.DownloadFileAsync(file.FilePath, fs);
            Debug.WriteLine($"file downloaded {filename} path {filepath}");
            MainWindow.Dispatcher?.Invoke(() =>
            MainWindow.Telegram_Logs.Items.Add($"file downloaded {filename} path {filepath}")
            );
            fs.Close();
            await fs.DisposeAsync();
            return filepath;
        }

        public async Task startBotAsync(CancellationTokenSource cts)
        {
            Debug.WriteLine($"Start listening for ");
            MainWindow.Telegram_Logs.Items.Add($"Telegram Bot is Online");

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            var me = await BotClient.GetMeAsync();
            Debug.WriteLine($"Bot is online. Start listening for @{me.Username}");
            MainWindow.Telegram_Logs.Items.Add($"Telegram Bot is online. Started listening for @{me.Username}");
            BotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
                );



        }
    }
}