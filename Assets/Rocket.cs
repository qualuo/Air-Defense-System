using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Unguided projectile */

public class Rocket : MonoBehaviour
{
    void FixedUpdate() 
    {
        if (GetComponent<Rigidbody>().velocity.magnitude == 0) return;
        transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity); // Turns with velocity

        if (transform.position.y < -5) {
            // Out of bounds
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("VisualOnly")) { // Ignore (e.g. dome visual)
            return;
        }

        Detonate();
    }

    private void Detonate() {
        // TODO: Explosion!
        Destroy(gameObject);
    }
}
