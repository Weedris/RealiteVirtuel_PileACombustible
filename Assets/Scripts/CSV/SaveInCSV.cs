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
        else if (osType.Contains("Android"))
        {
            saveFolder = "/storage/emulated/0/Documents/";
        }
        else if (osType.Contains("Mac"))
        {
            saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Documents");
        }
    }

    public void saveIt(string str)
    {
        stringBuilder.AppendLine(str);
    }

    public void save()
    {
        if (saveFolder == null)
        {
            saveFolder = UnityEngine.Application.persistentDataPath;
        }
        
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
        UnityEngine.Debug.Log(saveFolder);
        string filePath = Path.Combine(saveFolder, "CSV_PAC");
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        UnityEngine.Debug.Log(filePath);
        StringBuilder fileBuilder = new();
        fileBuilder.Append("Save_")
                   .Append(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"))
                   .Append("_data.csv");
        filePath = Path.Combine(filePath, fileBuilder.ToString());
        UnityEngine.Debug.Log(filePath);
        File.WriteAllText(filePath, stringBuilder.ToString());
    }
}
