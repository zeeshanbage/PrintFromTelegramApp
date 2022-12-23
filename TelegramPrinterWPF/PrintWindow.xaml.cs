using System;
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

        public PrintWindow(Models.DocFile downloadFile)
        {
            InitializeComponent();
        }
        public PrintWindow(DocFile file, string user)
        {
            InitializeComponent();
            FileName.Content = "File : " + file.Name;
            UserName.Content = "User : " + user;
            BitmapImage image = new BitmapImage();
            switch (file.Type)
            {
                case "pdf":
                    image.BeginInit();
                    image.UriSource = new Uri("pack://application:,,,/Static/pdf.png");
                    image.EndInit();
                    break;
                default:
                    image.BeginInit();
                    image.UriSource = new Uri("pack://application:,,,/Static/file.png");
                    image.EndInit();
                    break;
            }

            Thumbnail.Source = image;
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

    }
}
