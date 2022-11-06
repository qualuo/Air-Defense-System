using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignMissileLauncher : MonoBehaviour
{
    public GameObject missilePrefab;
    [SerializeField]
    private float missilesPerSecond;
    [SerializeField]
    private float launchForceMultiplier;
    [SerializeField]
    private float minFiringDistance;
    [SerializeField]
    private float maxFiringDistance;


    private float timeSinceFire = 0.0f;

    void Start()
    {
        if (!missilePrefab) {
            // Notify missing
        }
    }

    void Update()
    {
        timeSinceFire += Time.deltaTime;
        if (timeSinceFire >= 1/missilesPerSecond) {
            StartCoroutine("FireMissile");
            timeSinceFire = 0.0f;
        }
    }

    IEnumerator FireMissile() 
    {
        Vector3 firingPosition = new Vector3();
        float distance = 0;

        // Find a suitable spot to fire from.
        while(distance <= minFiringDistance) {
            firingPosition = new Vector3(Random.Range(-maxFiringDistance, maxFiringDistance + 1), 2f, Random.Range(-maxFiringDistance, maxFiringDistance));
            distance = Mathf.Sqrt(Mathf.Pow(0 - firingPosition.x, 2) + Mathf.Pow(0 - firingPosition.z, 2));
        }

        // Find a target position.
        Vector3 targetPosition = new Vector3(Random.Range(-5, 5), 2f, Random.Range(-5, 5));
        
        GameObject missile = Instantiate(missilePrefab, firingPosition, Quaternion.identity);
        missile.transform.LookAt(targetPosition);
        missile.transform.Rotate(new Vector3(-45, 0, 0)); // Lean upwards in forward direction (z)

        // Launch missile toward target.
        missile.GetComponent<Rigidbody>().AddForce(missile.transform.forward * distance * launchForceMultiplier);

        yield return null;
    }
}
