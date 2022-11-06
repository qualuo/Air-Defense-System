using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController : MonoBehaviour {
    [SerializeField]
    private float kP, kI, kD; // Gain controls; adjust these.

    private float P, I, D;
    private float prevError;

    public void Initialize(float kP, float kI, float kD) {
        this.kP = kP;
        this.kI = kI;
        this.kD = kD;
    }

    public float GetOutput(float currentError, float dt) {
        P = currentError;
        I += P * dt;
        D = (P - prevError) / dt;
        prevError = currentError;

        return P * kP + I * kI + D * kD;
    }
}
