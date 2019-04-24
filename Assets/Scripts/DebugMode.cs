using UnityEngine;

public class DebugMode : MonoBehaviour
{
    public GameObject DirectionalLight;

    void Start()
    {
        DirectionalLight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DirectionalLight.SetActive(!DirectionalLight.activeSelf);
        }
    }
}
