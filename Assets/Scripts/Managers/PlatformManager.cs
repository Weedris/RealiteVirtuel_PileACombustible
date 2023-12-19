using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlatformManager : MonoBehaviour
{
	public Settings settings;
	public GameObject Pc;
	public GameObject Vr;
	public GameObject SocketMainComponent;
	

	void Start()
	{
#if UNITY_STANDALONE || UNITY_EDITOR
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


		XRSocketInteractor[] childrens = SocketMainComponent.GetComponentsInChildren<XRSocketInteractor>();
		foreach (XRSocketInteractor child in childrens)
		{
            Destroy(child);
        }
    }
	private void changeToVR()
	{
		settings.platform = Platform.VR;
		Pc.SetActive(false);
		Vr.SetActive(true);

        ColiderComposent[] childrens = SocketMainComponent.GetComponentsInChildren<ColiderComposent>();
        foreach (ColiderComposent child in childrens)
        {
			Destroy(child);
        }
    }

}
