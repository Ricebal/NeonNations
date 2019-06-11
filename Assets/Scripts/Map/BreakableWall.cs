using Mirror;
using UnityEngine;

public class BreakableWall : NetworkBehaviour
{
    public delegate void OnWallDestroyedDelegate(Vector2Int coordinates);
    public event OnWallDestroyedDelegate WallDestroyedHandler;

    private Stat m_healthStat = new Stat(0, 50);

    // If the BreakableWall gets hit by a bullet, it will take damage. Will return true if the collider was a Bullet and the BreakableWall took damage.
    protected bool OnTriggerEnter(Collider collider)
    {
        if (!isServer)
        {
            return false;
        }
        if (collider.gameObject.tag == "Bullet")
        {
            m_healthStat.Subtract(collider.gameObject.GetComponent<Bullet>().Damage);

            if (m_healthStat.GetValue() <= 0)
            {
                RpcDestroyWall();
            }
            return true;
        }
        return false;
    }

    [ClientRpc]
    private void RpcDestroyWall()
    {
        Destroy(gameObject);
        if (WallDestroyedHandler != null)
        {
            Vector3 position = gameObject.transform.position;
            Vector2Int coordinates = new Vector2Int((int)position.x, (int)position.z);
            WallDestroyedHandler(coordinates);
        }
    }
}
