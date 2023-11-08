/* This is where we check if the placement of a component to place is right
 * 
 * To check if the compoenent is well placed read checkComponentPlacement
 * 
 */

using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ComponentPlacement : MonoBehaviour
{
    public static ComponentPlacement Instance;

    private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

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

	public void CheckComponentPlacement(GameObject colider , GameObject objet)
	{
		if (objet.transform.name == colider.name && colider.name == GameManager.Instance.state.ToString())
		{
            objet.transform.GetComponent<Collider>().enabled = false;
            objet.transform.tag = "Placed";
            SoundManager.Instance.PlaySFX(SfxType.GoodAnswer);
            GameManager.Instance.NextState();
        }
        else
        {
			GameManager.Instance.traceParser.traceSocket(colider, objet.transform.name);
			SoundManager.Instance.PlaySFX(SfxType.BadAnswer);
		}
	}


    public void Caught(GameObject go)
	{
		GameManager.Instance.traceParser.traceInApp(go);
	}

}
