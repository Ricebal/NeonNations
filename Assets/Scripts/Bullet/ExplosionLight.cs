using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLight : MonoBehaviour
{
    public float RangeMultiplier;
    public float IntensityMultiplier;
    public float Lifetime;
    public Light Light;

    // Start is called before the first frame update
    void Start()
    {
        //Destroys explosion after certain time
        Destroy(gameObject, Lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        Light.range += RangeMultiplier * Time.deltaTime;
        Light.intensity += IntensityMultiplier * Time.deltaTime;
    }
}
