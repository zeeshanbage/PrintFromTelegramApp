using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
namespace TelegramPrinterWPF.Source;
public class DocumentPrinter : IDisposable
{
    private StreamReader streamToPrint;

    public DocumentPrinter()
    {

    }
    public void print(string docPath)
    {
        try
        {
            streamToPrint = new StreamReader(docPath);
            try
            {
                foreach (string pName in PrinterSettings.InstalledPrinters) {

                }
                PrintDocument pd = new PrintDocument();
                pd.Print();
            }
            finally
            {
                streamToPrint.Close();
            }
        }
        catch (Exception ex)
        {

        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}