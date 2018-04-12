using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotater : MonoBehaviour
{
    //----------------------------------------------

    [SerializeField]
    float m_rotSpeed = 10.0f;

    //----------------------------------------------

    void Update()
    {
        if (GameController.INSTANCE.InCountdown() || GameController.INSTANCE.Paused())
            return;

        transform.Rotate(new Vector3(0, m_rotSpeed, 0) * Time.deltaTime);
    }

    //----------------------------------------------

    public void ResetPlatform()
    {
        transform.rotation = Quaternion.identity;
    }
}
