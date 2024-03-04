using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
	private Transform head;

	[FormerlySerializedAs("menu")][SerializeField] private GameObject pauseMenu;
	[SerializeField] private InputActionProperty showButton;

	private List<GameObject> menuStack = new();

	private void Start()
	{
		head = Camera.main.transform;
	}

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
			Vector3 headPosition = head.position;
			foreach (GameObject menu in menuStack)
			{
				Transform menuTransform = menu.transform;
				menuTransform.position = headPosition + new Vector3(head.forward.x, 0, head.forward.z) * spawnDistance;
				menuTransform.LookAt(new Vector3(headPosition.x, menuTransform.position.y, headPosition.z));
				menuTransform.forward *= -1;
			}
		}
	}

	public void AddMenu(GameObject menu)
	{
		menuStack.Add(menu);
		menu.SetActive(true);

		// shows the menu in front of the player
		Vector3 headPosition = head.position;
		Transform menuTransform = menu.transform;

		menuTransform.position = headPosition + new Vector3(head.forward.x, 0, head.forward.z) * spawnDistance;
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
