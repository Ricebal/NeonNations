using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLight : MonoBehaviour
{
    public float RangeMultiplier;
    public float IntensityMultiplier;
    public float Lifetime;
    public Light Light;
    private bool _growing = true;
    private float _timeCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Destroys explosion after certain time
        Destroy(gameObject, Lifetime*2);
    }

    // Update is called once per frame
    void Update()
    {
        _timeCount += Time.deltaTime;
        if(_timeCount > Lifetime && _growing)
        {
            _growing = false;
        }
        if (_growing)
        {
            Light.range += RangeMultiplier * Time.deltaTime;
            Light.intensity += IntensityMultiplier * Time.deltaTime;
        }
        else
        {
            Light.range -= RangeMultiplier * Time.deltaTime;
            Light.intensity -= IntensityMultiplier * Time.deltaTime;
        }
    }
}
