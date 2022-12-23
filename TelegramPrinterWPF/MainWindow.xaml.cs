using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramPrinterWPF.Models;
using TelegramPrinterWPF.Source;

namespace TelegramPrinterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        CancellationTokenSource cts;
        NotifyIcon notifyIcon;
        public MainWindow()
        {
            cts = new();
            Loaded += MyWindow_Loaded;
            InitializeComponent();
            notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;
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
            bool printed = false;
            var file = new DocFile("./DowloadedFiles/Zeeshanbage_certificate.pdf");
            var user = "Zeeshan";
            PrintWindow printWindow = new PrintWindow(file, user);


            var x = printWindow.ShowDialog();
            if (x == true)
            {
                var NoofCopies = printWindow.NoOfCopies.Text != string.Empty ? short.Parse(printWindow.NoOfCopies.Text) : (short)1;

                printed = TestPrinter.printWithSpire(file, printWindow.DuplexPrint.IsChecked, NoofCopies);
            }
            Debug.WriteLine("passed to the window");

        }
        //private void TestPrintButton_Click2(object sender, RoutedEventArgs e)
        //{
        //    var TestPrinter = new DocumentPrinter(this);
        //    bool printed = false;

        //    MessageBoxResult result = MessageBox.Show("Yes- For duplex print. No- for 1 side", "Printing the PDF", MessageBoxButton.YesNoCancel);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        printed = TestPrinter.printWithSpire("C:\\Users\\Zeeshan\\source\\repos\\zeeshanbage\\PrinterApp\\TelegramPrinterWPF\\bin\\Debug\\net7.0-windows\\DowloadedFiles\\Zeeshanbage_decbill.pdf");
        //    }
        //    else if (result == MessageBoxResult.No)
        //    {
        //        printed = TestPrinter.printWithSpire("./DowloadedFiles/Zeeshanbage_certificate.pdf");
        //    }
        //    else
        //    {
        //        return;
        //    }


        //    if (printed)
        //    {
        //        TestPrintButton.Background = Brushes.Green;
        //    }
        //    else
        //    {
        //        TestPrintButton.Background = Brushes.Red;
        //    }
        //}


        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            TelegramClient.StopTelegramBot(cts);
            ToggleBot.Background = Brushes.DarkRed;
            ToggleBot.Content = "Start Bot";
            Telegram_Logs.Items.Add("Telegram Bot is Stopped, Open App again to start");
            Telegram_Logs.Background = Brushes.LightPink;
            //ToggleBot.Visibility = Visibility.Hidden;


        }
        private void HandleUnchecked(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
        }

    }
}
