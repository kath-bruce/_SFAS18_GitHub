using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { NULL, INVINCIBILITY, POINT_STEALER, FREEZE }

public class PowerUp : MonoBehaviour
{
    //-------------------------------------------

    [SerializeField]
    float m_rotSpeed = 10.0f;

    //-------------------------------------------

    void Update()
    {
        //rotate power up mesh
        GameObject meshObj = GetComponentInChildren<MeshRenderer>().gameObject;

        meshObj.transform.Rotate(new Vector3(m_rotSpeed, m_rotSpeed, m_rotSpeed) * Time.deltaTime);

        //ensure power up text is always facing camera
        GameObject textObj = GetComponentInChildren<TextMesh>().gameObject;

        textObj.transform.rotation = Camera.main.transform.rotation;
    }
}
