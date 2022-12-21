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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TelegramPrinterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
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
            var TestPrinter = new DocumentPrinter(this);
            bool printed=false;
            PrintWindow printWindow = new PrintWindow();

            var x= printWindow.ShowDialog();
            if(x==true)
            {
                printed = TestPrinter.printWithSpire("./DowloadedFiles/Zeeshanbage_certificate.pdf", printWindow.isDuplexPrint.IsChecked, short.Parse(printWindow.NoOfCopies.Text));
            }
            Debug.WriteLine("passed to the window");

        }
        private void TestPrintButton_Click2(object sender, RoutedEventArgs e)
        {
            var TestPrinter = new DocumentPrinter(this);
            bool printed = false;

            MessageBoxResult result = MessageBox.Show("Yes- For duplex print. No- for 1 side", "Printing the PDF", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                printed = TestPrinter.printWithSpire("C:\\Users\\Zeeshan\\source\\repos\\zeeshanbage\\PrinterApp\\TelegramPrinterWPF\\bin\\Debug\\net7.0-windows\\DowloadedFiles\\Zeeshanbage_decbill.pdf");
            }
            else if (result == MessageBoxResult.No)
            {
                printed = TestPrinter.printWithSpire("./DowloadedFiles/Zeeshanbage_certificate.pdf");
            }
            else
            {
                return;
            }


            if (printed)
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
