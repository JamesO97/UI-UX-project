using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;

/*
 * This code was originally taken from the unity project:https://github.com/PingAK9/Speech-And-Text-Unity-iOS-Android
 * and then modified using other sources in order to make it continuous:
 * https://stackoverflow.com/questions/43834119/how-to-implement-speech-to-text-via-speech-framework
 * https://stackoverflow.com/questions/37821826/continuous-speech-recogn-with-sfspeechrecognizer-ios10-beta
 *
 * This class does not get instantiated by Voice controller, but rather is static class that is started automatically
 * This class then calls the methods on the plugin to start recogniton. When the plugins recognizes a word, it calls the 
 * result method on this class, which is associated with the voice control Manager class result method
 */
    

namespace IOSSpeechToText
{
    public class IOSVoiceControl : MonoBehaviour
    {
        /* Functions found in the plugin */
        #if UNITY_IPHONE
        [DllImport("__Internal")]
        private static extern void _TAG_startRecording();

        [DllImport("__Internal")]
        private static extern void _TAG_cancelRecording();

        [DllImport("__Internal")]
        private static extern void _TAG_SettingSpeech(string _language);

        /* "region Init" creates an instance of the current class, makes it static and then 
         * assigns it to a game object. It is required for the plugin to work. However, it 
         * might be possible to convert it to a more standard class, like WindowsVoiceController 
         * for better readability
         */
        #region Init
        static IOSVoiceControl _instance;
        public static IOSVoiceControl Instance
        {
            get
            {
                if (_instance == null)
                {
                    Init();
                }
                return _instance;
            }
        }
        public static void Init()
        {
            if (Instance != null) return;
            GameObject obj = new GameObject();
            obj.name = "IOSVoiceControl";
            _instance = obj.AddComponent<IOSVoiceControl>();
        }
        void Awake()
        {
            _instance = this;
        }
        #endregion

        public Action<string> onResultCallback;

        public void Setting(string _language)
        {
            _TAG_SettingSpeech(_language);
        }

        public void StartRecording(string _message = "")
        {
            System.Console.WriteLine(_message);
            _TAG_startRecording();
        }

        public void LogMessage(string _message)
        {
            Debug.Log(_message);
        }

        /* Called by plugin when recognition results are ready. */
        public void onResults(string _results)
        {
            if (onResultCallback != null)
            {
                onResultCallback("");
                onResultCallback(_results);
            }
        }   

        /* Used to toggle voice recognition on or off */
        public void ToggleRecognition(bool flag)
        {
            if (flag)
            {
                _TAG_startRecording();
            }
            else
            {
                _TAG_cancelRecording();
            }
        }
        #endif

    }
}