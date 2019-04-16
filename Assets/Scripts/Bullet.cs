﻿using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    // Bullet speed
    public float Speed;
    // Time in seconds before the bullet is destroyed
    public float LivingTime;
    // The explosion on impact
    public GameObject hitPrefab;
    // Damage done to a player on hit
    public int Damage;

    // The player that shot the bullet
    [SyncVar]
    private GameObject m_shooter;
    private float m_spawnTime;

    public void Start() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        // Move the bullet straight ahead with constant speed
        rigidbody.velocity = transform.forward * Speed;

        m_spawnTime = Time.time;
    }

    public void Update() {
        // Destroy the bullet when the LivingTime is elapsed
        if (Time.time - m_spawnTime > LivingTime) {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider collider) {
        if (!isServer) {
            return;
        }

        if(collider.gameObject != m_shooter) {
            // The bullet is destroyed on collision

            //Transform pos = collider.ClosestPoint(transform);
            //if(collider.gameObject == )
            if(hitPrefab != null)
            {
                hitPrefab = Instantiate(hitPrefab, transform);
            }
            NetworkBehaviour.Destroy(this.gameObject);
        }
    }

    public void SetShooter(GameObject shooter) {
        m_shooter = shooter;
    }

    public GameObject GetShooter() {
        return m_shooter;
    }

}
