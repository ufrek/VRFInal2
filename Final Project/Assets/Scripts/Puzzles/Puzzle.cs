using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base for all puzzle activation
public abstract class Puzzle : MonoBehaviour
{

    // Start is called before the first frame update
    public abstract void StartPuzzle();

    public abstract void ResetPuzzle();

    public abstract void ResetPlanet();

    public abstract Transform getStartingPosition();
}
