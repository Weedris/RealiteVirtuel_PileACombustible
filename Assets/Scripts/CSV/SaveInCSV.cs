/* This file is for saving the trace of the user
 * sooooo yeah that's it
 * 
 * 
 */


using System;
using System.IO;
using System.Text;

public class SaveInCSV
{

    private StringBuilder stringBuilder;
    private string saveFolder;

    public SaveInCSV()
    {
        stringBuilder = new StringBuilder();
        saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CSV_PAC");
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
    }

    public void saveIt(string str)
    {
        stringBuilder.AppendLine(str);
    }

    public void save()
    {
        StringBuilder fileBuilder = new StringBuilder();
        fileBuilder.Append(saveFolder)
                   .Append("/Save_")
                   .Append(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"))
                   .Append("_data.csv");
        StreamWriter outStream = System.IO.File.CreateText(fileBuilder.ToString());
        outStream.WriteLine(stringBuilder);
        outStream.Close();
    }

}
