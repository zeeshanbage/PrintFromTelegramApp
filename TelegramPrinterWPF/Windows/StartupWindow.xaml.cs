using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TelegramPrinterWPF.Windows
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        public Startup()
        {
            InitializeComponent();
        }

        private void TextBox_TouchEnter(object sender, TouchEventArgs e)
        {
            ConfigurationManager.AppSettings.Set("TelegramBotToken",BotToken.Text);
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings.Add("DownloadFolder", dialog.SelectedPath);
                config.AppSettings.Settings["FirstStartup"].Value = "false";
                config.AppSettings.Settings.Add("TelegramBotToken", BotToken.Text);
                config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");

                SelectedFolder.Content= dialog.SelectedPath;
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            Close();
            return;
        }
    }
}
