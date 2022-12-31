using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramPrinterWPF.Models;
using TelegramPrinterWPF.Source;
using TelegramPrinterWPF.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;

namespace TelegramPrinterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string FileDownloaded = "file downloaded ";
        public ObservableCollection<DocFile> DownloadedFiles = new ObservableCollection<DocFile>();
        CancellationTokenSource cts;
        DocumentPrinter documentPrinter;
        public MainWindow()
        {
            cts = new(); 

            if (ConfigurationManager.AppSettings["FirstStartup"]=="true")
            {
                SetupAppForFirstUse();
            }
            InitializeComponent();
            documentPrinter = new DocumentPrinter(this);
            Loaded += MyWindow_Loaded;
            ItemList.ItemsSource = DownloadedFiles;
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
                updateHandler: telegramHelper.GetMessagesAsync,
                pollingErrorHandler: telegramHelper.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
        }



        private void TestPrintButton_Click(object sender, RoutedEventArgs e)
        {
            
            var TestPrinter = new DocumentPrinter(this);
            bool? printed;
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                var file = new DocFile(openFileDlg.FileName,"Zeeshan");
                printed = TestPrinter.print(file);
            }
            
            Debug.WriteLine("Control passed to the main window");

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

        private void Telegram_Logs_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Telegram_Logs.SelectedItem != null && Telegram_Logs.SelectedItem.ToString().StartsWith(FileDownloaded))
            {
                var item = Telegram_Logs.SelectedItem.ToString().Split(' ');
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(item[5])
                {
                    UseShellExecute = true
                };
                p.Start();
            }
        }

        private void ItemList_Print(object sender, RoutedEventArgs e)
        {
         
        }

        private void MenuItemPrint_Click(object sender, RoutedEventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
            {
                return;
            }
            var filePath = ItemList.SelectedItem as DocFile;
            if(filePath != null)
            {
                documentPrinter.print(filePath);
            }
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            if (ItemList.SelectedIndex == -1)
            {
                return;
            }
            var file = ItemList.SelectedItem as DocFile;
            if(file != null)
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(file.Path)
                {
                    UseShellExecute = true
                };
                p.Start();
            }
        }
    }
}
