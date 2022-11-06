using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Guided, smart projectile */

public class GuidedMissile : MonoBehaviour
{

    public GameObject launcher;
    public GameObject target;
    public bool isGuided; // true = smart, guided missile

    // Adjust these to your needs
    [SerializeField]
    private float radiusEffectiveBlast; // Missile detonates when close enough
    [SerializeField]
    private float thrustForceMultiplier;
    [SerializeField]
    private float projectionTime; // Project this many seconds ahead
    [SerializeField]
    private float coldStartTime; // Time before thrusters activate
    [SerializeField]
    private float rotationP, rotationI, rotationD; // PID gains for rocket rotation

    private new Rigidbody rigidbody;
    private PIDController xRotationController;
    private PIDController yRotationController;
    private PIDController zRotationController;

    bool isCourseSet = false;
    bool isColdStarting = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.maxAngularVelocity = maxAngularVelocity;
        xRotationController = this.gameObject.AddComponent<PIDController>();
        xRotationController.Initialize(rotationP, rotationI, rotationD);
        yRotationController = this.gameObject.AddComponent<PIDController>();
        yRotationController.Initialize(rotationP, rotationI, rotationD);
        zRotationController = this.gameObject.AddComponent<PIDController>();
        zRotationController.Initialize(rotationP, rotationI, rotationD);
    }

    // Update is called once per frame
    void Update() 
    {
    }

    void FixedUpdate() {
        if (target) {

            if (!isCourseSet) {
                // Initial launch
                if (launcher) transform.rotation = launcher.gameObject.GetComponent<AirDefenseLauncher>().launcherModel.transform.rotation;
                rigidbody.AddRelativeForce(Vector3.forward * 40, ForceMode.Impulse);

                isCourseSet = true;
            }
            else if (isCourseSet && isColdStarting) {
                // Cold start phase
                coldStartTime -= Time.deltaTime;
                if (coldStartTime <= 0) isColdStarting = false;
            }
            else if (isCourseSet && isGuided) {
                // Guidance phase
                float deltaT = Time.fixedDeltaTime;
                /* v1 works but very simple 
                Quaternion goalRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, goalRotation, turnSpeed * deltaT);

                rigidbody.AddRelativeForce(transform.forward * thrustForce * Time.deltaTime);
                /* end v1 */

                /* v2 using PID rotation */
                // Rotate towards target
                Quaternion goalRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                Vector3 currentRotation = transform.rotation.eulerAngles;
                float xRotationError = Mathf.DeltaAngle(currentRotation.x, goalRotation.eulerAngles.x);
                float xTorqueAdjust = xRotationController.GetOutput(xRotationError, deltaT);
                float yRotationError = Mathf.DeltaAngle(currentRotation.y, goalRotation.eulerAngles.y);
                float yTorqueAdjust = yRotationController.GetOutput(yRotationError, deltaT);
                float zRotationError = Mathf.DeltaAngle(currentRotation.z, goalRotation.eulerAngles.z);
                float zTorqueAdjust = zRotationController.GetOutput(zRotationError, deltaT);
                rigidbody.AddRelativeTorque(new Vector3(xTorqueAdjust, yTorqueAdjust, zTorqueAdjust));


                float distanceToTarget = Mathf.Sqrt(
                    Mathf.Pow(transform.position.x - target.transform.position.x, 2) +
                    Mathf.Pow(transform.position.y - target.transform.position.y, 2) +
                    Mathf.Pow(transform.position.z - target.transform.position.z, 2));

                if (distanceToTarget > 5) {
                    Vector3 pos = target.transform.position;
                    Vector3 velocity = target.GetComponent<Rigidbody>().velocity;

                    Vector3 projectedpos = new Vector3(
                        pos.x + velocity.x * projectionTime,
                        pos.y + velocity.y * projectionTime,
                        pos.z + velocity.z * projectionTime);

                    rigidbody.AddForce((projectedpos - transform.position).normalized * thrustForceMultiplier, ForceMode.VelocityChange);
                } else {
                    rigidbody.AddForce((target.transform.position - transform.position).normalized * thrustForceMultiplier, ForceMode.VelocityChange);
                }
                /* end v2 */


            }
            // Detonate when close enough
            if (Vector3.Distance(transform.position, target.transform.position) < radiusEffectiveBlast) {
                Detonate();
                target.gameObject.SendMessage("Detonate");
            }

        }
        else {
            // Unknown phase
            Destroy(gameObject);
        }

        // Handle out of bounds
        if (transform.position.y < -5) { 
            Destroy(gameObject);
        }
    }
    


    private void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("IncomingMissile")) {
            return;
        }
        Detonate();
    }

    private void Detonate() {
        if (launcher) launcher.GetComponent<AirDefenseLauncher>().interceptedMissiles++; // Notify counter
        
        // TODO: Explosion!
        Destroy(gameObject);

        // Allow effects to persist for a few seconds
        Transform t = gameObject.transform.GetChild(0);
        Destroy(t.gameObject);
        t = gameObject.transform.GetChild(1);
        Destroy(t.gameObject, 5);
        t = gameObject.transform.GetChild(2);
        t.GetComponent<ParticleSystem>().Stop();
        Destroy(t.gameObject, 5);
        gameObject.transform.DetachChildren();
    }
}
