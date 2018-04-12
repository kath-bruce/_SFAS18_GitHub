using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // --------------------------------------------------------------

    // The character's running speed
    [SerializeField]
    float m_RunSpeed = 5.0f;

    // The gravity strength
    [SerializeField]
    float m_Gravity = 60.0f;

    // The maximum speed the character can fall
    [SerializeField]
    float m_MaxFallSpeed = 20.0f;

    // The character's jump height
    [SerializeField]
    float m_JumpHeight = 4.0f;

    // Identifier for Input
    [SerializeField]
    string m_PlayerInputString = "_P1";

    //minimum distance to ball in order to kick it
    [SerializeField]
    float m_kickDistance = 5.0f;

    [SerializeField]
    float m_kickForce = 300.0f;

    //minimum distance to other player in order to steal point or freeze
    [SerializeField]
    float m_PlayerCollisionDistance = 1.5f;

    [SerializeField]
    GameObject m_P1_Spawner;

    [SerializeField]
    GameObject m_P2_Spawner;

    [SerializeField]
    Material m_P1_frozenMat;

    [SerializeField]
    Material m_P2_frozenMat;

    // --------------------------------------------------------------

    // The charactercontroller of the player
    CharacterController m_CharacterController;

    AudioSource m_audioSrc;

    // The current movement direction in x & z.
    Vector3 m_MovementDirection = Vector3.zero;

    // The current movement speed
    float m_MovementSpeed = 0.0f;

    // The current vertical / falling speed
    float m_VerticalSpeed = 0.0f;

    // The current movement offset
    Vector3 m_CurrentMovementOffset = Vector3.zero;

    // Whether the player is alive or not
    bool m_IsAlive = true;

    // The time it takes to respawn
    const float MAX_RESPAWN_TIME = 1.0f;
    float m_RespawnTime = MAX_RESPAWN_TIME;

    PowerUpType m_powerUpType = PowerUpType.NULL;

    const float FROZEN_TIME = 3.0f;
    float m_FrozenTimeLeft = FROZEN_TIME;

    bool m_IsFrozen = false;

    Material m_defaultMat;

    int m_playerNum = 0;
    int m_otherPlayerNum = 0;

    // --------------------------------------------------------------

    void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_defaultMat = GetComponent<MeshRenderer>().material;
        m_audioSrc = GetComponent<AudioSource>();
        m_playerNum = GetPlayerNum();
        m_otherPlayerNum = GetOtherPlayerNum();
    }

    void OnEnable()
    {
        GameController.OnResetGame += OnResetPlayer;
    }

    void OnDisable()
    {
        GameController.OnResetGame -= OnResetPlayer;
    }

    void Update()
    {
        if (GameController.INSTANCE.HasTimeRunOut() || GameController.INSTANCE.InCountdown() || GameController.INSTANCE.Paused())
            return;

        // If the player is frozen update the frozen timer and exit update loop
        if (m_IsFrozen)
        {
            UpdateFrozenTime();
            return;
        }

        // If the player is dead update the respawn timer and exit update loop
        if (!m_IsAlive)
        {
            UpdateRespawnTime();
            return;
        }

        // Update movement input
        UpdateMovementState();

        // Update jumping input and apply gravity
        UpdateJumpState();
        ApplyGravity();

        // Calculate actual motion
        m_CurrentMovementOffset = (m_MovementDirection * m_MovementSpeed + new Vector3(0, m_VerticalSpeed, 0)) * Time.deltaTime;

        // Move character
        m_CharacterController.Move(m_CurrentMovementOffset);

        // Rotate the character in movement direction
        if (m_MovementDirection != Vector3.zero)
        {
            RotateCharacter(m_MovementDirection);
        }

        //check if player can kick the ball
        CheckKick();

        //check if players have collided with a powerup in play
        CheckPowerupCollision();
    }

    //----------------------------------------------

    void OnResetPlayer()
    {
        Respawn();
    }

    void Jump()
    {
        m_VerticalSpeed = Mathf.Sqrt(m_JumpHeight * m_Gravity);
    }

    void ApplyGravity()
    {
        // Apply gravity
        m_VerticalSpeed -= m_Gravity * Time.deltaTime;

        // Make sure we don't fall any faster than m_MaxFallSpeed.
        m_VerticalSpeed = Mathf.Max(m_VerticalSpeed, -m_MaxFallSpeed);
        m_VerticalSpeed = Mathf.Min(m_VerticalSpeed, m_MaxFallSpeed);
    }

    void UpdateMovementState()
    {
        // Get Player's movement input and determine direction and set run speed
        float horizontalInput = Input.GetAxisRaw("Horizontal" + m_PlayerInputString);
        float verticalInput = Input.GetAxisRaw("Vertical" + m_PlayerInputString);

        m_MovementDirection = new Vector3(horizontalInput, 0, verticalInput);
        m_MovementSpeed = m_RunSpeed;
    }

    void UpdateJumpState()
    {
        // Character can jump when standing on the ground
        if (Input.GetButtonDown("Jump" + m_PlayerInputString) && m_CharacterController.isGrounded)
        {
            Jump();
            PlayClip(GameController.INSTANCE.GetClip("PlayerJump"));
        }
    }

    void UpdateFrozenTime()
    {
        m_FrozenTimeLeft -= Time.deltaTime;

        if (m_FrozenTimeLeft <= 0.0f)
        {
            m_IsFrozen = false;
            GameController.INSTANCE.SetFrozenStatus(m_playerNum, m_IsFrozen);
            GetComponent<MeshRenderer>().material = m_defaultMat;
            m_FrozenTimeLeft = FROZEN_TIME;
        }
    }

    void CheckKick()
    {
        //player can kick ball if within minimum distance to ball
        if (Input.GetButtonDown("Kick" + m_PlayerInputString)
            && Vector3.Distance(GameController.INSTANCE.GetBallGO().transform.position, transform.position) < m_kickDistance)
        {
            Rigidbody ball_rb = GameController.INSTANCE.GetBallGO().GetComponent<Rigidbody>();

            Vector3 explosion_pos = transform.position;
            explosion_pos.y -= 1.5f;

            ball_rb.AddExplosionForce(m_kickForce, explosion_pos, 10.0f);

            GameController.INSTANCE.GetBallGO().GetComponent<BallManager>().SetWinningPlayer(m_playerNum);
            GameController.INSTANCE.CreateSparkles(transform.position);
            GameController.INSTANCE.GetBallGO().GetComponent<BallManager>().PlayClip(GameController.INSTANCE.GetClip("Kick"));
        }
    }

    void CheckPowerupCollision()
    {
        //can affect other player with power up if not frozen and within minimum distance to other player
        if (Vector3.Distance(GameController.INSTANCE.GetPlayerGO(m_otherPlayerNum).transform.position, transform.position) < m_PlayerCollisionDistance)
        {
            if (m_powerUpType == PowerUpType.POINT_STEALER)
            {
                //steal other players point
                GameController.INSTANCE.AddToPlayerScore(m_playerNum, 1);
                GameController.INSTANCE.AddToPlayerScore(m_otherPlayerNum, -1);

                GameController.INSTANCE.CreatePopUp("Player " + m_playerNum + " stole a point!", m_playerNum, Color.yellow);

                m_powerUpType = PowerUpType.NULL;

                GameController.INSTANCE.SetPowerUpStatus(m_playerNum, PowerUpType.NULL);
                PlayClip(GameController.INSTANCE.GetClip("StealingPoint"));
            }
            else if (m_powerUpType == PowerUpType.FREEZE)
            {
                //freeze other player
                GameController.INSTANCE.GetPlayerGO(m_otherPlayerNum).GetComponent<PlayerController>().FreezePlayer();

                m_powerUpType = PowerUpType.NULL;

                GameController.INSTANCE.SetPowerUpStatus(m_playerNum, PowerUpType.NULL);
                PlayClip(GameController.INSTANCE.GetClip("Frozen"));
            }
        }
    }

    void RotateCharacter(Vector3 movementDirection)
    {
        Quaternion lookRotation = Quaternion.LookRotation(movementDirection);
        if (transform.rotation != lookRotation)
        {
            transform.rotation = lookRotation;
        }
    }

    void UpdateRespawnTime()
    {
        m_RespawnTime -= Time.deltaTime;
        if (m_RespawnTime < 0.0f)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        m_IsAlive = true;

        if (m_playerNum == 1)
        {
            transform.position = m_P1_Spawner.transform.position;
        }
        else if (m_playerNum == 2)
        {
            transform.position = m_P2_Spawner.transform.position;
        }
        else
        {
            transform.position = Vector3.zero;
        }

        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        m_powerUpType = PowerUpType.NULL;
        GameController.INSTANCE.SetPowerUpStatus(m_playerNum, PowerUpType.NULL);
        m_IsFrozen = false;
        m_FrozenTimeLeft = FROZEN_TIME;
        GameController.INSTANCE.SetFrozenStatus(m_playerNum, m_IsFrozen);
        GetComponent<MeshRenderer>().material = m_defaultMat;
    }

    //----------------------------------------------

    public PowerUpType GetPlayerPowerUpType()
    {
        return m_powerUpType;
    }

    public void SetPlayerPowerUpType(PowerUpType new_powerUp)
    {
        m_powerUpType = new_powerUp;
    }
    
    public void FreezePlayer()
    {
        m_IsFrozen = true;

        GameController.INSTANCE.CreatePopUp("Player " + m_playerNum + " is frozen!", m_playerNum, Color.cyan);
        GameController.INSTANCE.SetFrozenStatus(m_playerNum, m_IsFrozen);

        if (m_playerNum == 1)
        {
            GetComponent<MeshRenderer>().material = m_P1_frozenMat;
        }
        else if (m_playerNum == 2)
        {
            GetComponent<MeshRenderer>().material = m_P2_frozenMat;
        }
    }

    public void PlayClip(AudioClip clip)
    {
        m_audioSrc.clip = clip;
        m_audioSrc.Play();
    }
    
    public int GetPlayerNum()
    {
        if (m_playerNum == 0)
        {
            if (m_PlayerInputString == "_P1")
            {
                m_playerNum = 1;
            }
            else if (m_PlayerInputString == "_P2")
            {
                m_playerNum = 2;
            }
        }

        return m_playerNum;
    }

    public int GetOtherPlayerNum()
    {
        if (m_otherPlayerNum == 0)
        {
            if (m_playerNum == 1)
            {
                m_otherPlayerNum = 2;
            }
            else if (m_playerNum == 2)
            {
                m_otherPlayerNum = 1;
            }
        }

        return m_otherPlayerNum;
    }

    public void Die()
    {
        m_IsAlive = false;
        m_RespawnTime = MAX_RESPAWN_TIME;
    }
    
    public bool IsAlive()
    {
        return m_IsAlive;
    }
}
