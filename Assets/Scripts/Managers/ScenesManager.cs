using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
	private static ScenesManager instance;
	public static ScenesManager Instance
	{
		get
		{
			instance ??= new ScenesManager();
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != this)
			return;
		DontDestroyOnLoad(gameObject);
	}

	public static void Exit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

}
