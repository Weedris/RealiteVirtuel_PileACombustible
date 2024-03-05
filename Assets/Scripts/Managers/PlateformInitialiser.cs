using UnityEngine;
using UnityEngine.XR;

namespace Assets.Scripts.Managers
{
	public class PlateformInitialiser : MonoBehaviour
	{
		[SerializeField] private GameObject VR;
		[SerializeField] private GameObject PC;

		private void Awake()
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			XRSettings.enabled = false;
			PC.SetActive(true);
			Destroy(VR);
			DataSaver.Instance.Log("XR was disabled");
#else
			XRSettings.enabled = true;
			VR.SetActive(true);
			Destroy(PC);
			DataSaver.Instance.Log("XR was enabled");
			if (Application.platform == RuntimePlatform.Android)
			{
				// configure for meta quest and other android xr devices
			}
#endif
			Destroy(gameObject);
		}
	}
}