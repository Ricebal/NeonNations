using Mirror;
using UnityEngine;

public class BreakableWall : NetworkBehaviour
{
    public delegate void OnWallDestroyedDelegate(Vector2Int coordinates);
    public event OnWallDestroyedDelegate WallDestroyedHandler;

    [SerializeField] protected Stat m_healthStat;
    [SerializeField] private AudioClip m_hitSound;
    [SerializeField] private float m_hitSoundVolume;
    [SerializeField] private AudioClip m_destroySound;
    [SerializeField] private float m_destroySoundVolume;
    private AudioSource m_audioSource;
    // Game object used to play destroy sound after the breakable wall has been destroyed
    private GameObject m_audioObject;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_audioObject = new GameObject();
        m_audioObject.transform.position = transform.position;
        m_audioObject.transform.parent = transform;
    }

    // If the BreakableWall gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the BreakableWall took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            m_healthStat.Subtract(collider.gameObject.GetComponent<Bullet>().Damage);

            if (m_healthStat.GetValue() <= 0)
            {
                CmdDestroyWall();
            }
            else // Play hit-sound.
            {
                m_audioSource.pitch = Random.Range(0.75f, 1.25f);
                m_audioSource.PlayOneShot(m_hitSound, m_hitSoundVolume);
            }
            return true;
        }
        return false;
    }

    [Command]
    private void CmdDestroyWall()
    {
        RpcDestroyWall();
    }

    [ClientRpc]
    private void RpcDestroyWall()
    {
        // Play destroy-sound.
        m_audioSource.PlayOneShot(m_destroySound, m_destroySoundVolume);
        m_audioObject.transform.parent = null;
        Destroy(m_audioObject, 1); // Destroy audio after 1 second.
        Destroy(gameObject); // Destroy wall instantly.

        if (WallDestroyedHandler != null)
        {
            Vector3 position = gameObject.transform.position;
            Vector2Int coordinates = new Vector2Int((int)position.x, (int)position.z);
            WallDestroyedHandler(coordinates);
        }
    }
}
