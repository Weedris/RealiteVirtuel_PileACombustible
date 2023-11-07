/*
 * This file is for saving the trace of the user
 */


using System;
using System.IO;
using System.Text;

public class SaveInCSV
{
    private string saveFolder = null;
    private string date;

    public SaveInCSV()
    {
        string osType = Environment.OSVersion.VersionString;

        if (osType.Contains("Windows"))
        {
            saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        else
        {
            saveFolder = "/storage/emulated/0/Documents/";
        }
        date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
    }

    private string checkDirectories()
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
        return filePath;
    }

    public void save(StringBuilder data, string name = "data")
    {   
        StringBuilder fileBuilder = new();
        fileBuilder.Append("Save_")
                   .Append(date)
                   .Append("_")
                   .Append(name)
                   .Append(".csv");
        writeFile(Path.Combine(checkDirectories(), fileBuilder.ToString()), data.ToString());
        /*File.WriteAllText(filePath, data.ToString());*/
    }

    private void writeFile(string filePath, string text)
    {
        using StreamWriter sw = new StreamWriter(filePath, true);
        sw.WriteLine(text);
        sw.Close();
    }
}
