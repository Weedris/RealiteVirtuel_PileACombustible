/* This script is for each maincomponent (or every element to place byt the user)
 * we use it to replace each element to ther starting position...
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class comeback : MonoBehaviour
{
    Vector3 pos;
    Quaternion rot;

    public void Start()
    {
        rot = transform.rotation;
        pos = transform.position;
    }

    public void returnToPosInit()
    {
        transform.position = pos;
        transform.rotation = rot;
    }

}
