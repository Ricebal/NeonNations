using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    public GameObject DirectionalLight;
    // Start is called before the first frame update
    void Start() {
        DirectionalLight.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            DirectionalLight.SetActive(!DirectionalLight.activeSelf);
        }
    }
}
