using System.Collections;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;

public class VRToggle : MonoBehaviour
{
    // Input.GetKeyDown(KeyCode.Escape) catches clicks on the "x" button in the google cardboard sdk:
    // https://gamedev.stackexchange.com/questions/153821/how-can-i-make-the-x-button-in-the-cardboard-xdk-exit-vr-mode
    void Update()
    {
        if (XRSettings.loadedDeviceName == "cardboard" && ( /*Input.GetMouseButtonDown(0) ||*/ Input.GetKeyDown(KeyCode.Escape) ) )
        {
            ToggleVR();
        }
    }

    public void ToggleVR()
    {

        if (XRSettings.loadedDeviceName == "cardboard")
        {
            StartCoroutine(LoadDevice("None"));
        }
        else
        {
            StartCoroutine(LoadDevice("cardboard"));
        }
    }

    IEnumerator LoadDevice(string newDevice)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = true;
    }
}