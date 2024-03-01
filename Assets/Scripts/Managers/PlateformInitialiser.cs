using UnityEngine;
using UnityEngine.XR;

namespace Assets.Scripts.Managers
{
	public class PlateformInitialiser : MonoBehaviour
	{
		[SerializeField] private GameObject VR;
		[SerializeField] private GameObject PC;

		private void Start()
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			XRSettings.enabled = false;
			VR.SetActive(false);
			PC.SetActive(true);
			DataSaver.Instance.Log("XR was disabled");
			Destroy(gameObject);
			return;
#endif
			XRSettings.enabled = true;
			VR.SetActive(true);
			PC.SetActive(false);
			if (Application.platform == RuntimePlatform.Android)
			{
				// configure for meta quest and other android xr devices
			}
			DataSaver.Instance.Log("XR was enabled");
			Destroy(gameObject);
		}
	}
}