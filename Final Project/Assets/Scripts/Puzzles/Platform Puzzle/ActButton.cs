using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The button used to activate platforms on the first planet
public class ActButton : MonoBehaviour
{
    bool isPressed = false;
    [SerializeField]
    Puzzle puz;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Renderer>().material.color = Color.blue;
    }

    public void ResetButton()
    {
        isPressed = false;
        this.GetComponent<Renderer>().material.color = Color.blue;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            if (!isPressed)
            {
                isPressed = true;
                this.GetComponent<Renderer>().material.color = Color.green;
                puz.StartPuzzle();
            }
        }
   
           

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
