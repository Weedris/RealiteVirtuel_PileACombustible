/*
 * This file is for saving the trace of the user
 */


using System;
using System.IO;
using System.Text;

public class SaveInCSV
{
	private readonly string saveFolder = null;
	private readonly string date;

	public SaveInCSV()
	{
		date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

		string osType = Environment.OSVersion.VersionString;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#elif UNITY_ANDROID
		saveFolder = "/storage/emulated/0/Documents/";
#endif
	}

	private string CheckDirectories()
	{
		if (!Directory.Exists(saveFolder))
			Directory.CreateDirectory(saveFolder);

		string filePath = Path.Combine(saveFolder, "CSV_PAC");  // folder not path

		if (!Directory.Exists(filePath))
			Directory.CreateDirectory(filePath);

		return filePath;
	}

	public void Save(StringBuilder data, string name = "data")
	{
		StringBuilder fileBuilder = new();
		fileBuilder.Append("Save_")
				   .Append(date)
				   .Append("_")
				   .Append(name)
				   .Append(".csv");
		WriteFile(Path.Combine(CheckDirectories(), fileBuilder.ToString()), data.ToString());
		/*File.WriteAllText(filePath, data.ToString());*/
	}

	private void WriteFile(string filePath, string text)
	{
		using StreamWriter sw = new StreamWriter(filePath, true);
		sw.WriteLine(text);
		sw.Close();
	}
}
