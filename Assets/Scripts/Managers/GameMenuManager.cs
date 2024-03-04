/*
 * Yeah... this ain't gonna cut it...
 * Well, there's no point in making it better now as the only menu that pops is the pause menu
 * If you need more, you'll have to had some kind of menu that remembers stuff it poped (probably with a prefab) then call this thing.
 * Still, it feels like it's poorly executed, not that i know how to do better for now
 * i need to gain more exp but don't have time to stay on this one gadget fonctinality for now
 */


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
	private static GameMenuManager _instance;
	public static GameMenuManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<GameMenuManager>();
				if (_instance == null)
				{
					_instance = new GameMenuManager();
					_instance.gameObject.name = "MenuManager";
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}
	
	[SerializeField] private float spawnDistance;
	[SerializeField] private bool menuFollowsPlayer;
	private Transform headTransform;

	[SerializeField] private GameObject pauseMenu;
	[SerializeField] private InputActionProperty showButton;

	private List<GameObject> menuStack = new();

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetButtonDown("Cancel") || showButton.action.WasPressedThisFrame())
		{
			if (menuStack.Count > 0) CloseLastMenu();
			else AddMenu(pauseMenu);
		}

		if (menuFollowsPlayer)
		{
			Vector3 headPosition = headTransform.position;
			foreach (GameObject menu in menuStack)
			{
				Transform menuTransform = menu.transform;
				menuTransform.position = headPosition + new Vector3(headTransform.forward.x, 0, headTransform.forward.z) * spawnDistance;
				menuTransform.LookAt(new Vector3(headPosition.x, menuTransform.position.y, headPosition.z));
				menuTransform.forward *= -1;
			}
		}
	}

	public void AddMenu(GameObject menu)
	{
		headTransform = Camera.main.transform;
		menuStack.Add(menu);
		menu.SetActive(true);

		// shows the menu in front of the player
		Vector3 headPosition = headTransform.position;
		Transform menuTransform = menu.transform;

		menuTransform.position = headPosition + new Vector3(headTransform.forward.x, 0, headTransform.forward.z) * spawnDistance;
		menuTransform.LookAt(new Vector3(headPosition.x, menuTransform.position.y, headPosition.z));
		menuTransform.forward *= -1;
	}

	private void CloseLastMenu()
	{
		menuStack[^1].SetActive(false);
		menuStack.RemoveAt(menuStack.Count-1);
	}

	public void CloseSpecificMenu(GameObject menu)
	{
		menu.SetActive(false);
		menuStack.Remove(menu);
	}
}
