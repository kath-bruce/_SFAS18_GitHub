using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    //-------------------------------------------

    [SerializeField]
    GameObject m_powerupPrefab;

    //-------------------------------------------

    GameObject m_currentPowerUp;

    PowerUpType m_currentPowerUpType = PowerUpType.NULL;

    const float SPAWN_TIMER = 5.0f;
    float m_timeUntilSpawn = SPAWN_TIMER;

    bool m_isPowerUpCurrentlySpawned = false;

    //-------------------------------------------

    void OnEnable()
    {
        GameController.OnResetGame += OnResetSpawner;
    }

    void OnDisable()
    {
        GameController.OnResetGame -= OnResetSpawner;
    }

    void Update()
    {
        if (GameController.INSTANCE.InCountdown() || GameController.INSTANCE.Paused())
            return;

        m_timeUntilSpawn -= Time.deltaTime;

        if (m_timeUntilSpawn <= 0.0f)
        {
            //replace power up
            if (m_isPowerUpCurrentlySpawned)
            {
                Destroy(m_currentPowerUp);
            }

            m_isPowerUpCurrentlySpawned = true;

            m_timeUntilSpawn = SPAWN_TIMER;

            m_currentPowerUp = Instantiate(m_powerupPrefab, transform);

            ChangePowerUp();
        }
    }

    //-------------------------------------------

    void OnResetSpawner()
    {
        if (m_currentPowerUp != null)
        {
            Destroy(m_currentPowerUp);
        }

        m_isPowerUpCurrentlySpawned = false;

        m_timeUntilSpawn = SPAWN_TIMER;

        m_currentPowerUpType = PowerUpType.NULL;
    }

    void ChangePowerUp()
    {
        //generate random power up
        m_currentPowerUpType = (PowerUpType)Random.Range(1, System.Enum.GetNames(typeof(PowerUpType)).Length);

        switch (m_currentPowerUpType)
        {
            case PowerUpType.INVINCIBILITY:
                ChangePowerUpText("Invincibility");
                ChangePowerUpColour(Color.magenta);
                break;

            case PowerUpType.POINT_STEALER:
                ChangePowerUpText("Point Stealer");
                ChangePowerUpColour(Color.yellow);
                break;

            case PowerUpType.FREEZE:
                ChangePowerUpText("Freeze");
                ChangePowerUpColour(Color.cyan);
                break;

            default:
                break;
        }
    }

    void ChangePowerUpText(string new_text)
    {
        TextMesh powerUpText = m_currentPowerUp.GetComponentInChildren<TextMesh>();
        powerUpText.text = new_text;
    }

    void ChangePowerUpColour(Color colour)
    {
        Material powerUpMat = m_currentPowerUp.GetComponentInChildren<MeshRenderer>().material;

        powerUpMat.SetColor("_EmissionColor", colour);

        powerUpMat.color = colour;

        Light powerUpLight = m_currentPowerUp.GetComponentInChildren<Light>();

        powerUpLight.color = colour;
    }

    //-------------------------------------------

    public void PowerUpCollected(GameObject player_go)
    {
        Destroy(m_currentPowerUp);

        m_isPowerUpCurrentlySpawned = false;

        m_timeUntilSpawn = SPAWN_TIMER;

        //give player powerup

        PlayerController player = player_go.GetComponent<PlayerController>();

        player.SetPlayerPowerUpType(m_currentPowerUpType);
        player.PlayClip(GameController.INSTANCE.GetClip("PowerUpPickUp"));

        GameController.INSTANCE.SetPowerUpStatus(player.GetPlayerNum(), m_currentPowerUpType);
        GameController.INSTANCE.CreateSparkles(player.gameObject.transform.position);

        m_currentPowerUpType = PowerUpType.NULL;
    }
}
