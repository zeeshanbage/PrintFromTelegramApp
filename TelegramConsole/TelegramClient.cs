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


            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                


                var chatId = message.Chat.Id;

                
                if(update.Message.Document is not null)
                {
                    
                    var file = await botClient.GetFileAsync(message.Document.FileId);
                    FileStream fs = new FileStream("D:\\Repos\\PrinterApp\\DowloadedFiles\\"+message.Document.FileName, FileMode.Create);
                    await botClient.DownloadFileAsync(file.FilePath, fs);
                    fs.Close();
                    fs.Dispose();
                }
                

                //new PrintingExample(filePath);



                // Echo received message text
                if(message.Text is not null)
                {
                    Console.WriteLine($"Received a '{message.Text}' message in chat {chatId}.");
                    Message sentMessage = await botClient.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: "Ok Bro " + message.Text,
                                        cancellationToken: cancellationToken);
                }
                
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
    }
}
