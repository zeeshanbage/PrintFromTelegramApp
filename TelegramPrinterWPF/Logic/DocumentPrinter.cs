using Spire.Pdf;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using TelegramPrinterWPF.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using PrintDialog = System.Windows.Forms.PrintDialog;
using DocumentFormat.OpenXml.Packaging;

namespace TelegramPrinterWPF.Source;
public class DocumentPrinter
{
    MainWindow _MainWindow;

    public DocumentPrinter(MainWindow mainWindow)
    {
        _MainWindow = mainWindow;
    }

    public bool print(DocFile file)
    {

        bool? takePrint = false;
        PrintWindow printWindow;
        bool printResult = false;
        System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
        {
            printWindow = new PrintWindow(file);
            printWindow.Owner = _MainWindow;
            takePrint = printWindow.ShowDialog();
            if (takePrint == true)
            {
                var NoofCopies = printWindow.NoOfCopies.Text != string.Empty ? short.Parse(printWindow.NoOfCopies.Text) : (short)1;
                switch (file.Type)
                {
                    case "pdf":
                        Duplex mode = (bool)printWindow.DuplexPrint.IsChecked ? Duplex.Vertical : Duplex.Simplex;
                        printResult = printWithSpireWithDailog(file, mode, NoofCopies);
                        break;
                    case "jpj":
                    case "jpeg":
                    case "png":
                        printResult = printImage2(file);
                        break;
                    case "docx":
                        printResult = printDocx(file, printWindow.DuplexPrint.IsChecked, NoofCopies);
                        break;
                }
            }
        });
        return printResult;
    }
    public bool printWithSpire(DocFile file, Duplex mode = Duplex.Simplex, short NoOfCopies = 1)
    {
        try
        {
            using PdfDocument pdfdocument = new PdfDocument();
            pdfdocument.LoadFromFile(file.Path);
            //pdfdocument.PrintSettings.PrinterName = "OneNote for Windows 10";
            pdfdocument.PrintSettings.Copies = NoOfCopies;

            pdfdocument.PrintSettings.Duplex = mode;
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
            System.Windows.MessageBox.Show("Error", ex.Message);
            return false;
        }
    }
    public bool printWithSpireWithDailog(DocFile file, Duplex mode=Duplex.Simplex, short NoOfCopies = 1)
    {
        try
        {
            using PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(file.Path);
            //pdfdocument.PrintSettings.PrinterName = "OneNote for Windows 10";
            PrintDialog dialogPrint = new PrintDialog();
            dialogPrint.AllowPrintToFile = true;
            dialogPrint.AllowSomePages = true;
            dialogPrint.PrinterSettings.MinimumPage = 1;
            dialogPrint.PrinterSettings.MaximumPage = doc.Pages.Count;
            dialogPrint.PrinterSettings.FromPage = 1;
            dialogPrint.PrinterSettings.ToPage = doc.Pages.Count;
            dialogPrint.PrinterSettings.Copies = NoOfCopies;
            dialogPrint.PrinterSettings.Duplex = mode;

            if (dialogPrint.ShowDialog() == DialogResult.OK)
            {
                doc.Print();
            }
            return true;

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            System.Windows.MessageBox.Show("Error", ex.Message);
            return false;
        }
    }


    public bool printImage2(DocFile file)
    {

        try
        {
            // Create a PrintDialog object
            System.Windows.Controls.PrintDialog imagePrintDialog = new System.Windows.Controls.PrintDialog();

            // Set the image to print
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(file.Path, UriKind.Absolute));

            // Set the page size and margins
            System.Windows.Size pageSize = new System.Windows.Size(imagePrintDialog.PrintableAreaWidth, imagePrintDialog.PrintableAreaHeight);
            image.Measure(pageSize);
            image.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));

            // Print the image
            //imagePrintDialog.ShowDialog();
            if(imagePrintDialog.ShowDialog() == true)
            {
                imagePrintDialog.PrintVisual(image, "Print Image");
            }
            

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            System.Windows.Forms.MessageBox.Show("Error", ex.Message);
            return false;
        }
    }

    internal bool printDocx(DocFile downloadFile, bool? isChecked, short noofCopies)
    {
        using (WordprocessingDocument doc = WordprocessingDocument.Open(downloadFile.Path, true))
        {
            PrintDocument printDoc = new PrintDocument();
            PrintDialog dialogPrint = new PrintDialog();
            if (dialogPrint.ShowDialog() == DialogResult.OK)
            {
                printDoc.PrinterSettings= dialogPrint.PrinterSettings;
                printDoc.Print();
            }
        }
        return true;
    }
}