using Mirror;
using UnityEngine;

public class LightController : NetworkBehaviour
{
    [SerializeField] private GameObject m_pointLight = null;
    [SerializeField] private GameObject m_spotLight = null;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        m_pointLight.SetActive(true);
        m_spotLight.SetActive(true);
    }

}
