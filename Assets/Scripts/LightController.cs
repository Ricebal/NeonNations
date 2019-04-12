using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightController : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer)
            return;

        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
