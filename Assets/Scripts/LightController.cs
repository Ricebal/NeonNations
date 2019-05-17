using Mirror;

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

}
