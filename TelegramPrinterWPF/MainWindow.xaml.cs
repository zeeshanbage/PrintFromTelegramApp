using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
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
        public MainWindow()
        {
            Loaded += MyWindow_Loaded;
            InitializeComponent();

        }
        public void AddToTelegramConsole(string item)
        {
            Telegram_Logs.Items.Add(item);
        }
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource cts = new();

            var telegramHelper = new TelegramClient(this);
            Debug.WriteLine($"Start listening for ");
            Telegram_Logs.Items.Add($"Telegram Bot is Online");

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            telegramHelper.BotClient.StartReceiving(
                updateHandler: telegramHelper.HandleUpdateAsync,
                pollingErrorHandler: telegramHelper.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
        }

    }
}
