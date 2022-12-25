using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramPrinterWPF.Models;
using TelegramPrinterWPF.Source;
using TelegramPrinterWPF.Windows;

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

            if (ConfigurationManager.AppSettings["FirstStartup"]=="true")
            {
                SetupAppForFirstUse();
            }
            InitializeComponent();

            Loaded += MyWindow_Loaded;
            
        }


        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var telegramHelper = new TelegramClient(this);
            Debug.WriteLine($"Start listening for ");
            var me = telegramHelper.BotClient.GetMeAsync();
            Telegram_Logs.Items.Add($"Telegram Bot is Online ID:{me.Result.Id} Name: {me.Result.FirstName} ");
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
            var downloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];
            var file = new DocFile(downloadFolder+"/doc.pdf");
            var user = "Zeeshan";
            PrintWindow printWindow = new PrintWindow(file, user);


            var x = printWindow.ShowDialog();
            if (x == true)
            {
                var NoofCopies = printWindow.NoOfCopies.Text != string.Empty ? short.Parse(printWindow.NoOfCopies.Text) : (short)1;
                var mode =(bool) printWindow.DuplexPrint.IsChecked ? Duplex.Vertical : Duplex.Simplex;
                printed = TestPrinter.printWithSpireWithDailog(file,mode , NoofCopies);
            }
            Debug.WriteLine("passed to the window");

        }
        


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

        private void SetupAppForFirstUse()
        {
            Startup startup = new Startup();
            startup.ShowDialog();
        }
    }
}
