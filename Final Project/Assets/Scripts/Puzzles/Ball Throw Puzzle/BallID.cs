using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Identification for ball being thrown
public class BallID : MonoBehaviour
{
    [SerializeField]
    int ballID;     //set in inspector

    public int getBallID()
    {
        return ballID;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
