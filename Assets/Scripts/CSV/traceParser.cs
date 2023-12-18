/* This file is to make phrase for the save function
 * so us human can understand what this is easly
 * 
 * 
 * 
 */

using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class traceParser : MonoBehaviour
{
	public static traceParser Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private double lastTime;
	private Dictionary<string, int> nbGrabObjectsByState = new Dictionary<string, int>();

	private SaveInCSV saveInCSV = new SaveInCSV();

	private Dictionary<Type, string> cmpntTypeToAction = new Dictionary<Type, string>
	{
		{ typeof(ActionBasedController), ";Grab;" },
		{ typeof(XRGrabInteractable), ";Caught;" },
		{ typeof(XRSocketInteractor), ";PutInSocket;" }
	};

	private void chgNbGrabObjectsByState(GameObject go)
	{
		string name = go.name;
		if (nbGrabObjectsByState.ContainsKey(name))
		{
			nbGrabObjectsByState[name] += 1;
		}
	}

	private void rstNbGrabObjectsByState(GameManager.State state)
	{
		//Save nb grab objects:
		StringBuilder sb = new StringBuilder();
		sb.Append(state.ToString())
		  .Append(";");
		foreach (KeyValuePair<string, int> kvp in nbGrabObjectsByState)
		{
			sb.Append(kvp.Value)
			  .Append(";");
		}
		saveInCSV.Save(sb, "nbGrabObjects");
		//Rst nb grab object to 0:
		foreach (string key in new List<string>(nbGrabObjectsByState.Keys))
		{
			nbGrabObjectsByState[key] = 0;
		}
	}

	private double getTimeSinceStartState()
	{
		double time = Time.realtimeSinceStartup - lastTime;
		return time < 0.5 ? 0 : time;
	}

	private void setLastTime()
	{
		lastTime = Time.realtimeSinceStartup;
	}

	void Start()
	{
		string directoryPath = "Assets/Model/Prefab/Room/PAC_Components";
		if (Directory.Exists(directoryPath))
		{
			string[] fileNames = Directory.GetFiles(directoryPath);
			foreach (string fileName in fileNames)
			{
				if (!fileName.Contains(".meta"))
				{
					string cmpntName = fileName.Replace(directoryPath, "").Replace(".prefab", "").Replace("\\", "");
					nbGrabObjectsByState.Add(cmpntName, 0);
				}
			}
		}
		else
		{
			Debug.LogError("Le répertoire n'existe pas : " + directoryPath);
		}
		StringBuilder sb = new StringBuilder();
		sb.Append(";");
		foreach (string key in new List<string>(nbGrabObjectsByState.Keys))
		{
			sb.Append(key).Append(";");
		}
		saveInCSV.Save(sb, "nbGrabObjects");
		lastTime = Time.realtimeSinceStartup;
	}

	public void traceMainStep(GameManager.State state)
	{
		rstNbGrabObjectsByState(state);
		StringBuilder data = new StringBuilder();
		data.Append("ChangeStep;State;")
			.Append(state.ToString())
			.Append(";")
			.Append(Time.realtimeSinceStartup)
			.Append(";")
			.Append(getTimeSinceStartState());
		saveInCSV.Save(data);
		setLastTime();
	}

	public void traceSocket(XRSocketInteractor si, string str)
	{
		StringBuilder data = new StringBuilder();
		data.Append("Receive;")
			.Append(str)
			.Append(";")
			.Append(si.name)
			.Append(";")
			.Append(Time.realtimeSinceStartup)
			.Append(";")
			.Append(getTimeSinceStartState());
		saveInCSV.Save(data);
	}
	public void traceSocket(GameObject si, string str)
	{
		StringBuilder data = new StringBuilder();
		data.Append("Receive;")
			.Append(str)
			.Append(";")
			.Append(si.name)
			.Append(";")
			.Append(Time.realtimeSinceStartup)
			.Append(";")
			.Append(getTimeSinceStartState());
		saveInCSV.Save(data);
	}

	public void traceInApp(GameObject go)
	{
		chgNbGrabObjectsByState(go);
		StringBuilder data = new StringBuilder();
		data.Append("Action;")
			.Append(go.name);

		Component[] components = go.GetComponents<Component>();
		int i = 0;
		bool notFound = true;
		while (i < components.Length && notFound)
		{
			Type componentType = components[i].GetType();
			if (cmpntTypeToAction.ContainsKey(componentType))
			{
				notFound = false;
				string customString = cmpntTypeToAction[componentType];
				data.Append(customString);
			}
			i++;
		}

		data.Append(Time.realtimeSinceStartup)
			.Append(";")
			.Append(getTimeSinceStartState());
		saveInCSV.Save(data);
	}

	public void save()
    {
        saveInCSV.Save(new StringBuilder().Append("END"));
    }

}
