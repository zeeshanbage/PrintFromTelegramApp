
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;

namespace TelegramPrinterWPF.Source;
public class DocumentPrinter1
{


    public DocumentPrinter1()
    {

    }


    public bool printWithDynamicPdf(string fileName,int mode=1)
    {
        try
        {
            //var filepath = @"C:\\Users\\Zeeshan\\source\\repos\\zeeshanbage\\PrinterApp\\TelegramPrinterWPF\\bin\\Debug\\net7.0-windows\" + fileName;
            string fileDir1 = @"C:\Program Files (x86)\Foxit Software\Foxit PDF Reader\FoxitPDFReader.exe";
            Process pdfProcess = new Process();
            pdfProcess.StartInfo.FileName = fileDir1;
            pdfProcess.StartInfo.Arguments = string.Format(@"/t {0} {1}", fileName, "pos-80");
            pdfProcess.StartInfo.CreateNoWindow = true;
            pdfProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileDir1);
            pdfProcess.Start();

            if (!pdfProcess.WaitForExit(2500))
            {
                pdfProcess.Kill();

            }
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