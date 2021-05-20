using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

//Lets you reset the planet  you are on if something is acting up. Most likely some collision issues with platforms
public class RestartPlanetButton : MonoBehaviour
{
    [SerializeField]
    XRNode inputSource;
    [SerializeField]
     Puzzle currentPuzzle;

    [SerializeField]
    private bool isButtonPressed;
    [SerializeField]
    private bool isButtonHeld = false;
    InputDevice device;

    void Update()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out isButtonPressed);

        if (isButtonPressed)
        {
            if (isButtonHeld)
            {
                //ButtonHeldEvent.Invoke();
            }
            else
            {
                isButtonHeld = true;
                currentPuzzle.ResetPlanet();
                //ButtonDownEvent.Invoke();
            }

        }

        else
        {
            //ButtonUpEvent.Invoke();
            isButtonHeld = false;
        }
    }

    public void SetCurrentPuzle(Puzzle p)
    {
        currentPuzzle = p;
    }
}
