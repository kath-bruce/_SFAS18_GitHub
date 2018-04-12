using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTrigger : MonoBehaviour
{
    //-------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponentInParent<PowerUpSpawner>().PowerUpCollected(other.gameObject);
        }
    }
}
