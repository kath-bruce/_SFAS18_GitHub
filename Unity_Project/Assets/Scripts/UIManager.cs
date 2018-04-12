using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // --------------------------------------------------------------

    [SerializeField]
    Text m_Player1ScoreText;

    [SerializeField]
    Text m_Player2ScoreText;

    [SerializeField]
    Text m_TimeLeftText;

    [SerializeField]
    Text m_GameText;

    [SerializeField]
    Text m_Player1PowerUpStatus;

    [SerializeField]
    Text m_Player2PowerUpStatus;

    [SerializeField]
    GameObject m_Player1FrozenStatus;

    [SerializeField]
    GameObject m_Player2FrozenStatus;

    [SerializeField]
    GameObject m_PopupPrefab;

    [SerializeField]
    Color m_P1_Popup_Colour;

    [SerializeField]
    Color m_P2_Popup_Colour;

    //-------------------------------------------
    
    GameObject m_currentPopUp;

    // --------------------------------------------------------------
    
    void OnEnable()
    {
        GameController.OnResetGame += OnResetUI;
    }

    void OnDisable()
    {
        GameController.OnResetGame -= OnResetUI;
    }

    //-------------------------------------------
    
    void OnResetUI()
    {
        m_GameText.text = GameController.INSTANCE.GetCountdown().ToString("0");
        m_Player1ScoreText.text = "0";
        m_Player2ScoreText.text = "0";
        m_TimeLeftText.text = GameController.INSTANCE.GetTime().ToString("0.0") + " secs left";
    }

    //-------------------------------------------

    public void SetGameText(string gameText)
    {
        m_GameText.text = gameText;
    }

    public void CreatePopUp(string popupText, int player, Color popupColour)
    {
        if (m_currentPopUp != null)
        {
            Destroy(m_currentPopUp);
        }

        m_currentPopUp = Instantiate(m_PopupPrefab, FindObjectOfType<Canvas>().gameObject.transform);

        m_currentPopUp.GetComponentInChildren<Text>().text = popupText;

        m_currentPopUp.GetComponentInChildren<Text>().color = popupColour;

        if (player == 1)
        {
            m_currentPopUp.GetComponent<Image>().color = m_P1_Popup_Colour;
        }
        else if (player == 2)
        {
            m_currentPopUp.GetComponent<Image>().color = m_P2_Popup_Colour;
        }
        else
        {
            m_currentPopUp.GetComponent<Image>().color = Color.black;
        }
    }

    public void SetPowerUpStatus(int playerNum, string statusText, Color statusColour)
    {
        if (playerNum == 1)
        {
            m_Player1PowerUpStatus.text = statusText;
            m_Player1PowerUpStatus.color = statusColour;
        }
        else if (playerNum == 2)
        {
            m_Player2PowerUpStatus.text = statusText;
            m_Player2PowerUpStatus.color = statusColour;
        }
    }

    public void SetFrozenStatus(int playerNum, bool isFrozen)
    {
        if (playerNum == 1)
        {
            m_Player1FrozenStatus.SetActive(isFrozen);
        }
        else if (playerNum == 2)
        {
            m_Player2FrozenStatus.SetActive(isFrozen);
        }
    }

    public void UpdateCountdown(float countdown)
    {
        if (countdown > 0.0f)
        {
            m_GameText.text = ((int)++countdown).ToString("0");
        }
        else
        {
            m_GameText.text = "";
        }

    }

    public void UpdateScore(int playerNum, int playerScore)
    {
        if (playerNum == 1)
        {
            m_Player1ScoreText.text = "" + playerScore;
        }
        else if (playerNum == 2)
        {
            m_Player2ScoreText.text = "" + playerScore;
        }
    }

    public void UpdateTime(float time)
    {
        m_TimeLeftText.text = time.ToString("0.0") + " secs left";
    }

    public void GameEnd(int winningPlayer, string playerColour)
    {
        if (winningPlayer == 0)
        {
            m_GameText.text = "DRAW!";
        }
        else
        {
            m_GameText.text = "<color="+playerColour+">Player " + winningPlayer + "</color>\nWINS!";
        }
    }

}
