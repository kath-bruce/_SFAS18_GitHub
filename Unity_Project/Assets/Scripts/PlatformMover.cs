using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    //----------------------------------------------

    [SerializeField]
    float m_maxY = 10.0f;

    [SerializeField]
    float m_minY = -5.0f;

    [SerializeField]
    float m_platformSpeed = 10.0f;

    //----------------------------------------------

    bool m_goingUp = false;

    //----------------------------------------------
    
    void Update()
    {
        if (GameController.INSTANCE.InCountdown() || GameController.INSTANCE.Paused())
            return;

        Vector3 currentPos = transform.position;

        //move platform between two points - maxY and minY
        if (m_goingUp)
        {
            currentPos.y += m_platformSpeed * Time.deltaTime;

            if (currentPos.y < m_maxY)
            {
                transform.position = currentPos;
            }
            else
            {
                m_goingUp = false;
            }
        }
        else
        {
            currentPos.y -= m_platformSpeed * Time.deltaTime;

            if (currentPos.y > m_minY)
            {
                transform.position = currentPos;
            }
            else
            {
                m_goingUp = true;
            }

        }
    }
}
