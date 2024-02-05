using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlatformManager : MonoBehaviour
{
	public Settings settings;
	public GameObject Pc;
	public GameObject Vr;
	public GameObject SocketMainComponent;




    void Awake()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        // Code sp�cifique aux PC
        ChangeToPC();
#elif UNITY_ANDROID
		// Code sp�cifique aux Android (casque VR)
		ChangeToVR();
#endif
    }

    private void ChangeToPC()
	{
		settings.platform = Platform.PC;
		Vr.SetActive(false);
		Pc.SetActive(true);

		DeleteChidrens<XRSocketInteractor>(SocketMainComponent);

		if (Settings.Instance.isPlayerPastAssembly)
			DeleteChidrens<ColiderComposent>(SocketMainComponent);

	}

	private void ChangeToVR()
	{
		settings.platform = Platform.VR;
		Pc.SetActive(false);
		Vr.SetActive(true);

		DeleteChidrens<ColiderComposent>(SocketMainComponent);

		if (Settings.Instance.isPlayerPastAssembly)
			DeleteChidrens<XRSocketInteractor>(SocketMainComponent);
	}

	private void DeleteChidrens<T>(GameObject parent)
	{
		if (parent == null) return;
		T[] childrens = parent.GetComponentsInChildren<T>();
		foreach (T child in childrens)
			Destroy(child as Component);
	}
}
