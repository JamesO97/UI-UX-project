using System.Collections;
using UnityEngine;

public class ToggleMenuStrip : MonoBehaviour
{
    private GameObject menuStripContainer, settingsPanel;
    private BrowseFiles browseFiles_script;
    private Coroutine timeoutRoutine;

    // Start is called before the first frame update
    void Start()
    {
        // Get the BrowseFiles script (not a child of UICanvas)
        browseFiles_script = GameObject.Find("BrowseFiles").GetComponentInChildren<BrowseFiles>();

        /* Traverse through all of UICanvas's immediate children (actually through their transform components)
        We use switch case and foreach so that we only iterate over the hierarchy once for UICanvas' children,
        instead of calling GameObject.Find on each and forcing multiple traversals */
        Transform[] transforms = GameObject.Find("UICanvas").GetComponentsInChildren<Transform>(true); // true allows you to find inactive objects

        foreach (Transform t in transforms)
        {
            switch (t.name)
            {
                case "SettingsPanel":   // used to set VR options
                    settingsPanel = t.gameObject;
                    break;
                case "MenuStripContainer":  // used for containing "Hamburger menu" icons
                    menuStripContainer = t.gameObject;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Toggles on or off the MenuStripContainer
    /// </summary>
    public void Toggle()
    {
        if (menuStripContainer.activeSelf == false)
        {
            menuStripContainer.SetActive(true);
            timeoutRoutine = StartCoroutine(HideMenuStripOnTimeout(5));

        }
        else
        {
            HideMenuStrip();
            HideChildUI();
        }
    }

    public void HideMenuStrip()
    {
        // stops timeout to avoid a previous coroutine from unexpectedly closing the strip later
        StopTimeout();
        menuStripContainer.SetActive(false);
    }

    private IEnumerator HideMenuStripOnTimeout(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        menuStripContainer.SetActive(false);
    }

    /// <summary>
    /// Disables timeout. This can be used on Menu elements when clicked, as that implies
    /// the menu is currently being used and should not time out.
    /// </summary>
    public void StopTimeout()
    {
        StopCoroutine(timeoutRoutine);
    }

    /// <summary>
    /// Closes various UI panels that are opened as extensions/children of the "Hamburger menu"
    /// </summary>
    private void HideChildUI()
    {
        if (settingsPanel != null)
        {
            // Close VR settings panel if active
            settingsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("SettingsPanel not found");
        }

        if (browseFiles_script != null)
        {
            // Close the built-in file explorer window if it is active
            browseFiles_script.CloseBrowserPanel();
        }
        else
        {
            Debug.LogError("BrowseFiles script not found");
        }
    }
}
