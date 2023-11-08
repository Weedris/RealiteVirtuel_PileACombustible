using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiderComposent : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        ComponentPlacement.Instance.CheckComponentPlacement(gameObject, other.gameObject);
    }
}
