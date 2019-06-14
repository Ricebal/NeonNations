/**
 * Authors: Chiel
 */

using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Singleton;
    public Transform PlayerTransform;
    public float DistanceFromPlayer = 11;
    public double CameraTilt = 80;

    private Vector3 m_offset;
    private bool m_active;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    private void LateUpdate()
    {
        // for debugging
        transform.eulerAngles = new Vector3((float)CameraTilt, 0, 0);
        m_offset = new Vector3(0, 0, 0);
        m_offset.y = (float)Math.Cos(Math.PI / 180 * (90 - CameraTilt)) * DistanceFromPlayer; // cos(θ) = Adjacent / Hypotenuse
        m_offset.z = (float)Math.Sin(Math.PI / 180 * (90 - CameraTilt)) * -DistanceFromPlayer; // sin(θ) = Opposite / Hypotenuse

        if (m_active)
        {
            transform.position = PlayerTransform.position + m_offset;
        }
    }

    public static void SetTarget(Transform target)
    {
        Singleton.m_active = true;
        Singleton.PlayerTransform = target;
        Singleton.transform.position = Singleton.PlayerTransform.position;
    }

    public static void SetInactive()
    {
        Singleton.m_active = false;
    }
}
