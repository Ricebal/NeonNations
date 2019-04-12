using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float Speed;

    private Rigidbody m_rigidBody;
    private Player m_player;
    private GameObject m_escapeMenu;
    private CameraController m_cameraController;

    public void Start() {
        if (!isLocalPlayer) {
            return;
        }
        m_rigidBody = GetComponent<Rigidbody>();
        m_player = GetComponent<Player>();
        m_escapeMenu = GameObject.Find("MenuCanvas").transform.GetChild(0).gameObject;
        m_cameraController = Camera.main.GetComponent<CameraController>();
    }

    public void Update() {
        if (!isLocalPlayer || m_escapeMenu.activeSelf) {
            return;
        }

        // If the 'ctrl' key is held down
        if (Input.GetButton("Fire1")) {
            m_player.Shoot();
        }

        // If the 'space' key is held down
        if (Input.GetButton("Jump")) {
            m_player.Sonar();
        }
    }

    public void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        m_rigidBody.velocity = movement * Speed;
    }

    public override void OnStartLocalPlayer() {
        m_cameraController.setTarget(gameObject.transform);
    }

    void OnDestroy() {
        if (!isLocalPlayer) {
            return;
        }

        m_cameraController.setInactive();
        m_cameraController.PlayerTransform = null;
    }

}
