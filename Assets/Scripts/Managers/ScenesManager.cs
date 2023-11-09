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
		SceneManager.LoadScene("GameScene");
	}
	public static void Chapter2()
	{
		SceneManager.LoadScene("GameScene");
	}

}
