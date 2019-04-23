using UnityEngine;

public class LightController : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
    }

}
