using System.Collections.Generic;
using UnityEngine;

public class HideUI : MonoBehaviour
{
    public List<GameObject> allButton;

    // Start is called before the first frame update
    void Start()
    {
        //if application is build for webGL, hide(some) ui buttons
        #if UNITY_WEBGL
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            HideUIWebGL();
        }
        #endif

    }
    /// <summary>
    /// Hides elements for the webGL Build. Uses preprocessor directives
    /// Uses RectTransform to find the objects since RectTransform also allows for the resizing of said game objects.
    /// Information on how to resize taken from: https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/
    /// </summary>
#if UNITY_WEBGL
    void HideUIWebGL()
    {

        GameObject uiCanvas = GameObject.Find("UICanvas");

        if (uiCanvas == null)
        {
            Debug.LogError("object not found in scene");
        }
        else
        {
            RectTransform[] objects = uiCanvas.GetComponentsInChildren<RectTransform>(true); //true allows you to find inactive objects

            foreach (RectTransform rt in objects)
            {
                switch (rt.name)
                {
                    case "OpenFileBrowserButton":
                    case "VRToggleButtonContainer":
                        rt.gameObject.SetActive(false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
#endif
    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        // https://kylewbanks.com/blog/unity-2d-detecting-gameobject-clicks-using-raycasts
    //        Debug.Log("Screen clicked");
    //        if (isButtonDisplayed)
    //        {
    //            HideAllButtons();
    //        }
    //        else
    //        {
    //            ShowAllButtons();
    //        }
    //        count++;
    //    }
    //    else
    //    {
    //        count = 0;
    //    }
    //}



    //// https://gamedev.stackexchange.com/questions/133520/show-hide-button-in-unity-using-c
    //void ShowAllButtons()
    //{
    //    if (count == 0)
    //    {
    //        for (int index = 0; index < allButton.Count; index++)
    //        {
    //            allButton[index].SetActive(true);
    //        }
    //        isButtonDisplayed = true;
    //    }
    //}  
}
