using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TelegramPrinterWPF.Models;

namespace TelegramPrinterWPF
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        DocFile DocFile;
        public PrintWindow(DocFile file)
        {
            DocFile = file;
            InitializeComponent();
            FileName.Content = "File : " + DocFile.Name;
            UserName.Content = "User : " + DocFile.FromUser;
            BitmapImage image = new BitmapImage();

            switch (DocFile.Type)
            {
                case "pdf":
                    image.BeginInit();
                    image.UriSource = new Uri("pack://application:,,,/Static/pdf.png");
                    image.EndInit();
                    break;
                case "jpg":
                case "jpeg":
                case "png":
                    image.BeginInit();
                    image.UriSource = new Uri("pack://application:,,,/Static/jpg.png");
                    image.EndInit();
                    break;
                case "docx":
                    image.BeginInit();
                    image.UriSource = new Uri("pack://application:,,,/Static/docx.png");
                    image.EndInit();
                    break;
                default:
                    image.BeginInit();
                    image.UriSource = new Uri("pack://application:,,,/Static/file.png");
                    image.EndInit();
                    break;
            }
            Thumbnail.Source = image;
            if(DocFile.Type=="docx" || DocFile.Type== "jpeg" || DocFile.Type=="png")
            {
                Print.Visibility= Visibility.Hidden;   
            }
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Hide();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Hide();
            Close();
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var root = ConfigurationManager.AppSettings["DownloadFolder"];
            System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", root);

        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(DocFile.Path)
            {
                UseShellExecute = true
            };
            p.Start();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            new ToastContentBuilder()
                .AddText($"{UserName.Content} Sent a File {DocFile.Name}")
                .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
        }
    }
}
