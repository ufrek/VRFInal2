using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//allows for resetting of puzzle if bugged out
public class BridgePuzzle : Puzzle
{
    [SerializeField]
    Transform startPos;
    [SerializeField]
    BridgeSnapTrigger[] snapZones;  //all attach zones
    [SerializeField]
    BridgePieceGrab[] objectGrabbables; //all bridge pieces
    Vector3[] grabbableStartPoints;
    Quaternion[] grabbableStartRotations;

    [SerializeField]
    Teleporter tele;

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        tele.ActivateTeleporter();

        grabbableStartPoints = new Vector3[objectGrabbables.Length];
        grabbableStartRotations = new Quaternion[objectGrabbables.Length];
        for (int i = 0; i < objectGrabbables.Length; i++)
        {
            grabbableStartPoints[i] = objectGrabbables[i].transform.position;
            grabbableStartRotations[i] = objectGrabbables[i].transform.rotation;
        }
        startPos = this.GetComponentInChildren<StartPoint>().transform;
       

    }

    public override void StartPuzzle()
    {
        //does nothing here
       
    }

    public override void ResetPuzzle()
    {
        foreach (BridgeSnapTrigger bs in snapZones)
        {
            bs.UnsnapObject();
        }
        for (int i = 0; i < objectGrabbables.Length; i++)
        {
          
            objectGrabbables[i].OnRelease();
            objectGrabbables[i].transform.position = grabbableStartPoints[i];
            objectGrabbables[i].transform.rotation = grabbableStartRotations[i];
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
