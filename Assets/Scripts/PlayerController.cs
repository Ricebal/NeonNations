using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float Speed;

    public GameObject Shot;
    public GameObject Sonar;
    public Transform ShotSpawn;
    // Fire rate in seconds
    public float FireRate;
    // Sonar rate in seconds
    public float SonarRate;
    // Amiunt of energy a bullet will consume
    public int BulletCost;
    // Amount of energy a sonar will consume
    public int SonarCost;

    private Rigidbody m_rigidBody;
    // The next time the entity will be able to shoot in seconds
    private float m_nextFire;
    // The next time the entity will be able to use the sonar in seconds
    private float m_nextSonar;
    private PlayerEnergy m_playerEnergy;
    private GameObject m_escapeMenu;

    public void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
        if(!isLocalPlayer)
            return;
        m_playerEnergy = GetComponent<PlayerEnergy>();
        m_escapeMenu = GameObject.Find("MenuCanvas").transform.GetChild(0).gameObject;
    }

    public void Update() {
        if(!isLocalPlayer) {
            return;
        }

        // If the 'ctrl' key is held down, the entity is able to shoot and has enough energy
        if(Input.GetButton("Fire1") && Time.time > m_nextFire && m_playerEnergy.CurrentEnergy >= BulletCost && !m_escapeMenu.activeSelf) {
            m_nextFire = Time.time + FireRate;
            m_playerEnergy.AddEnergy(-BulletCost);
            CmdShoot();
        }

        // If the 'space' key is held down, the entity is able to use the sonar and has enough energy
        if(Input.GetButton("Jump") && Time.time > m_nextSonar && m_playerEnergy.CurrentEnergy >= SonarCost) {
            m_nextSonar = Time.time + SonarRate;
            m_playerEnergy.AddEnergy(-SonarCost);
            CmdSonar();
        }
    }

    [Command]
    public void CmdShoot() {
        GameObject bullet = Instantiate(Shot, ShotSpawn.position, ShotSpawn.rotation) as GameObject;

        // Instanciate the bullet on the network for all players 
        NetworkServer.Spawn(bullet);
    }

    [Command]
    public void CmdSonar() {
        for(int i = 0; i < 360; i += 2) {
            GameObject sonarBullet = Instantiate(Sonar, ShotSpawn.position, ShotSpawn.rotation) as GameObject;
            sonarBullet.transform.RotateAround(this.gameObject.transform.position, new Vector3(0.0f, 1.0f, 0.0f), i);

            // Instanciate the bullet on the network for all players 
            NetworkServer.Spawn(sonarBullet);
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
        m_playerEnergy.AddEnergy(1);
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
