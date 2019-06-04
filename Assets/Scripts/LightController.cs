using Mirror;

public class LightController : NetworkBehaviour
{

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

}
