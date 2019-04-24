using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public Renderer MyRenderer;
    public Light MyLight;
    private const float LIFESPAN = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, LIFESPAN);
    }

    // Update is called once per frame
    void Update()
    {
        Color oldColor = MyRenderer.material.color;
        float newAlpha = oldColor.a - ((Time.deltaTime / LIFESPAN) / 2);
        MyRenderer.material.color = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);

        float newIntensity = MyLight.intensity - ((Time.deltaTime / LIFESPAN) / 2);
        MyLight.intensity = newIntensity;
    }
}
