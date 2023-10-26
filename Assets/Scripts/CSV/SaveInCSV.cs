/*
 * This file is for saving the trace of the user
 */


using System;
using System.IO;
using System.Text;

public class SaveInCSV
{
    private StringBuilder stringBuilder;
    private string saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public SaveInCSV()
    {
        stringBuilder = new();
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
        /*
        StreamWriter outStream = System.IO.File.CreateText(fileBuilder.ToString());
        outStream.WriteLine(stringBuilder);
        outStream.Close();
        */
    }
}
