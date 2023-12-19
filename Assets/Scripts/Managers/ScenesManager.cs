using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance
	{
		get
		{
			instance ??= new ScenesManager();
			return instance;
		}
	}
	private static ScenesManager instance;

	private void Awake()
	{
		if (instance != this)
		{
			Debug.LogWarning("A SceneManager already exists, ignoring this");

            return;
		}
		DontDestroyOnLoad(gameObject);
        
    }

	public static void Chapter1()
	{
        Settings.Instance.set_pass_assembly(false);

        SceneManager.LoadScene("GameScene");
	}
	public static void Chapter2()
	{
        Settings.Instance.set_pass_assembly(true)  ;
		
        SceneManager.LoadScene("GameScene");
        
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
