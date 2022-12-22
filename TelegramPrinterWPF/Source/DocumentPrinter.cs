using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Spire.Pdf;
using Telegram.Bot.Types;
using TelegramPrinterWPF.Models;

namespace TelegramPrinterWPF.Source;
public class DocumentPrinter
{
    MainWindow _MainWindow;

    public DocumentPrinter()
    {

    }
    public DocumentPrinter(MainWindow mainWindow)
    {
        _MainWindow = mainWindow;
    }
    public bool printWithSpire(DocFile file,bool? duplexMode=false, short NoOfCopies=1)
    {
        try
        {
            using PdfDocument pdfdocument = new PdfDocument();
            pdfdocument.LoadFromFile(file.Path);
            //pdfdocument.PrintSettings.PrinterName = "OneNote for Windows 10";
            pdfdocument.PrintSettings.Copies = NoOfCopies;

            if (duplexMode==true && pdfdocument.PrintSettings.CanDuplex)
            {
                pdfdocument.PrintSettings.Duplex = Duplex.Vertical;
            }
            _MainWindow.Telegram_Logs.Items.Dispatcher.Invoke(() =>{
                _MainWindow.Telegram_Logs.Items.Add($"Printing File: {file.Name} with Printer: {pdfdocument.PrintSettings.PrinterName}");
            });
            pdfdocument.Print();
            pdfdocument.Dispose();
            return true;

        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show(ex.Message);
            return false;
        }
    }
    //public bool printWithIronPdf(string docPath)
    //{
    //    try
    //    {
    //        //Load a PDF document

    //        IronPdf.ChromePdfRenderer Renderer = new IronPdf.ChromePdfRenderer();

    //        using IronPdf.PdfDocument Pdf = new IronPdf.PdfDocument(docPath);
    //        // Send the PDF to the default printer to print
    //        Pdf.Print();
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        MessageBox.Show(ex.Message);
    //        return false;
    //    }
    //}

    //public bool printWithDynamicPdf(string docPath,int mode=1)
    //{
    //    try
    //    {
    //        // Create a print job with the document to be printed to the default printer
    //        PrintJob printJob = new PrintJob(Printer.Default, docPath);
    //        if(mode==2)
    //        {
    //            printJob.PrintOptions.DuplexMode = DuplexMode.DuplexVertical;
    //        }
            
    //        // Print the print job
    //        printJob.Print();
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex);
    //        MessageBox.Show(ex.Message);
    //        return false;
    //    }
    //}
}