using System;
using Microsoft.Win32;
using System.Diagnostics;
using Spire.Pdf;
using System.Drawing.Printing;

namespace TelegramPrinterWPF.Source;
public class DocumentPrinter
{


    public DocumentPrinter()
    {

    }
    public void printWithSpire(string docPath)
    {
        //Load a PDF document
        PdfDocument doc = new PdfDocument(docPath);

        //Specify printer name
        //doc.PrintSettings.PrinterName = "OneNote for Windows 10";

        //Prevent the printer dialog from displaying
        doc.PrintSettings.PrintController = new StandardPrintController();
        doc.PrintSettings.Color = false;

        //Print the document
        doc.Print();
    }

}