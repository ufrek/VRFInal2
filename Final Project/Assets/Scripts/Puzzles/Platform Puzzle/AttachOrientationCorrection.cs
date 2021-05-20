using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachOrientationCorrection : MonoBehaviour
{
    [SerializeField]
    GameObject currentPlanet;

    //adjusts the rotation of the transform so that it's straight with the planet
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void AdjustOrientation()
    {
        Vector3 orientation = (this.transform.position - currentPlanet.transform.position).normalized;
        this.transform.up = orientation;
    }
}
