/* This is where we check if the placement of a component to place is right
 * 
 * To check if the compoenent is well placed read checkComponentPlacement
 * 
 */

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ComponentPlacement : MonoBehaviour
{

    private GameManager GameManager;
    public GameObject good;
    public GameObject bad;

    public void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        good = GameObject.Find("CorrectAnswerSound");
        bad = GameObject.Find("IncorrectAnswerSound");
    }

    public void checkComponentPlacement(XRSocketInteractor socket)
    {

        if (socket.GetOldestInteractableSelected().transform.name == socket.name && socket.name == GameManager.state.ToString())
        {
            socket.GetOldestInteractableSelected().transform.GetComponent<Collider>().enabled = false;
            socket.GetOldestInteractableSelected().transform.tag = "Placed";
            GameManager.NextState();

            good.GetComponent<AudioSource>().Play();
        }
        else
        {
            GameManager.traceParser.traceSocket(socket, socket.GetOldestInteractableSelected().transform.name);

            bad.GetComponent<AudioSource>().Play();
        }
    }

    public void caught(GameObject go)
    {
        GameManager.traceParser.traceInApp(go);
    }

}
