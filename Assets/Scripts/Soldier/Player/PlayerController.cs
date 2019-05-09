using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody m_rigidbody;
    private Player m_player;
    private Action m_action;
    private DashController m_playerDash;
    private bool m_isEnabled;

    public void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_rigidbody = GetComponent<Rigidbody>();
        m_player = GetComponent<Player>();
        m_action = GetComponent<Action>();
        m_playerDash = GetComponent<DashController>();
        SetEnabled(true);
    }

    public void Update()
    {
        if (!isLocalPlayer || !m_isEnabled)
        {
            return;
        }

        // If a fire key is held down
        if (Input.GetButton("Fire1"))
        {
            m_action.Shoot();
        }

        // If the 'space' key is held down
        if (Input.GetButton("Jump"))
        {
            m_action.Sonar();
        }

        // If shift or mouse3 is pressed
        if (Input.GetButton("Fire3"))
        {
            m_action.Dash();
        }
    }

    public void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Vector3 movement = new Vector3();
        // If player controller is enabled, take into account user's inputs
        if (m_isEnabled)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            movement = new Vector3(moveHorizontal, 0, moveVertical);

            // If dashing, normalize movement vector so you are always at max speed
            if (m_playerDash.IsDashing())
            {
                movement.Normalize();
            }
        }

        m_rigidbody.velocity = movement * m_player.Speed * m_playerDash.GetMultiplier();
    }

    // Enable / disable player's movements
    public void SetEnabled(bool isEnabled)
    {
        m_isEnabled = isEnabled;
    }

}
