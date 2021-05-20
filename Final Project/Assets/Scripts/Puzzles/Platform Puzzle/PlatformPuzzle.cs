using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//the puzzle has a switch that spawns platforms that disappear after a short amount of time
//the platforms lead to the top of the tower
public class PlatformPuzzle : Puzzle
{
    [SerializeField]
    Transform startPos;

    [SerializeField]
    ActButton button;           //set in inspector
    [SerializeField]
    Teleporter tele;            //set in inspector
    [SerializeField]
    float despawnWaitTime = 4f;

    GameObject player;
    MeshRenderer[] platforms;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        platforms = this.GetComponentsInChildren<MeshRenderer>();
        Array.Sort(platforms, SortByPlatform);
        foreach (MeshRenderer r in platforms)
        {
            print(r.name);
        }

        tele.ActivateTeleporter();

        startPos = this.GetComponentInChildren<StartPoint>().transform;
    }



    public override void StartPuzzle()
    {
        StartCoroutine(SpawnPlatforms());
    }

    IEnumerator SpawnPlatforms()
    {
        for(int i = 0; i < platforms.Length; i++)
        {
            platforms[i].enabled = true;
            platforms[i].gameObject.GetComponent<BoxCollider>().enabled = true;
            if (i == platforms.Length - 1)
                StartCoroutine(DespawnPlatform(platforms[i], true));
            else
            {
                StartCoroutine(DespawnPlatform(platforms[i], false));
                yield return new WaitForSeconds(1);
            }
               
            
            
        }

    }

    IEnumerator DespawnPlatform(MeshRenderer r, bool b)
    {
        yield return new WaitForSeconds(despawnWaitTime);
        r.enabled = false;
        r.gameObject.GetComponent<BoxCollider>().enabled = false;
        if (b)
        {
            yield return new WaitForSeconds(1);
            button.ResetButton();
        }
    }

    public override void ResetPuzzle()
    {
        foreach (MeshRenderer r in platforms)
        {
            r.gameObject.GetComponent<BoxCollider>().enabled = false;
            r.enabled = false;
        }
        button.ResetButton();

    }

    //resets puzzle and resets player position....quick escape for bugs
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

    public int SortByPlatform(MeshRenderer a, MeshRenderer b)
    {
        if (a == null)
        {
            if (b == null)
            {
                // If a null and b null, they're eqyal.
                return 0;
            }
            else
            {
                // If a null and b isn't, b is greater
                return -1;
            }
        }
        else
        {
            // a not null
            if (b == null)
            //b is null, x greater
            {
                return 1;
            }
            else
            {
                //compare lengths
                string aVal = a.gameObject.name;
                aVal.Remove(0, aVal.Length - 2);
                string bVal = b.gameObject.name;
                bVal.Remove(0, bVal.Length - 2);
                int aNum = int.Parse(aVal);
                int bNum = int.Parse(bVal);
                int retval = aNum.CompareTo(bNum);

                return retval;
            }
        }
    }
}
