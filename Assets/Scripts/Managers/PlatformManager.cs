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

        SupprimerComposantsEnfants<XRSocketInteractor>(SocketMainComponent);

        if (Settings.Instance.get_pass_assembly())
        {
            SupprimerComposantsEnfants<ColiderComposent>(SocketMainComponent);
        }
    }

    private void changeToVR()
    {
        settings.platform = Platform.VR;
        Pc.SetActive(false);
        Vr.SetActive(true);

        SupprimerComposantsEnfants<ColiderComposent>(SocketMainComponent);

        if (Settings.Instance.get_pass_assembly())
        {
            SupprimerComposantsEnfants<XRSocketInteractor>(SocketMainComponent);
        }
    }

    private void SupprimerComposantsEnfants<T>(GameObject parent)
    {
        if (parent != null)
        {
            T[] childrens = parent.GetComponentsInChildren<T>();
            foreach (T child in childrens)
            {
                Destroy(child as Component);
            }
        }
    }

}
