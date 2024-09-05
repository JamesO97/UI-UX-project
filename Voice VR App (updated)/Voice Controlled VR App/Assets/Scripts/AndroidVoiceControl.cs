using UnityEngine;
using UnityEngine.Android;

/*
 * Class that is used by VoiceControlManager.cs script to handle all Android voice recognition.
 * This class interfaces with the the androidvoicerecognition plugin to receive user voice input
 * and perform voice recognition using Android's native Voice Recognition alongside Snowboy's
 * Hotword detection library to create a "virtual assistant"-like continuous voice control experience
 *
 * For more information
 * Snowboy: https://github.com/Kitt-AI/snowboy
 * SpeechRecognizer: https://developer.android.com/reference/android/speech/SpeechRecognizer
 */
public class AndroidVoiceControl
{
#if UNITY_ANDROID
    private AndroidJavaObject androidVoiceRecognizer;
    private bool isRecognitionActive = false;

    public AndroidVoiceControl()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            initVoiceRecognizer();  // implicitly starts recording
        }
        else
        {
            // Creates a non-blocking (i.e. game keeps running) prompt for the permission
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    /// <summary>
    /// Instantiates the AndroidVoiceRecognizer Java class and starts recording for user input.
    /// </summary>
    private void initVoiceRecognizer()
    {
        isRecognitionActive = true;
        androidVoiceRecognizer = new AndroidJavaObject("com.fiu.androidvoicerecognition.AndroidVoiceRecognizer");
        androidVoiceRecognizer.Call("startRecording");
    }

    /*
     * Handles various behaviors when the focus of the application changes
     */
    public void OnApplicationFocus(bool inFocus)
    {
        if (androidVoiceRecognizer == null && inFocus)
        {
            // Because the first Android permission request causes the app to lose focus, 
            // we leverage this fact to re-check permissions once more on refocus and create
            // the recognizer in case permission was given during this request
            if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                initVoiceRecognizer();  // implicitly starts recording
            }
            else
            {
                Debug.LogWarning("Insufficient permissions to initialize androidVoiceRecognizer");
            }
        } else if (isRecognitionActive) // && androidVoiceRecognizer != null
        {
            // Prevents recognition in out-of-focus contexts (e.g. app in background)
            if (inFocus)
            {
                androidVoiceRecognizer.Call("startRecording");
            }
            else
            {
                androidVoiceRecognizer.Call("stopRecording");
            }
        } 
    }

    /// <summary>
    /// Function used to enable or disable voice recognition. 
    /// Accepts a boolean to either start or stop the keyword recognizer
    /// </summary>
    /// <param name="enabled">True to start, False to stop</param>
    public void ToggleRecognition(bool enabled)
    {
        if (androidVoiceRecognizer == null)
        {
            Debug.LogError("androidVoiceRecognizer not initialized, toggle failed");
        }
        else
        {
            isRecognitionActive = enabled;

            if (enabled)
            {
                androidVoiceRecognizer.Call("startRecording");
            }
            else
            {
                androidVoiceRecognizer.Call("stopRecording");
            }
        }
    }
#endif
}