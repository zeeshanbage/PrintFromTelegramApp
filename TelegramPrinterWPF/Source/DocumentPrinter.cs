using System;
using System.Diagnostics;
using System.Windows.Forms;
using System;
using ceTe.DynamicPDF.Printing;
using Spire.Pdf;
using IronPdf;

namespace TelegramPrinterWPF.Source;
public class DocumentPrinter
{


    public DocumentPrinter()
    {

    }
    public bool printWithSpire(string docPath)
    {
        try
        {
            //Load a PDF document
            using Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument(docPath);

            //Specify printer name
            doc.PrintSettings.PrinterName = "Brother DCP-L2540DW series";
            doc.PrintSettings.SelectSinglePageLayout(Spire.Pdf.Print.PdfSinglePageScalingMode.ActualSize);

            //Prevent the printer dialog from displaying
            //doc.PrintSettings.PrintController = new StandardPrintController();
            //doc.PrintSettings.Color = false;

            //Print the document
            doc.Print();
            doc.Dispose();
            return true;
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show(ex.Message);
            return false;
        }
    }
    public bool printWithIronPdf(string docPath)
    {
        try
        {
            //Load a PDF document

            IronPdf.ChromePdfRenderer Renderer = new IronPdf.ChromePdfRenderer();

            using IronPdf.PdfDocument Pdf = new IronPdf.PdfDocument(docPath);
            // Send the PDF to the default printer to print
            Pdf.Print();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public bool printWithDynamicPdf(string docPath,int mode=1)
    {
        try
        {
            // Create a print job with the document to be printed to the default printer
            PrintJob printJob = new PrintJob(Printer.Default, docPath);
            if(mode==2)
            {
                printJob.PrintOptions.DuplexMode = DuplexMode.DuplexVertical;
            }
            
            // Print the print job
            printJob.Print();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            MessageBox.Show(ex.Message);
            return false;
        }
    }
}