using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramPrinterWPF.Models
{
    static class AppConstants
    {
        public static bool ReadyToPrint = true;
        public const string FileDownloaded = "file downloaded ";
        public static string DownloadFolder = ConfigurationManager.AppSettings["DownloadFolder"]?? "D:\\";

    }
}
