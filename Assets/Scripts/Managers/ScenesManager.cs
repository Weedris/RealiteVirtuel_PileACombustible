using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesManager : MonoBehaviour
{
	public static ScenesManager Instance { get; private set; }

	public enum Scene
	{
		MAIN_MENU,
		ASSEMBLY_SCENE,
		PERFORMANCE_LAB
	}

	// A remplir avec les noms de scenes
	private Dictionary<Scene, string> scenes = new()
	{
		[Scene.MAIN_MENU] = "MainMenu",
		[Scene.ASSEMBLY_SCENE] = "AssemblyScene",
		[Scene.PERFORMANCE_LAB] = "PerformanceLabScene"
	};

	private void Awake()
	{
		// singleton
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else Destroy(gameObject);
	}

	public void LoadScene(Scene scene)
	{
		StartCoroutine(LoadSceneAsync(scene));
	}

	private IEnumerator LoadSceneAsync(Scene scene)
	{
		AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenes[scene]);
		while (!asyncLoad.isDone)
		{
			if (asyncLoad.progress >= 0.9f)
				asyncLoad.allowSceneActivation = true;
			yield return null;
		}
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
