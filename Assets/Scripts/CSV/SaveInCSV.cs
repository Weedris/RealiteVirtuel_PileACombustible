/*
 * This file is for saving the trace of the user
 */


using System;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveInCSV : MonoBehaviour
{
    private StringBuilder stringBuilder;
    private string saveFolder;

    public SaveInCSV()
    {
        stringBuilder = new();
    }

    public void Awake()
    {
#if UNITY_EDITOR
		saveFolder = Application.dataPath + "/CSV_PAC/";
#elif UNITY_ANDROID
        saveFolder = Application.persistentDataPath+"/CSV_PAC/";
#elif UNITY_IPHONE
        saveFolder = Application.persistentDataPath+"/CSV_PAC/";
#else
        saveFolder = Application.dataPath +"/CSV_PAC/";
#endif
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
        //StringBuilder fileBuilder = new();
        //fileBuilder.Append(saveFolder)
        //           .Append("Save_")
        //           .Append(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"))
        //           .Append("_data.csv");
        //StreamWriter outStream = System.IO.File.CreateText(fileBuilder.ToString());
        //outStream.WriteLine(stringBuilder);
        //outStream.Close();
    }
}
