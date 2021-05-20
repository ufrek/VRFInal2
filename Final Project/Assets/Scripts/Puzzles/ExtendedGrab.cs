using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//adds some functionality for when player grabs and releases an object
public abstract  class ExtendedGrab : MonoBehaviour
{
    // Start is called before the first frame update
    public abstract void OnGrab();

    public abstract void OnRelease();


}
