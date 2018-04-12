using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    //----------------------------------------------
    
    [SerializeField]
    Material m_p1Mat;

    [SerializeField]
    Material m_p2Mat;

    [SerializeField]
    Material m_defaultMat;

    [SerializeField]
    List<GameObject> m_SpawningPositions;

    //-----------------------------------------------

    //which player last touched the ball
    int m_WinningPlayer = 0;

    //if it has landed then players can score points from the ball
    //this is done as the ball spawns just above the platform and players could stand on the spawn
    //point and head the ball for an easy point
    bool m_HasLanded = false;

    AudioSource m_audioSrc;

    //------------------------------------------------

    void OnEnable()
    {
        GameController.OnResetGame += OnResetBall;
    }

    void OnDisable()
    {
        GameController.OnResetGame -= OnResetBall;
    }
    
    void Start()
    {
        m_audioSrc = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameController.INSTANCE.Paused())
        {
            GetComponent<Rigidbody>().Sleep();
        }
        else
        {
            GetComponent<Rigidbody>().WakeUp();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && m_HasLanded)
        {
            int playerNum = collision.gameObject.GetComponent<PlayerController>().GetPlayerNum();

            SetWinningPlayer(playerNum);
        }
        else if (collision.gameObject.tag == "Platform")
        {
            m_HasLanded = true;
        }
    }

    //-----------------------------------------------

    void OnResetBall()
    {
        Respawn();
    }

    //-----------------------------------------------

    public void Respawn()
    {
        //random spawning position
        transform.position = m_SpawningPositions[Random.Range(0, m_SpawningPositions.Count)].transform.position;

        m_WinningPlayer = 0;
        gameObject.GetComponent<Renderer>().material = m_defaultMat;
        m_HasLanded = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public int GetWinningPlayer()
    {
        return m_WinningPlayer;
    }

    public void SetWinningPlayer(int playerNum)
    {
        if (playerNum == 1)
        {
            gameObject.GetComponent<Renderer>().material = m_p1Mat;
        }
        else if (playerNum == 2)
        {
            gameObject.GetComponent<Renderer>().material = m_p2Mat;
        }

        m_WinningPlayer = playerNum;
    }

    public void PlayClip(AudioClip clip)
    {
        m_audioSrc.clip = clip;
        m_audioSrc.Play();
    }

}
