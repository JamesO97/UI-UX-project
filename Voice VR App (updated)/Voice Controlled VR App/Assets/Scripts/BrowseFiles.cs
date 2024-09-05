using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using System.Runtime.InteropServices;

public class BrowseFiles : MonoBehaviour
{
    // For iOS device use
	[DllImport("__Internal")]
	private static extern void OpenVideoPicker(string game_object_name, string function_name);

    //TESTING SYSTEM.IO.FILE + SYSTEM.IO.DIRECTORY LIBRARIES
    //Some of these strings are unused pieces leftover from the development process - TO BE DELETED
    public string currentPath = string.Empty;
    public string previousPath = string.Empty;
    public string path = "\\";
    public string path2 = string.Empty;
    public string[] logicalDrives;
    public ArrayList pathsUsed = new ArrayList();
    public Stack pathHistory = new Stack();

    public int minButtonHeight = 20;    //Changes the size of entries in the file browser

    GameObject parent;
    GameObject browserPanel;
    GameObject scrollView;
    GameObject button;
    RectTransform contentBox;

    ArrayList directories = new ArrayList();
    ArrayList files = new ArrayList();
    ArrayList contentBoxItems = new ArrayList();

    //by alfonso
    //[DllImport("__Internal")]
    //private static extern void printToAlert(string str);

    //The file browser panel starts off as disabled, but its parent UI canvas is enabled
    //So we can get to the browser panel by starting with a reference of its parent object
    private void FindBrowserPanel()
    {
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
                if (t.name == "BrowserPanel")
                {
                    browserPanel = t.gameObject;
                }
            }
        }
    }
    
    // Called when the file browser button is clicked
    // TODO: change from "OpenBrowserPanel" to "ToggleBrowserPanel"
    // if the browser panel is already open, clicking the same button should close it
    public void OpenBrowserPanel()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // Create an AndroidFileBrowser object using the respective plugin
            AndroidJavaObject afb  = new AndroidJavaObject("com.fiu.androidfilebrowser.AndroidFileBrowser");

            /* openBrowser starts the built-in Android file explorer. After selecting
            a file, the plugin passes the filepath to LoadVideoURL.SetFilePath() */
            afb.Call("openBrowser");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            button = GameObject.Find("BrowseFiles");
            OpenVideoPicker(button.name, "SetVideoClip");
        }

        else if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            pathHistory.Clear();
            previousPath = string.Empty;

            if (browserPanel == null)
            {
                FindBrowserPanel();
            }

            if (browserPanel != null)
            {
                //We dont want to move the camera around while navigating the file browser
                CameraController cameraController = FindObjectOfType<CameraController>();

                if (cameraController == null)
                {
                    Debug.Log("CameraController was not found in the scene");
                }
                else
                {
                    cameraController.enabled = false;
                }

                //Make the UI object visible
                browserPanel.SetActive(true);

                //We need references to the scrollview & content box
                //so that we can add entries to them procedurally 
                scrollView = GameObject.Find("FilesScrollView");
                contentBox = FindContent(scrollView);

                ShowLogicalDrives();
            }
            else
            {
                Debug.LogError("Could not find FileBrowserPanel in the scene");
            }
        }
    }
    

    public void ShowLogicalDrives()
    {
        currentPath = string.Empty;

        logicalDrives = Directory.GetLogicalDrives(); //Fill array with logical drives

        if (logicalDrives == null)
        {
            Debug.Log("Error: No logical drives were accessible");
        }
        else 
        {
            //Without clearing entries, you can spam-click the open file browser button and fill it
            //with repeated entries of the logical drives 
            ClearBrowserEntries();

            foreach (string drive in logicalDrives)
            {
                Debug.Log(drive);
                Debug.Log(currentPath);
                CreateBrowserEntry( drive );
            }
        }
    }

    //Adds a clickable button to the "Content" child object of the file browser window
    //
    //The "Content" has components "Vertical Layout Group" and "Content Size Fitter" which 
    //determine how the buttons are placed together evenly in the box.
    //
    //I havent 100% figured out how those components affect the layout but I just
    //changed their settings until it looked right 
    private void CreateBrowserEntry(string path)
    {
        //Create a browser entry as a button - this use of resources was found on stack exchange
        DefaultControls.Resources TempResource = new DefaultControls.Resources();
        GameObject NewEntry = DefaultControls.CreateButton(TempResource);
        NewEntry.name = path.Remove(0, currentPath.Length);

        //Set Entry Layout
        NewEntry.AddComponent<LayoutElement>();
        LayoutElement layout = NewEntry.GetComponent<LayoutElement>();
        layout.minHeight = 38; // Set height to 40 to allow for larger text sizes

        //Set Entry length and width
        RectTransform rect = NewEntry.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, minButtonHeight);

        //Set Entry button's colors using a new ColorBlock object 
        Button button = NewEntry.GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        button.colors = cb;

        //Set the text for the new entry
        Text t = NewEntry.GetComponentInChildren<Text>();
        t.alignment = TextAnchor.MiddleLeft;
        t.fontSize = 26; // Larger font size to improve readability on mobile devices
        t.text = NewEntry.name;

        //Add Listener to onClick
        button.onClick.AddListener(delegate { EntryClick(path); });

        //Place the entry into the content area of the browser window
        NewEntry.transform.SetParent(contentBox);
        contentBoxItems.Add(NewEntry);  //Keep reference to created entry so that it can be deleted later when we are done with it
    }

    //When an entry in the file browser is clicked, either its a directory
    //and we move further into the file tree, or its an MP4 and we play it
    void EntryClick(string newPath)
    {
        if (newPath.Contains(".mp4"))   //the clicked filepath represents an mp4 file
        {
            Debug.Log("Video file selected");
            SetVideoClip(newPath);
            CloseBrowserPanel();
        }
        else
        {
            ShowNewPathContents(newPath);   //the clicked filepath represents a directory
        }

    }

    //This method requires the LoadVideoURL script to be present in the scene
    //LoadVideoURL has member filePath which we set with this method
    public void SetVideoClip(string newPath)
    {
        LoadVideoURL loadVideoURL_script = FindObjectOfType<LoadVideoURL>();

        if (loadVideoURL_script == null)
        {
            Debug.Log("Script name: LoadVideoURL was not found in the scene - unable to set video clip");
        }
        else
        {
            loadVideoURL_script.filePath = newPath;
            loadVideoURL_script.LoadVideoFile();
        }
    }


    //This method is called when an entry clicked in the file browser represents a directory
    public void ShowNewPathContents(string newCurrentPath)
    {
        if (currentPath != string.Empty && newCurrentPath.Length > currentPath.Length )
        {
            pathHistory.Push(currentPath);
        }

        previousPath = currentPath; //Will eventually use this to implement a back button
        Debug.Log("PreviousPath = " + previousPath);

        currentPath = newCurrentPath;
        Debug.Log("Current path is now: " + currentPath);

        string[] items = Directory.GetDirectories(currentPath);

        foreach (string s in items)
        {
            directories.Add(s); //Fill arraylist with strings that represent directories
        }

        items = Directory.GetFiles(currentPath);

        foreach (string s in items)
        {
            if (s.Contains(".mp4")) //Filter out any file that is not an mp4
            {
                files.Add(s);   //Fill arraylist with strings that represent (mp4)files
            }
        }

        ClearBrowserEntries();  //Clear the current entries shown in the browser window

        ShowDirectories(); //Fill window with new entries
        ShowFiles();
    }

    public void ShowPreviousDirectory()
    {
        // if stack is empty the app will crash so I placed it in a try block
        // when this executes on an empty stack it will call the catch block and this method will return
        try
        {
            pathHistory.Peek(); 
        }
        catch
        {
            ShowLogicalDrives();
            return;
        }

        if (pathHistory.Peek().ToString() != string.Empty)
        {
            Debug.Log("PathHistorry.Peek(): " + pathHistory.Peek().ToString());
            ShowNewPathContents(pathHistory.Pop().ToString());
        }
    }

    private void ClearBrowserEntries()
    {
        foreach (GameObject obj in contentBoxItems)
        {
            Destroy(obj);
        }
    }

    public void ShowDirectories()
    {
        foreach (string path in directories)
        {
            CreateBrowserEntry(path);
        }

        directories.Clear();    //we are done with this set of directories, empty the array list
    }

    public void ShowFiles()
    {
        foreach (string file in files)
        {
            CreateBrowserEntry(file);
        }

        files.Clear();
    }

    //Helper Method to find the "Content" component of a scroll view
    public RectTransform FindContent(GameObject ScrollViewObject)
    {
        RectTransform RetVal = null;
        Transform[] Temp = ScrollViewObject.GetComponentsInChildren<Transform>();

        foreach (Transform Child in Temp)
        {
            if (Child.name == "Content") { RetVal = Child.gameObject.GetComponent<RectTransform>(); }
        }

        return RetVal;
    }

    public void CloseBrowserPanel()
    {
        if (browserPanel == null)
            return;

        ClearBrowserEntries(); //Otherwise previous entries will remain in the browser when you open it again

        //re-enable the camera controller that was disabled when the file browser was opened
        CameraController cameraController = FindObjectOfType<CameraController>(); 

        if (cameraController == null)
        {
            Debug.Log("CameraController was not found in the scene");
        }
        else
        {
            cameraController.enabled = true;
        }

        pathHistory.Clear();
        previousPath = string.Empty;
        currentPath = string.Empty;

        browserPanel.SetActive(false);
    }

    /*
    private void InitializeBrowserPanel()
    { 
    
    }
    */

    public void OpenVideoPickerIOS() {

        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            button = GameObject.Find("Button");
            OpenVideoPicker(button.name, "VideoPicked");
        }  
        else
        {
            Debug.LogWarning("Wrong platform!");
        }
            
    }

    void VideoPicked( string path ){
		//Debug.Log( path );
        SetVideoClip(path);
	}

}
