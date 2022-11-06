using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    [Min(0)]
    public float timeScale;

    // Start is called before the first frame update
    void Start() 
    {
    }

    // Update is called once per frame
    void Update() 
    {
        Time.timeScale = this.timeScale;
    }
}
