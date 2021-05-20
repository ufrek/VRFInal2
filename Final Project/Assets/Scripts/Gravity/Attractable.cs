using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attach to make object able to be affected by graviational pull
public class Attractable : MonoBehaviour
{
    bool attractable = true;

    public bool getAttractable()
    {
        return attractable;
    }

    public void setAttractable(bool a)
    {
        attractable = a;
    }
}
