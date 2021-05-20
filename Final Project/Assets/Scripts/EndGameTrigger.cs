using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Loads End Game Scene when you reach the final teleporter
public class EndGameTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    void OnTriggerEnter(Collider col)
    {
        print("end");
        if(col.gameObject.tag.Equals("Player"))
            SceneManager.LoadScene(2);
    }

}
