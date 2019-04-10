using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public float distanceFromPlayer = 11;
    public double cameraTilt = 80;

    private Vector3 offset;

    private bool active;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // for debugging
        transform.eulerAngles = new Vector3((float)cameraTilt, 0, 0);
        offset =  new Vector3(0, 0, 0);
        offset.y = (float)Math.Cos(Math.PI / 180 * (90 - cameraTilt)) * distanceFromPlayer; // cos(θ) = Adjacent / Hypotenuse
        offset.z = (float)Math.Sin(Math.PI / 180 * (90 - cameraTilt)) * -distanceFromPlayer; // sin(θ) = Opposite / Hypotenuse

        if (active)
            transform.position = playerTransform.position + offset;
    }

    public void setTarget(Transform target)
    {
        active = true;
        playerTransform = target;
        transform.position = playerTransform.position;
    }

    public void setInactive()
    {
        active = false;
    }
}
