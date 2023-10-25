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
    private StringBuilder text = new StringBuilder();

    private SaveInCSV saveInCSV = new SaveInCSV();

    private Dictionary<Type, string> cmpntTypeToAction = new Dictionary<Type, string>
    {
        //{ typeof(ActionBasedController), ";Grab;" },
        //{ typeof(XRGrabInteractable), ";Caught;" },
        //{ typeof(XRSocketInteractor), ";PutInSocket;" }
    };

    public void traceMainStep(GameManager.State state)
    {
        //text.Append("ChangeStep;State;")
        //    .Append(state.ToString())
        //    .Append(";")
        //    .Append(Time.realtimeSinceStartup);

        //sendIt(text.ToString());
    }

    public void traceSocket(XRSocketInteractor si, string str)
    {
        //text.Append("Receive;")
        //    .Append(str)
        //    .Append(";")
        //    .Append(si.name)
        //    .Append(";")
        //    .Append(Time.realtimeSinceStartup);

        //sendIt(text.ToString());
    }

    public void traceInApp(GameObject go)
    {
        //text.Append("Action;")
        //    .Append(go.name);

        //Component[] components = go.GetComponents<Component>();
        //int i = 0;
        //bool notFound = true;
        //while(i < components.Length && notFound) {
        //    Type componentType = components[i].GetType();
        //    if (cmpntTypeToAction.ContainsKey(componentType))
        //    {
        //        notFound = false;
        //        string customString = cmpntTypeToAction[componentType];
        //        text.Append(customString);
        //    }
        //    i++;
        //}

        //text.Append(Time.realtimeSinceStartup);

        //sendIt(text.ToString());

    }

    public void sendIt(string txt)
    {
        //saveInCSV.saveIt(txt);
        //text.Clear();
    }

    public void save()
    {
        //saveInCSV.save();
    }

}
