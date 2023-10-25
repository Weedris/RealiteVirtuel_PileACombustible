using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{

    public static ScenesManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void chapter1() 
    {
        SceneManager.LoadScene("GameScene");
    }
    public void chapter2()
    {
        SceneManager.LoadScene("GameScene");
    }

    /*Scene scene = SceneManager.GetActiveScene();
    Debug.Log("La scène active est : " + scene.name);
    */
    
}
