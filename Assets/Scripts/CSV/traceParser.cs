/* This file is to make phrase for the save function
 * so us human can understand what this is easly
 * 
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class traceParser : MonoBehaviour
{
    private string text;

    private GameManager gm;

    private void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    public void traceMainStep(GameManager.State state)
    {
        text = "State;" + state.ToString() + ";" + Time.realtimeSinceStartup;

        sendIt(text);
    }

    public void traceSocket(XRSocketInteractor si, string str)
    {
        text = str + ";receive;" + si.name;

        sendIt(text);
    }

    public void traceInApp(GameObject go)
    {
        if (go.GetComponent<ActionBasedController>() != null)
        {
            text = go.name + ";Grab;" + Time.realtimeSinceStartup;
        }
        else if (go.GetComponent<XRGrabInteractable>() != null)
        {
            text = go.name + ";Caught;" + Time.realtimeSinceStartup;
        }
        else if (go.GetComponent<XRSocketInteractor>() != null)
        {
            text = go.name + ";PutInSocket;" + Time.realtimeSinceStartup;
        }


        sendIt(text);

    }

    public void sendIt(string txt)
    {

        gm.saveInCSV.saveIt(text);
    }

}
