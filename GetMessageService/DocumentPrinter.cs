using System.Drawing;
using System.Drawing.Printing;

namespace GetMessageService;

public class DocumentPrinter
{
    private StreamReader streamToPrint;
    private readonly ILogger<Worker> _logger;
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
                    _logger.LogInformation($"printer name :{pName}");
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
}