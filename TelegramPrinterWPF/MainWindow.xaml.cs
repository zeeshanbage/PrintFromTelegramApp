using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramPrinterWPF.Source;

namespace TelegramPrinterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts;
        public MainWindow()
        {
            cts = new();
            Loaded += MyWindow_Loaded;
            InitializeComponent();

        }
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var telegramHelper = new TelegramClient(this);
            Debug.WriteLine($"Start listening for ");
            Telegram_Logs.Items.Add($"Telegram Bot is Online");
            ToggleBot.Content = "Stop Bot";
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            Telegram_Logs.Background = Brushes.LightGreen;
            telegramHelper.BotClient.StartReceiving(
                updateHandler: telegramHelper.HandleUpdateAsync,
                pollingErrorHandler: telegramHelper.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
        }



        private void TestPrintButton_Click(object sender, RoutedEventArgs e)
        {
            var TestPrinter = new DocumentPrinter();
            var result = TestPrinter.printWithSpire("./DowloadedFiles/Zeeshanbage_certificate.pdf");
            if (result)
            {
                TestPrintButton.Background = Brushes.Green;
            }
            else
            {
                TestPrintButton.Background = Brushes.Red;
            }
        }


        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            TelegramClient.StopTelegramBot(cts);
            ToggleBot.Background = Brushes.LightPink;
            ToggleBot.Content = "Start Bot";
            Telegram_Logs.Items.Add("Telegram Bot is Stopped");
            Telegram_Logs.Background = Brushes.LightPink;


        }
        private void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            var telegramHelper = new TelegramClient(this);
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            Debug.WriteLine($"Bot is online. Start listening for");
            Telegram_Logs.Items.Add($"Telegram Bot is online. Started listening for");
            Telegram_Logs.Background = Brushes.LightBlue;
            ToggleBot.Background = Brushes.Green;
            ToggleBot.Content = "Stop Bot";
            telegramHelper.BotClient.StartReceiving(
                updateHandler: telegramHelper.HandleUpdateAsync,
                pollingErrorHandler: telegramHelper.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
              );
        }

    }
}
