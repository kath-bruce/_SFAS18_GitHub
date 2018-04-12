using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GlobalState { MAIN_MENU, MANUAL, PLAY }

public class GlobalController : MonoBehaviour
{
    //----------------------------------------------------

    //singleton - handles menus and global state
    public static GlobalController INSTANCE { get; protected set; }

    private GlobalState m_currentState = GlobalState.MAIN_MENU;

    public GlobalState CurrentState
    {
        get
        {
            return m_currentState;
        }

        private set
        {
            m_currentState = value;
        }
    }

    //------------------------------------------------------
    
    [SerializeField]
    GameObject m_mainMenu;

    [SerializeField]
    GameObject m_play;

    [SerializeField]
    GameObject m_manual;

    [SerializeField]
    GameObject m_back;

    //-----------------------------------------------------

    AudioSource m_audioSrc;

    //---------------------------------------------------

    void Awake()
    {
        if (INSTANCE != null)
        {
            Debug.LogError("MORE THAN ONE GLOBAL CONTROLLER!!!!");
        }
        else
        {
            INSTANCE = this;
        }
    }

    void Start()
    {
        m_audioSrc = GetComponent<AudioSource>();

        //ensure that main menu is active and other ui is inactive
        m_mainMenu.SetActive(true);
        m_back.SetActive(false);
        m_manual.SetActive(false);
        m_play.SetActive(false);
    }

    //-----------------------------------------------------

    #region button handlers
    public void ShowBackButton(bool pause)
    {
        m_back.SetActive(pause);
        PlayClip(GameController.INSTANCE.GetClip("ButtonBeep"));
    }

    public void OnClickBackButton()
    {
        m_currentState = GlobalState.MAIN_MENU;

        m_mainMenu.SetActive(true);

        m_back.SetActive(false);
        m_manual.SetActive(false);
        m_play.SetActive(false);

        GameController.INSTANCE.ResetGameController();
        PlayClip(GameController.INSTANCE.GetClip("ButtonBeep"));
    }

    public void OnClickPlayButton()
    {
        m_currentState = GlobalState.PLAY;

        m_play.SetActive(true);

        m_mainMenu.SetActive(false);
        m_back.SetActive(false);
        m_manual.SetActive(false);

        Cursor.visible = false;

        GameController.INSTANCE.ResetGameController();
        PlayClip(GameController.INSTANCE.GetClip("CountdownBeep"));
    }

    public void OnClickManualButton()
    {
        m_currentState = GlobalState.MANUAL;

        m_manual.SetActive(true);
        m_back.SetActive(true);

        m_mainMenu.SetActive(false);
        m_play.SetActive(false);
        PlayClip(GameController.INSTANCE.GetClip("ButtonBeep"));
    }

    public void OnClickQuitButton()
    {
        PlayClip(GameController.INSTANCE.GetClip("ButtonBeep"));

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
#endregion
    
    public void PlayClip(AudioClip clip)
    {
        m_audioSrc.clip = clip;
        m_audioSrc.Play();
    }
}
