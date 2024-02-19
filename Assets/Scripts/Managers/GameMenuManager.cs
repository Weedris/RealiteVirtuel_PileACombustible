using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
	[SerializeField] private Transform head;
	[SerializeField] private float spawnDistance;

	[SerializeField] private GameObject menu;
	[SerializeField] private InputActionProperty showButton;

    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
		{
			menu.SetActive(!menu.activeSelf);
			Transform menuTransform = menu.transform;
			Vector3 headPosition = head.position;
			menuTransform.position = headPosition + new Vector3(head.forward.x, 0, head.forward.z) * spawnDistance;
			menuTransform.LookAt(new Vector3(headPosition.x, menuTransform.position.y, headPosition.z));
			menuTransform.forward *= -1;
		}
    }
}
