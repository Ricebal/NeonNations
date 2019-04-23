using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float Speed;

    private Rigidbody m_rigidbody;
    private Player m_player;
    private bool m_isEnabled;

    public void Start() {
        if (!isLocalPlayer) {
            return;
        }

        m_rigidbody = GetComponent<Rigidbody>();
        m_player = GetComponent<Player>();
        SetEnabled(true);
    }

    public void Update() {
        if (!isLocalPlayer || !m_isEnabled) {
            return;
        }

        // If a fire key is held down
        if (Input.GetButton("Fire1")) {
            m_player.Shoot();
        }

        // If the 'space' key is held down
        if (Input.GetButton("Jump")) {
            m_player.Sonar();
        }
    }

    public void FixedUpdate() {
        if (!isLocalPlayer || !m_isEnabled) {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        m_rigidbody.velocity = movement * Speed;
    }

    // Set player's speed to 0
    public void Freeze() {
        m_rigidbody.velocity = new Vector3(0, 0, 0);
    }

    // Enable / disable player's movements
    public void SetEnabled(bool isEnabled) {
        m_isEnabled = isEnabled;
    }

}
