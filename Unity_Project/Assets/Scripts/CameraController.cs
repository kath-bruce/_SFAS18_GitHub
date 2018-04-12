using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float m_zoomFactor = 20.0f;

    [SerializeField]
    float m_minZoomDistance = 75.0f;

    [SerializeField]
    float m_maxZoomDistance = 100.0f;

    [SerializeField]
    float m_lowerViewportLimit = 0.1f;

    [SerializeField]
    float m_upperViewportLimit = 0.9f;

    //------------------------------------------------

    Vector3 m_OriginalPos = Vector3.zero;

    //------------------------------------------------

    void OnEnable()
    {
        GameController.OnResetGame += OnResetCamera;
    }

    void OnDisable()
    {
        GameController.OnResetGame -= OnResetCamera;
    }
    
    void Start()
    {
        m_OriginalPos = transform.position;
    }
    
    void Update()
    {
        if (GameController.INSTANCE.Paused() || GameController.INSTANCE.HasTimeRunOut() || GameController.INSTANCE.InCountdown())
            return;

        //check if ball is in view
        //if not then zoom out to less than max zoom distance
        //otherwise
        //zoom in to more than min zoom distance

        if (IsObjectInView(GameController.INSTANCE.GetBallGO()))
        {
            if (CanZoom(true))
            {
                Vector3 new_pos = transform.position;

                new_pos += transform.forward * m_zoomFactor * Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, new_pos, 0.5f);
            }
        }
        else
        {
            if (CanZoom(false))
            {
                Vector3 new_pos = transform.position;

                new_pos -= transform.forward * m_zoomFactor * Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, new_pos, 0.5f);
            }
        }

    }
    
    //-----------------------------------------------

    void OnResetCamera()
    {
        transform.position = m_OriginalPos;
    }

    //------------------------------------------------

    //returns true if object is in view of camera within set bounds
    //returns false otherwise
    bool IsObjectInView(GameObject player)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(player.transform.position);

        if (
            viewportPos.x >= m_lowerViewportLimit && viewportPos.x <= m_upperViewportLimit &&
            viewportPos.y >= m_lowerViewportLimit && viewportPos.y <= m_upperViewportLimit &&
            viewportPos.z >= 0.0f
            )
        {
            return true;
        }

        return false;
    }

    //zoomIn - false = check if can zoom out, true = check if can zoom in
    bool CanZoom(bool zoomIn)
    {
        if (zoomIn)
        {
            //return true if distance from camera to platform is greater than minimum zoom distance - therefore can zoom in
            return Vector3.Distance(transform.position, GameController.INSTANCE.GetPlatformGO().transform.position) > m_minZoomDistance;
        }
        else
        {
            //return true if distance is less than maximum zoom distance - therefore can zoom out
            return Vector3.Distance(transform.position, GameController.INSTANCE.GetPlatformGO().transform.position) < m_maxZoomDistance;
        }
    }

    
}
