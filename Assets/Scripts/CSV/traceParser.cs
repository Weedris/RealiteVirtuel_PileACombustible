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
    private StringBuilder data = new StringBuilder();
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
        return Time.realtimeSinceStartup - lastTime;
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
        data.Append("ChangeStep;State;")
            .Append(state.ToString())
            .Append(";")
            .Append(Time.realtimeSinceStartup)
            .Append(";")
            .Append(getTimeSinceStartState())
            .AppendLine();
        setLastTime();
    }

    public void traceSocket(XRSocketInteractor si, string str)
    {
        data.Append("Receive;")
            .Append(str)
            .Append(";")
            .Append(si.name)
            .Append(";")
            .Append(Time.realtimeSinceStartup)
            .Append(";")
            .Append(getTimeSinceStartState())
            .AppendLine();
    }

    public void traceInApp(GameObject go)
    {
        chgNbGrabObjectsByState(go);
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
            .Append(getTimeSinceStartState())
            .AppendLine();
    }

    public void save()
    {
        saveInCSV.saveNbGrabObject(nbGrabObjects);
        saveInCSV.save(data);
    }

}
