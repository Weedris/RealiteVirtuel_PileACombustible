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

public class traceParser : MonoBehaviour
{
    private double lastTime;
    private StringBuilder nbGrabObjects = new StringBuilder();
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
        if(nbGrabObjectsByState.ContainsKey(name))
        {
            nbGrabObjectsByState[name] += 1;
        }
        else
        {
            nbGrabObjectsByState.Add(name, 1);
        }
    }

    private void rstNbGrabObjectsByState(GameManager.State state)
    {
        nbGrabObjects.Append("State;")
                     .Append(state.ToString())
                     .AppendLine();
        foreach (KeyValuePair<string, int> kvp in nbGrabObjectsByState)
        {
            nbGrabObjects.Append(kvp.Key)
                         .Append(";")
                         .Append(kvp.Value)
                         .AppendLine();
        }
        nbGrabObjectsByState.Clear();
    }

    private double getTimeSinceStartState()
    {
        double time = Time.realtimeSinceStartup - lastTime;
        return time<0.5?0:time;
    }

    private void setLastTime()
    {
        lastTime = Time.realtimeSinceStartup;
    }

    void Start()
    { 
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
        saveInCSV.save(data);
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
        saveInCSV.save(data);
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
        while(i < components.Length && notFound) {
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
        saveInCSV.save(data);
    }

    public void save()
    {
        saveInCSV.save(nbGrabObjects, "nbGrabObjects");
        StringBuilder data = new StringBuilder();
        data.Append("End;;;")
            .Append(Time.realtimeSinceStartup)
            .Append(";")
            .Append(getTimeSinceStartState());
        saveInCSV.save(data);
    }

}
