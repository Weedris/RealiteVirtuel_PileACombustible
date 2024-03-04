using System;
using System.IO;


public class DataSaver
{
	private static DataSaver _instance;
	public static DataSaver Instance
	{
		get
		{
			_instance ??= new();
			return _instance;
		}
	}

	private string logFilePath;

	private DataSaver()
	{
		// find the document folder depending on the os (fuck you meta quest)
		string documentFolderPath = UnityEngine.XR.XRSettings.enabled
			? "/storage/emulated/0/Documents/"
			: Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		// retrieve date
		string now = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

		// store the log file path
		logFilePath = Path.Combine(documentFolderPath, UnityEngine.Application.productName, UnityEngine.Application.version, now + ".txt");
	}

	public void Log(string message)
	{
		string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		string fullMessage = $"[{timestamp}] {message}";

		// generate folder recursively if it doesn't exists
		DirectoryInfo dirInfo = new(Path.GetDirectoryName(logFilePath));
		if (!dirInfo.Exists)
			dirInfo.Create();

		// generate file if doesn't exist
		using StreamWriter sw = File.Exists(logFilePath) ? File.AppendText(logFilePath) : File.CreateText(logFilePath);

		// append message to the file
		sw.WriteLine(fullMessage);
	}
}
