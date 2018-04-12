using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //-------------------------------------------------

    //singleton - central script to allow scripts to easily access
    //relevant method from other scripts
    public static GameController INSTANCE { get; protected set; }

    public int Player1Score { get; private set; }
    public int Player2Score { get; private set; }

    public delegate void ResetGame();
    public static event ResetGame OnResetGame;

    //-------------------------------------------------

    [SerializeField]
    GameObject m_Player1;

    [SerializeField]
    GameObject m_Player2;

    [SerializeField]
    GameObject m_Platform;

    [SerializeField]
    GameObject m_Ball;

    [SerializeField]
    GameObject m_Sparkles;

    [SerializeField]
    float m_startCountdown = 6.0f;

    //------------------------------------------------

    UIManager m_UI_Manager;

    AudioManager m_Audio_Manager;

    //various members to hold relevant timings
    const float TIME_LIMIT = 60.0f;
    float m_timeLeft = TIME_LIMIT;

    bool m_hasTimeRunOut = false;

    const float COUNTDOWN = 3.0f;
    float m_countdownLeft = COUNTDOWN;
    
    bool m_inCountdown = true;
    float m_countdownSecs = 0.0f;

    bool m_paused = false;

    //----------------------------------------------

    void Awake()
    {
        if (INSTANCE != null)
        {
            Debug.LogError("MORE THAN ONE GAME CONTROLLER!!!!");
        }
        else
        {
            INSTANCE = this;
        }
    }

    void Start()
    {
        m_UI_Manager = FindObjectOfType<UIManager>();

        m_Audio_Manager = FindObjectOfType<AudioManager>();

        ResetScores();
        ResetTime();
    }

    void Update()
    {
        //only update if currently playing
        if (GlobalController.INSTANCE.CurrentState != GlobalState.PLAY || m_hasTimeRunOut)
        {
            Cursor.visible = true;
            return;
        }

        CheckIfPaused();

        if (m_inCountdown && !m_paused)
        {
            UpdateCountdown();
        }
        else if (!m_paused)
        {
            UpdateTimeLeft();
        }
    }

    //-----------------------------------------

    void CheckIfPaused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_paused = !m_paused;

            Cursor.visible = m_paused;

            GlobalController.INSTANCE.ShowBackButton(m_paused);

            if (m_paused)
                m_UI_Manager.SetGameText("PAUSED");
            else
                m_UI_Manager.SetGameText("");
        }
    }

    void UpdateTimeLeft()
    {
        m_timeLeft -= Time.deltaTime;

        if (!m_hasTimeRunOut)
        {
            m_UI_Manager.UpdateTime(m_timeLeft);

            //if time left in game is less than the time to start the final countdown
            //play a sound effect every second
            if (m_timeLeft <= m_startCountdown)
            {
                m_countdownSecs += Time.deltaTime;

                if (m_countdownSecs >= 1.0f)
                {
                    GlobalController.INSTANCE.PlayClip(GetClip("CountdownBeep"));
                    m_countdownSecs = 0.0f;
                }
            }
        }

        if (m_timeLeft <= 0.0f)
        {
            m_hasTimeRunOut = true;
            GlobalController.INSTANCE.ShowBackButton(m_hasTimeRunOut);

            //display relevant end game text
            if (Player1Score == Player2Score)       //draw
            {
                m_UI_Manager.GameEnd(0, "white");
            }
            else if (Player1Score > Player2Score)   //player 1 win
            {
                m_UI_Manager.GameEnd(1, "blue");
            }
            else                                    //player 2 win
            {
                m_UI_Manager.GameEnd(2, "red");
            }
        }
    }

    void UpdateCountdown()
    {
        m_countdownLeft -= Time.deltaTime;
        m_UI_Manager.UpdateCountdown(m_countdownLeft);

        if (m_countdownLeft <= 0.0f)
        {
            m_inCountdown = false;
            m_countdownSecs = 0.0f;
        }

        m_countdownSecs += Time.deltaTime;

        //play sound effect every second
        if (m_countdownSecs >= 1.0f)
        {
            GlobalController.INSTANCE.PlayClip(GetClip("CountdownBeep"));
            m_countdownSecs = 0.0f;
        }
    }

    void ResetScores()
    {
        Player1Score = 0;
        Player2Score = 0;
    }

    void ResetTime()
    {
        m_timeLeft = TIME_LIMIT;
        m_hasTimeRunOut = false;

        m_countdownLeft = COUNTDOWN;
        m_inCountdown = true;

        m_paused = false;

        m_countdownSecs = 0.0f;
    }

    //-----------------------------------------------------------

    #region various getters
    public float GetCountdown()
    {
        return m_countdownLeft;
    }

    public bool InCountdown()
    {
        return m_inCountdown;
    }

    public bool Paused()
    {
        return m_paused;
    }

    public AudioClip GetClip(string clipName)
    {
        return m_Audio_Manager.GetAudioClip(clipName);
    }

    #region get gameobjects
    public GameObject GetPlayer1GO()
    {
        return m_Player1;
    }

    public GameObject GetPlayer2GO()
    {
        return m_Player2;
    }

    public GameObject GetPlatformGO()
    {
        return m_Platform;
    }

    public GameObject GetBallGO()
    {
        return m_Ball;
    }
    #endregion

    public bool HasTimeRunOut()
    {
        return m_hasTimeRunOut;
    }

    public float GetTime()
    {
        return TIME_LIMIT;
    }

    public GameObject GetPlayerGO(int playerNum)
    {
        if (playerNum == 1)
        {
            return m_Player1;
        }
        else if (playerNum == 2)
        {
            return m_Player2;
        }
        else
        {
            return null;
        }
    }
    #endregion

    //---------------------------------------------------------

    public void AddToPlayerScore(int playerNum, int amountToAdd)
    {
        //ensures no points can be gained after game has ended
        if (m_hasTimeRunOut)
            return;

        if (playerNum == 1)
        {
            Player1Score += amountToAdd;
            m_UI_Manager.UpdateScore(playerNum, Player1Score);
        }
        else if (playerNum == 2)
        {
            Player2Score += amountToAdd;
            m_UI_Manager.UpdateScore(playerNum, Player2Score);
        }
    }
    
    public void ResetGameController()
    {
        ResetTime();
        ResetScores();

        //force platform to reset first as platform has spawner positions
        m_Platform.GetComponentInChildren<PlatformRotater>().ResetPlatform();

        OnResetGame();
    }

    //allows other scripts to create relevant ui elements
    //without needing to access the UI Manager script
    #region ui handlers
    public void CreateSparkles(Vector3 pos)
    {
        Instantiate(m_Sparkles, pos, Quaternion.identity);
    }
    
    public void CreatePopUp(string popupText, int player, Color popupColour)
    {
        m_UI_Manager.CreatePopUp(popupText, player, popupColour);
    }

    public void SetPowerUpStatus(int playerNum, PowerUpType powerUp)
    {
        switch (powerUp)
        {
            case PowerUpType.FREEZE:
                m_UI_Manager.SetPowerUpStatus(playerNum, "FREEZE", Color.cyan);
                break;

            case PowerUpType.INVINCIBILITY:
                m_UI_Manager.SetPowerUpStatus(playerNum, "INVINCIBILITY", Color.magenta);
                break;

            case PowerUpType.POINT_STEALER:
                m_UI_Manager.SetPowerUpStatus(playerNum, "POINT\nSTEALER", Color.yellow);
                break;

            case PowerUpType.NULL:
                m_UI_Manager.SetPowerUpStatus(playerNum, "", Color.black);
                break;

            default:
                break;
        }
    }

    public void SetFrozenStatus(int playerNum, bool isFrozen)
    {
        m_UI_Manager.SetFrozenStatus(playerNum, isFrozen);
    }
    #endregion
}
