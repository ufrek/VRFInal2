using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stepping onto a green teleport pad will send you to the next planet

public class Teleporter : MonoBehaviour
{
    public int callOrder;               //set in inspector
    public bool isActive = false;
    [SerializeField]
    Renderer teleportPad;
    TeleportPosition teleSpot;          //put on an empty game object to get exact teleport position desired
    GameObject planet;                  //the planet that this teleporter is on
    GameObject player;

    [SerializeField]
    TeleportManager teleMgr;

    [SerializeField]
    Transform startingPosition;     //place to teleport to
    [SerializeField]
    Puzzle currentPuzzle;           //reference holder for resetting puzzles


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        planet = transform.parent.gameObject;
         teleSpot = GetComponentInChildren<TeleportPosition>();
        teleportPad = this.GetComponent<Renderer>();
       

        ActivateTeleporter();               //just as a default way to start the teleporter
        
    }



    public void ActivateTeleporter()
    {
        isActive = true;
        teleportPad.material.color = Color.green;
    }

    public void DeactivateTeleporter()
    {
        isActive = false;
        teleportPad.material.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("teleporting");
            if (isActive)
            {
                Teleport();
            }
        }
    }

    public GameObject getPlanet()
    {
        return planet;
    }

    void Teleport()
    {
        Teleporter[] path = teleMgr.getPath();
        Transform targetPos = path[1].startingPosition;
        //reset gravity and move player to other planet
        GravMgr.currentPlanet.GetComponentInChildren<Gravity2>().isInGravityPull = false;
        GravMgr.currentPlanet = path[1].getPlanet();
        GravMgr.currentPlanet.GetComponentInChildren<Gravity2>().isInGravityPull = true;
        player.transform.position = targetPos.position;
        player.transform.rotation = targetPos.rotation;
        player.GetComponent<RestartPlanetButton>().SetCurrentPuzle(path[1].currentPuzzle);

        //reset orientation
        Vector3 gravityUp = (player.transform.position - path[1].transform.position).normalized;
        player.transform.Rotate(gravityUp);

        //rebuilds the path
        callOrder = 99; //puts this planet as last in the rebuilt path
        teleMgr.Invoke("UpdatePath", 1f);
        

    }



}
