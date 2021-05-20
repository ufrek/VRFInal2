using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//target will activate platform if hit with ball of correct ID
public class Target : MonoBehaviour
{
    [SerializeField]
    int targetID;   //set in inspector
    
    [SerializeField]
    ThrowPuzzle puz;
    Color originalColor;

    void Start()
    {
        originalColor = this.GetComponent<Renderer>().material.color;
    }

    void OnTriggerEnter(Collider col)
    {
       
        if (col.gameObject.tag.Equals("Ball"))
        {
            int ballID = col.gameObject.GetComponent<BallID>().getBallID();
            print(ballID);
            if (targetID == ballID)
            {
                print("ID passed");
                puz.ActivatePlatform(targetID);
                this.GetComponent<Renderer>().material.color = Color.green;
            }
        }

    }

    public void ResetMaterial()
    {
        this.GetComponent<Renderer>().material.color = originalColor;
    }


    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
