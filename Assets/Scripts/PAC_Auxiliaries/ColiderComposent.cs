using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class ColiderComposent : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Verif(other);
	}
	private void OnTriggerExit(Collider other)
	{
		Verif(other);
	}

	public void Verif(Collider other)
	{
		
		if (!KeyboardActionManager.Instance.GrabOn())
			ComponentPlacement.Instance.CheckComponentPlacement(transform, other.transform);
	}
}
