using UnityEngine;

public class LightController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
    }

}
