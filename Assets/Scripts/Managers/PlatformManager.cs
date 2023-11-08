using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public Settings settings;
    public GameObject Pc;
    public GameObject Vr;

    void Start()
    {
#if UNITY_STANDALONE
        // Code spécifique à PC
        changeToPC();
#elif UNITY_ANDROID
    // Code spécifique à Android (appareil 3D)
        changeToVR();
#endif
    }

    private void changeToPC() 
    {
        settings.platform = Platform.PC;
        Vr.SetActive(false);
        Pc.SetActive(true);
    }
    private void changeToVR()
    {
        settings.platform = Platform.VR;
        Pc.SetActive(false);
        Vr.SetActive(true);
    }

}
