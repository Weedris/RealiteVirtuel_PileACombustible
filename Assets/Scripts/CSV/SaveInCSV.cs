/*
 * This file is for saving the trace of the user
 */


using System;
using System.IO;
using System.Text;

public class SaveInCSV
{
    private StringBuilder stringBuilder;
    private string saveFolder = null;

    public SaveInCSV()
    {
        stringBuilder = new();
        string osType = Environment.OSVersion.VersionString;

        if (osType.Contains("Windows"))
        {
            saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        else
        {
            saveFolder = "/storage/emulated/0/Documents/";
        }
    }

    public void saveIt(string str)
    {
        stringBuilder.AppendLine(str);
    }

    public void save()
    {   
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
        string filePath = Path.Combine(saveFolder, "CSV_PAC");
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        StringBuilder fileBuilder = new();
        fileBuilder.Append("Save_")
                   .Append(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"))
                   .Append("_data.csv");
        filePath = Path.Combine(filePath, fileBuilder.ToString());
        File.WriteAllText(filePath, stringBuilder.ToString());
    }
}
