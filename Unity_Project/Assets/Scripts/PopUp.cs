using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    //-------------------------------------------

    float m_timeLeft = 1.5f;

    //-------------------------------------------

    void Start()
    {
        Destroy(gameObject, m_timeLeft);
    }
}
