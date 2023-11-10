/* This is where we check if the placement of a component to place is right
 * 
 * To check if the compoenent is well placed read checkComponentPlacement
 * 
 */

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ComponentPlacement : MonoBehaviour
{
	public void CheckComponentPlacement(XRSocketInteractor socket)
	{

		if (socket.GetOldestInteractableSelected().transform.name == socket.name && socket.name == GameManager.Instance.state.ToString())
		{
			socket.GetOldestInteractableSelected().transform.GetComponent<Collider>().enabled = false;
			socket.GetOldestInteractableSelected().transform.tag = "Placed";
			SoundManager.Instance.PlaySFX(SfxType.GoodAnswer);
            GameManager.Instance.NextState();
        }
		else
		{
			GameManager.Instance.traceParser.traceSocket(socket, socket.GetOldestInteractableSelected().transform.name);
			SoundManager.Instance.PlaySFX(SfxType.BadAnswer);
		}
	}

	public void Caught(GameObject go)
	{
		GameManager.Instance.traceParser.traceInApp(go);
	}

}
