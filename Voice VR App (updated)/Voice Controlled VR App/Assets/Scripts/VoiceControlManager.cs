using UnityEngine;
using UnityEngine.UI;
using IOSSpeechToText;
using System.Collections;

/*
 * Script tied to the VoiceControl GameObject that serves as the central point
 * for all Voice Control
 * 
 * NOTE: If you want to create voice control for a new platform, it is strongly
 * recommended you create a class called "PLATFORMVoiceControl.cs" that can be
 * initialized here, and then handle all platform specific logic in that class.
 * The use of preprocessors is also encouraged to avoid compile-time errors and
 * needless compilation of unused libraries.
 */
public class VoiceControlManager : MonoBehaviour
{
    private InterpretCommand interpreter;
    private bool isRecognitionActive = true;
    private GameObject micActivityIcon;
    private IEnumerator fadeCoroutine;

    private Image voiceToggleButtonImg;     /*---TESTING---*/

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private WindowsVoiceControl windowsVoiceControl;
    #endif

    #if UNITY_ANDROID
    private AndroidVoiceControl androidVoiceControl;
    #endif

    #if UNITY_WEBGL
    private WebGLVoiceControl webglVoiceControl;
    #endif

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    /// Initiates voice controller objects for each build and an InterpretCommand object
    /// used to pass the commands to the video player
    /// It also initiates a gameobject to use for debugging purposes
    private void Start()
    {
        interpreter = new InterpretCommand();

        /*
         * Gets the visual indicator that shows if voice recognition is active
         * Note: We assume all implementations start with voice recognition "on"
         * It also assigns and starts the "fade" coroutine to make the icon fade 
         * in/out every 1 second
         */
        micActivityIcon = GameObject.Find("UICanvas").transform.Find("MicrophoneActivityContainer").gameObject;
        fadeCoroutine = FadeIcon();
        StartCoroutine(fadeCoroutine);

        /*---TESTING---*/
        voiceToggleButtonImg = GameObject.Find("UICanvas").transform.Find("MenuStripContainer")
            .transform.Find("VoiceRecognitionServiceButton").gameObject.GetComponent<Image>();
        voiceToggleButtonImg.color = isRecognitionActive ? Color.cyan : Color.white;
        /*---TESTING---*/

        //DEBUG: enables button on scene for tesing purposes
        //Show "DevTestButton" in scene
        //GameObject.Find("UICanvas").transform.Find("DevTestButton").gameObject.SetActive(true);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        windowsVoiceControl = new WindowsVoiceControl(interpreter);
        #endif

        #if UNITY_ANDROID
        androidVoiceControl = new AndroidVoiceControl();
        #endif

        #if UNITY_WEBGL
        webglVoiceControl = new WebGLVoiceControl();
        webglVoiceControl.Listen();
        #endif

        #if UNITY_IOS
        IOSVoiceControl.Instance.Setting("en-US");
        IOSVoiceControl.Instance.onResultCallback = Result;
        IOSVoiceControl.Instance.StartRecording();
        #endif


    }

    /// <summary>
    /// Receives any message result from a platform specific voice recognition solution.
    /// Because some solutions, like Android, must interface with a GameObject to return
    /// messages to Unity, this method serves as a middleman which then passes the message
    /// onto an "interpreter" that performs various actions depending on the message
    /// </summary>
    /// <param name="message"></param>
    public void Result(string message)
    {
        interpreter.Parse(message);
    }

    /// <summary>
    /// This is used to toggle voice recognition on the different platforms.
    /// It is called by the button VoiceRecognitionServiceButton found on the MenuStripContainer.
    /// The variable isRecognitionActive is changed everytime the button is pressed.
    /// Also, the microphone icon is activated/deactivated on the scene to provide feedback for the user.
    /// </summary>
    public void ToggleRecognition()
    {
        isRecognitionActive = !isRecognitionActive;
        //micActivityIcon.SetActive(isRecognitionActive);   /*---TESTING---*/
        if (isRecognitionActive)
        {
            /*---TESTING---*/
            //StartCoroutine(fadeCoroutine);
            voiceToggleButtonImg.color = Color.cyan;
            /*---TESTING---*/
        }
        else
        {
            /*---TESTING---*/
            //StopCoroutine(fadeCoroutine);
            voiceToggleButtonImg.color = Color.white;
            /*---TESTING---*/
        }

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        windowsVoiceControl.ToggleRecognition(isRecognitionActive);
        #endif

        #if UNITY_ANDROID
        androidVoiceControl.ToggleRecognition(isRecognitionActive);
        #endif

        #if UNITY_WEBGL
        webglVoiceControl.ToggleRecognition(isRecognitionActive);
        #endif

        #if UNITY_IOS
        IOSVoiceControl.Instance.ToggleRecognition(isRecognitionActive);
        #endif

    }

    /// <summary>
    /// Corutine used to make the icon fade. Its called at the "start" function and 
    /// when the "toggle voice recognition" button is pressed
    /// FIXME: get rid of magic numbers for time
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIcon()
    {
        while (isRecognitionActive)
        {

            micActivityIcon.transform.Find("MicrophoneActiveIcon").GetComponent<Image>().CrossFadeAlpha(0, 0.5f, false);
            yield return new WaitForSeconds(1.0f);
            micActivityIcon.transform.Find("MicrophoneActiveIcon").GetComponent<Image>().CrossFadeAlpha(1, 0.5f, false);
            yield return new WaitForSeconds(1.0f);
        }

    }

    /// <summary>
    /// OnApplicationFocus is called when the application loses or gains focus
    /// </summary>
    /// https://docs.unity3d.com/2018.4/Documentation/ScriptReference/MonoBehaviour.OnApplicationFocus.html
    private void OnApplicationFocus(bool focus)
    {
        #if UNITY_ANDROID
        if (androidVoiceControl != null) // avoids error on startup
        {
            androidVoiceControl.OnApplicationFocus(focus);
        }
        #endif
    }
}
