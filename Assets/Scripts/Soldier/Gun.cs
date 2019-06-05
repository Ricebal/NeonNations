using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
