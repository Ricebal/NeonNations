using UnityEngine;
using UnityEngine.Networking;

public class SonarTry : NetworkBehaviour
{
    public float RangeMultiplier;
    public float IntensityMultiplier;
    public float Lifetime;
    public Light Light;
    private float _timeCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Destroys explosion after certain time
        NetworkBehaviour.Destroy(gameObject, Lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        _timeCount += Time.deltaTime;
        Light.range += RangeMultiplier * Time.deltaTime;
        Light.intensity += IntensityMultiplier * Time.deltaTime;
    }
}
