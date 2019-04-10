using UnityEngine;

public class BulletMover : MonoBehaviour
{

    public float Speed;

    public void Start() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        // Move the bullet straight ahead with constant speed
        rigidbody.velocity = transform.forward * Speed;
    }

    public void OnCollisionEnter(Collision collision) {
        // Destroy the bullet when it hits a wall
        if (collision.gameObject.name == "Wall") {
            Destroy(this.gameObject);
        }
    }

}
