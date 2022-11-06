using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDefenseLauncher : MonoBehaviour
{
    public GameObject missilePrefab;
    public GameObject launcherModel;
    [Range(1,2)]
    public int mode = 1;
    public float headTurnSpeed = 30;

    public int detectedMissiles;
    public int interceptedMissiles;

    GameObject currentTarget;

    private Queue<GameObject> targetsToFireAt = new Queue<GameObject>();
    private bool isRotating = false;

    void Start()
    {
    }

    void Update()
    {
        if (mode == 1) {
            if (!currentTarget) isRotating = false;

            if (isRotating) {
                Quaternion goalRotation = Quaternion.LookRotation(currentTarget.transform.position - launcherModel.transform.position);
                launcherModel.transform.rotation = Quaternion.Lerp(launcherModel.transform.rotation, goalRotation, headTurnSpeed * Time.deltaTime);

                if (Quaternion.Angle(goalRotation, launcherModel.transform.rotation) < 5) {
                    // Currently facing target
                    FireMissile();

                    isRotating = false;
                }
            }
            if (!isRotating && targetsToFireAt.Count > 0) {
                currentTarget = targetsToFireAt.Dequeue();

                isRotating = true;
            }
        } else if (mode == 2) { // Does not rotate
            launcherModel.transform.eulerAngles = new Vector3(-90, 0, 0);
            if (targetsToFireAt.Count > 0) {
                currentTarget = targetsToFireAt.Dequeue();
                FireMissile();
            }

        }

    }

    private void FireMissile() {
        if (!currentTarget) return;
        Vector3 pos = currentTarget.gameObject.transform.position;
        Vector3 velocity = currentTarget.gameObject.GetComponent<Rigidbody>().velocity;

        GameObject missile = Instantiate(missilePrefab, new Vector3(
            launcherModel.transform.position.x,
            launcherModel.transform.position.y,
            launcherModel.transform.position.z),
            launcherModel.transform.rotation);
        
        missile.GetComponent<GuidedMissile>().launcher = this.gameObject;
        missile.GetComponent<GuidedMissile>().target = currentTarget.gameObject;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("IncomingRocket")) {
            return;
        }
        detectedMissiles++;

        targetsToFireAt.Enqueue(other.gameObject);
    }
}
