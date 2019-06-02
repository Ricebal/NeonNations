using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    [SerializeField] private Rigidbody m_pivot;
    private Vector3 m_height = new Vector3(0f, 0.3f, 0f);
    private float m_speed;
    private const float SHIFTMULTIPLIER = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        m_speed = m_pivot.gameObject.GetComponent<Soldier>().Speed;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the direction of the player
        Vector3 velocityDiretion = m_pivot.velocity;
        velocityDiretion.Normalize();
        // Get the magnitude of the velocity, limited to max speed to prevent decapitation when dashing
        float velocityAmount = Mathf.Min(m_pivot.velocity.magnitude, m_speed);
        // Get a number between 0 and 1 based on the velocity magnitude between 0 and speed
        velocityAmount /= m_speed;
        velocityAmount *= SHIFTMULTIPLIER;
        // Apply the amount to the direction
        velocityDiretion *= velocityAmount;
        // Position is the playerposition + a constant height so that the head is on top + the direction of the movement
        transform.position = m_pivot.position + m_height + velocityDiretion;
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
