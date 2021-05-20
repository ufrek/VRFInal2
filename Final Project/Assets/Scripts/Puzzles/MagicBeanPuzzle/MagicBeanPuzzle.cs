using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBeanPuzzle : Puzzle
{
    [SerializeField]
    Transform startPos;
    [SerializeField]
    MagicBoxGrab[] seeds; //set in inspector for specific order of tower steps
    Vector3[] seedStartPoints;
    Quaternion[] seedStartRotations;
    [SerializeField]
    MagicPotTrigger[] snapZones; //set in inspector

    BallRespawner[] balls;

    [SerializeField]
    Teleporter tele;
   

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        tele.ActivateTeleporter();

        seedStartPoints = new Vector3[seeds.Length];
        seedStartRotations = new Quaternion[seeds.Length];
        for (int i = 0; i < seeds.Length; i++)
        {
            seedStartRotations[i] = seeds[i].transform.rotation;
            seedStartPoints[i] = seeds[i].transform.position;
        }

        startPos = this.GetComponentInChildren<StartPoint>().transform;
    }
    public override void StartPuzzle()
    {
        //does nothing here

    }

    public override void ResetPuzzle()
    {
        foreach (MagicPotTrigger bs in snapZones)
        {
            bs.UnsnapObject();
        }
        for (int i = 0; i < seeds.Length; i++)
        {

            seeds[i].OnRelease();
            seeds[i].transform.position = seedStartPoints[i];
            seeds[i].transform.rotation = seedStartRotations[i];
        }
    }

    public override void ResetPlanet()
    {
        ResetPuzzle();
        player.transform.position = startPos.position;
        player.transform.rotation = startPos.rotation;
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
