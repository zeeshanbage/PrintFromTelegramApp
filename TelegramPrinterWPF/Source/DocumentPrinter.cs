using Spire.Pdf;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TelegramPrinterWPF.Models;

namespace TelegramPrinterWPF.Source;
public class DocumentPrinter
{
    MainWindow _MainWindow;

    public DocumentPrinter(MainWindow mainWindow)
    {
        _MainWindow = mainWindow;
    }
    public bool printWithSpire(DocFile file, bool? duplexMode = false, short NoOfCopies = 1)
    {
        try
        {
            using PdfDocument pdfdocument = new PdfDocument();
            pdfdocument.LoadFromFile(file.Path);
            //pdfdocument.PrintSettings.PrinterName = "OneNote for Windows 10";
            pdfdocument.PrintSettings.Copies = NoOfCopies;

            if (duplexMode == true && pdfdocument.PrintSettings.CanDuplex)
            {
                pdfdocument.PrintSettings.Duplex = Duplex.Vertical;
            }
            _MainWindow.Telegram_Logs.Items.Dispatcher.Invoke(() =>
            {
                _MainWindow.Telegram_Logs.Items.Add($"Printing File: {file.Name} with Printer: {pdfdocument.PrintSettings.PrinterName}");
            });
            pdfdocument.Print();
            pdfdocument.Dispose();
            return true;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show("Error", ex.Message);
            return false;
        }
    }

    public bool printImage(DocFile downloadFile, bool? isChecked, short noofCopies)
    {
        try
        {
            // Create a PrintDialog object
            PrintDialog printDialog = new PrintDialog();

            // Set the image to print
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(downloadFile.Path, UriKind.Absolute));

            // Set the page size and margins
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            image.Measure(pageSize);
            image.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));

            // Print the image
            printDialog.PrintVisual(image, "Print Image");

            return true;
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show("Error", ex.Message);
            return false;
        }
    }
}