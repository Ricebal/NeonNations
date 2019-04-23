using UnityEngine.Networking;

public class LightController : NetworkBehaviour
{
    void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
