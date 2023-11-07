/*
 * This file is for saving the trace of the user
 */


using System;
using System.IO;
using System.Text;

public class SaveInCSV
{
    private string saveFolder = null;
    private DateTime date;

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
        date = DateTime.Now;
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

    public void save(StringBuilder data)
    {   
        StringBuilder fileBuilder = new();
        fileBuilder.Append("Save_")
                   .Append(date.ToString("yyyy-MM-dd_HH-mm-ss"))
                   .Append("_data.csv");
        string filePath = Path.Combine(checkDirectories(), fileBuilder.ToString());
        File.WriteAllText(filePath, data.ToString());
    }

    public void saveNbGrabObject(StringBuilder nbGrabObjects)
    {
        StringBuilder fileBuilder = new();
        fileBuilder.Append("Save_")
                   .Append(date.ToString("yyyy-MM-dd_HH-mm-ss"))
                   .Append("_nbGrabObjects.csv");
        string filePath = Path.Combine(checkDirectories(), fileBuilder.ToString());
        File.WriteAllText(filePath, nbGrabObjects.ToString());
    }
}
