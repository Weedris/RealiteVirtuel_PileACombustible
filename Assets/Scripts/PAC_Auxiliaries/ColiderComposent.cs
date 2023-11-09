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

   private void Verif(Collider other) 
    {
        if (!KeyboardActionManager.Instance.grabOn())
            ComponentPlacement.Instance.CheckComponentPlacement(transform, other.transform);
    }
}
