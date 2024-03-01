using System;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

public class DataSaver : MonoBehaviour
{
	private static DataSaver _instance;
	public static DataSaver Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<DataSaver>();
				if (_instance == null)
				{
					_instance = new DataSaver();
					_instance.gameObject.name = "DataSaver";
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}

	private const string fileExtension = ".txt";
	private string folderPath;
	private string fileName;
	private string filePath;


	private void Awake()
	{
		// singleton
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}

		// find the document folder depending on the os
		string documentFolderPath = XRSettings.enabled
			? "/storage/emulated/0/Documents/"
			: Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		folderPath = Path.Combine(documentFolderPath, Application.productName, Application.version);
		fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		filePath = Path.Combine(folderPath, fileName + fileExtension);
	}

	public void Log(string message)
	{
		string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		string fullMessage = $"[{timestamp}] {message}";

		DirectoryInfo dirInfo = new(Path.GetDirectoryName(filePath));
		if (!dirInfo.Exists)
			dirInfo.Create();
		using StreamWriter sw = File.Exists(filePath) ? File.AppendText(filePath) : File.CreateText(filePath);
		sw.WriteLine(fullMessage);
	}
}
