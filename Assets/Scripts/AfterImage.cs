using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AfterImage : NetworkBehaviour
{
    public Renderer MyRenderer;
    public Light MyLight;
    private float m_initialIntensity;
    private const float LIFESPAN = 1f;

    // Start is called before the first frame update
    void Start()
    {
        m_initialIntensity = MyLight.intensity;
        Destroy(this.gameObject, LIFESPAN);
    }

    [ClientRpc]
    public void RpcSetColor(Color color)
    {
        MyRenderer.material.color = color;
        MyLight.color = new Color(color.r, color.g, color.b, 1);
    }

    // Update is called once per frame
    void Update()
    {
        // Set opacity between 0.5 and 0 based on lifespan
        Color oldColor = MyRenderer.material.color;
        float newAlpha = oldColor.a - ((Time.deltaTime / LIFESPAN) / 2);
        MyRenderer.material.color = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);

        // Set light intensity between 0.5 and 0 based on lifespan
        float newIntensity = MyLight.intensity - ((Time.deltaTime / LIFESPAN) * m_initialIntensity);
        MyLight.intensity = newIntensity;
    }
}
