using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//gathers references to all the teleporters and lets you teleport to planets in a set order
public class TeleportManager : MonoBehaviour
{
    static Teleporter[] path;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePath();
    }

    void UpdatePath()
    {
        path = this.GetComponentsInChildren<Teleporter>();
        /*foreach (WayPoint w in path)
        {
            w.GetComponent<MeshRenderer>().enabled = false;
        }*/
        Array.Sort(path, SortByOrder);

    }

    public int SortByOrder(Teleporter a, Teleporter b)
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
                int retval = a.callOrder.CompareTo(b.callOrder);

                return retval;
            }
        }
    }

    public Teleporter[] getPath()
    {

        UpdatePath();
        return path;

        //for vector 3 output
        /* UpdatePath();
         Vector3[] positions = new Vector3[path.Length];
         for(int i = 0; i < path.Length; i++)
         {
             positions[i] = path[i].gameObject.transform.position;
         }

         return positions;
        */
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
