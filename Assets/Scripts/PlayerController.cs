using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float Speed;

    public GameObject Shot;
    public Transform ShotSpawn;
    // Fire rate in seconds
    public float FireRate;

    private Rigidbody m_rigidBody;
    // The next time the entity will be able to shoot in seconds
    private float m_nextFire;

    public void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    public void Update() {
        if(!isLocalPlayer) {
            return;
        }

        // If the 'ctrl' key is held down and the entity is able to shoot
        if(Input.GetButton("Fire1") && Time.time > m_nextFire) {
            m_nextFire = Time.time + FireRate;
            CmdShoot();
        }
    }

    [Command]
    public void CmdShoot() {
        GameObject bullet = Instantiate(Shot, ShotSpawn.position, ShotSpawn.rotation) as GameObject;

        // Instanciate the bullet on the network for all players 
        NetworkServer.Spawn(bullet);
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
    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<CameraController>().setTarget(gameObject.transform);
    }

    void OnDestroy()
    {
        if (isLocalPlayer)
        {
            Camera.main.GetComponent<CameraController>().setInactive();
            Camera.main.GetComponent<CameraController>().playerTransform = null;
        }
    }

}
