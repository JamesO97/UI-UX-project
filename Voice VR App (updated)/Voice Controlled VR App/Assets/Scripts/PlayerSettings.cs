using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PlayerSettings : MonoBehaviour
{
    string videoType;
    string vrType;
    Color buttonSelected = new Color(90.0f, 255.0f, 234.0f, 255.0f);
    Color buttonDefault = new Color(255.0f, 255.0f, 255.0f, 255.0f);
    GameObject settingsPanel;

    public void ToggleSettingsPanel()
    {
        if (settingsPanel == null)
        {
            FindSettingsPanel();
        }

        if (settingsPanel == null)
        {
            Debug.LogError("Could not find SettingsPanel in scene");
        }
        else
        {
            if (settingsPanel.activeSelf == true)
            {
                settingsPanel.SetActive(false);
            }
            else
            {
                settingsPanel.SetActive(true);
                InitializeButtons();
            }
        }
    }

    private void FindSettingsPanel()
    {
        // Settings panel starts off inactive and cannot be found using .Find()
        // So we must get its parent object and find the settingsPanel in its child components
        GameObject uiCanvas = GameObject.Find("UICanvas");

        if (uiCanvas == null)
        {
            Debug.LogError("object not found in scene");
        }
        else
        {
            Transform[] transforms = uiCanvas.GetComponentsInChildren<Transform>(true); //true allows you to find inactive objects

            foreach (Transform t in transforms)
            {
                if (t.name == "SettingsPanel")
                {
                    settingsPanel = t.gameObject;
                }
            }
        }
    }

    //This was an attempt to change the color of any settings button that matches the current video type
    //For example if the video player was set to 180 Side by Side, 
    //then the 180 button and the Side by Side button would have a diffrent color
    //
    //This doesn't work due to a known bug
    //If you look at the color in the inspector its still white, but if you click on it, it shows that it should be a different color
    private void InitializeButtons()
    {
        //Debug.Log("Initializing SettingsPanel buttons");

        LoadVideoURL loadVideoURL_script = FindObjectOfType<LoadVideoURL>();

        if (loadVideoURL_script == null)
        {
            Debug.LogError("Script: LoadVideoURL was not found in scene - Settings Panel unable to initialize buttons");
            return;
        }

        videoType = loadVideoURL_script.currentVideoType;
        vrType = loadVideoURL_script.currentVrType;

        //Debug.Log("Current Video Type: " + videoType);
        //Debug.Log("Current VR Type: " + vrType);

        GameObject[] playerSettingsButtons = GameObject.FindGameObjectsWithTag("PlayerSettingsButton");

        foreach (GameObject gameObject in playerSettingsButtons)
        {
            //Debug.Log(gameObject.name);
            Button button = gameObject.GetComponent<Button>();
            ColorBlock cb = button.colors;

            if (gameObject.name == videoType || gameObject.name == vrType)
            {
                //Debug.Log(gameObject.name + " matches current video player settings!");

                cb.normalColor = buttonSelected;
                button.colors = cb;
            }
            else
            {
                cb.normalColor = buttonDefault;
                button.colors = cb;
            }
        }
    }
}
