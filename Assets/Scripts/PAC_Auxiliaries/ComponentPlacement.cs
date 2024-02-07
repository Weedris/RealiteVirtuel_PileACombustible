/* This is where we check if the placement of a component to place is right
 * 
 * To check if the compoenent is well placed read checkComponentPlacement
 * 
 */

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ComponentPlacement : MonoBehaviour
{
	public static ComponentPlacement Instance;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public void CheckComponentPlacement(XRSocketInteractor socket)
	{
		traceParser.Instance.traceSocket(socket, socket.GetOldestInteractableSelected().transform.name);

		if (socket.GetOldestInteractableSelected().transform.name == socket.name && socket.name == GameManager.Instance.state.ToString())
		{
			socket.GetOldestInteractableSelected().transform.GetComponent<Collider>().enabled = false;
			socket.GetOldestInteractableSelected().transform.tag = "Placed";
			SoundManager.Instance.PlaySFX(SfxType.GoodAnswer);
			GameManager.Instance.NextState();
		}
		else SoundManager.Instance.PlaySFX(SfxType.BadAnswer);

	}

	public void CheckComponentPlacement(Transform target, Transform objectToPlace)
	{
		traceParser.Instance.traceSocket(target.gameObject, objectToPlace.name);
		if (objectToPlace.transform.name == target.name && target.name == GameManager.Instance.state.ToString())
		{
			Rigidbody rb = objectToPlace.GetComponent<Rigidbody>();
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			objectToPlace.GetComponent<Collider>().enabled = false;
			objectToPlace.tag = "Placed";
			objectToPlace.SetPositionAndRotation(target.position, target.rotation);

			SoundManager.Instance.PlaySFX(SfxType.GoodAnswer);
			GameManager.Instance.NextState();
		}
		else SoundManager.Instance.PlaySFX(SfxType.BadAnswer);

	}


	public void Caught(GameObject go)
	{
		GameManager.Instance.traceParser.traceInApp(go);
	}

}
