
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using Spire.Pdf;
using System.Windows;

namespace TelegramPrinterWPF.Source;
public class DocumentPrinter1
{


    public DocumentPrinter1()
    {

    }


    public bool printFoxitcPdf(string fileName,int mode=1)
    {
        try
        {
            var printerSettings = new PrinterSettings
            {
                PrinterName = "OneNote for Windows 10",
                Copies = (short)1
            };

            MessageBox.Show(printerSettings.CanDuplex.ToString());
            //string strPrintFile = "D:\\Application2.pdf";
            ProcessStartInfo info = new ProcessStartInfo(fileName);
            info.Verb = "Print";
            info.FileName = @"C:\Program Files (x86)\Foxit Software\Foxit PDF Reader\FoxitPDFReader.exe";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            //fs.Close();
            Process.Start(info);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show(ex.Message);
            return false;
        }
    }
    public bool PrintPDFium(string filepath, int copies=1)
    {
        try
        {
            PdfDocument pdfdocument = new PdfDocument();
            pdfdocument.LoadFromFile(filepath);
            pdfdocument.PrintSettings.PrinterName = "OneNote for Windows 10";
            pdfdocument.PrintSettings.Copies = 2;
            pdfdocument.Print();
            pdfdocument.Dispose();
            return true;
        }
        catch (System.Exception e)
        {
            return false;
        }
    }
}