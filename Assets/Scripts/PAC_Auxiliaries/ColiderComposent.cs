using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class ColiderComposent : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        verif(other);
    }
    private void OnTriggerExit(Collider other)
    {
        verif(other);
    }

   private void verif(Collider other) 
    {
        if (KeyboardActionManager.Instance.grabOn() == false)
            ComponentPlacement.Instance.CheckComponentPlacement(gameObject, other.gameObject);
    }
}
