using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource cts = new();

            //var BotClient = new TelegramBotClient("5744763753:AAFV98ovmx_LGfFPeFGYXH3zjBOqw-NLTPU");
            var telegramHelper = new TelegramClient();
            var me =  telegramHelper.BotClient.GetMeAsync();
            Debug.WriteLine($"Start listening for ");
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
