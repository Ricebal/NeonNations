using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody m_rigidbody;
    private Player m_player;
    private Action m_action;
    private Dash m_playerDash;

    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_rigidbody = GetComponent<Rigidbody>();
        m_player = GetComponent<Player>();
        m_action = GetComponent<Action>();
        m_playerDash = GetComponent<Dash>();
    }

    void Update()
    {
        if (!isLocalPlayer)
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

    // TODO: Remove (used to debug)
    Vector3 oldPos;

    void FixedUpdate()
    {

        // TODO: Remove (used to debug)
        float speed = Vector3.Distance(transform.position, oldPos) / Time.deltaTime;
        if (speed > 0)
        {
            Debug.Log(transform.name + " : " + speed);
        }
        oldPos = transform.position;

        if (!isLocalPlayer)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        // If dashing, normalize movement vector so you are always at max speed
        if (m_playerDash.IsDashing())
        {
            movement.Normalize();
        }

        m_rigidbody.velocity = movement * m_player.Speed * m_playerDash.GetMultiplier();
    }

    void OnDisable()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_rigidbody.velocity = new Vector3(0, 0, 0);
    }

}
