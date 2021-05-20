using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles resetting of the throw ball puzzle
public class ThrowPuzzle : Puzzle
{
    [SerializeField]
    Transform startPos;
    [SerializeField]
    GameObject[] platforms; //set in inspector for specific order of tower steps
    [SerializeField]
    GameObject[] targets; //set in inspector

    BallRespawner[] balls;     

    [SerializeField]
    Teleporter tele;
 

    GameObject player;


    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (GameObject p in platforms) //makes them all disapper at start so can keep on during debugging
        {
            p.GetComponent<BoxCollider>().enabled = false;
            p.GetComponent<MeshRenderer>().enabled = false;
        }
        balls = this.GetComponentsInChildren<BallRespawner>();

        tele.ActivateTeleporter();  //just make teleporter reachable

        startPos = this.GetComponentInChildren<StartPoint>().transform;
    }

    public override void StartPuzzle()
    {
        //don't think this needs to do anything
        
    }

    public void ActivatePlatform(int index)
    {
        print("activate");
        platforms[index].GetComponent<BoxCollider>().enabled = true;
        platforms[index].GetComponent<MeshRenderer>().enabled = true;
    }

    public override void ResetPlanet()
    {
      
        player.transform.position = startPos.position;
        player.transform.rotation = startPos.rotation;
        ResetPuzzle();

    }

    public override void ResetPuzzle()
    {
        foreach (GameObject p in platforms) //makes them all disapper 
        {
            p.GetComponent<BoxCollider>().enabled = false;
            p.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach (BallRespawner b in balls)
        {
            b.InstantRespawn();
        }

        foreach (GameObject t in targets)
        {
            t.GetComponent<Target>().ResetMaterial();
        }
    }

    public override Transform getStartingPosition()
    {
        return startPos;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
