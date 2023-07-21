/* This file is for saving the trace of the user
 * sooooo yeah that's it
 * 
 * 
 */



using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using TMPro;

public class SaveInCSV : MonoBehaviour
{

    private List<string> data = new List<string>();

    public void saveIt(string str)
    {
        data.Add(str);
    }

    public void sauvegarde()
    {
        StringBuilder sbtrue = new StringBuilder();

        for (int i = 0; i < data.Count; i ++) 
        {
            sbtrue.AppendLine(data[i]);
        }

        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sbtrue);
        outStream.Close();
    }

    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "Saved_data.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath +"/"+"Saved_data.csv";
#endif
    }

}
