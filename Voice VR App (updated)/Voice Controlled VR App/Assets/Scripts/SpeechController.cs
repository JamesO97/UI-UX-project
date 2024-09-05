using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class SpeechController : MonoBehaviour
{
    const string LANG_CODE = "en-US";
    Text uiText;

    void Start()
    {
        Setup(LANG_CODE);
#if UNITY_ANDROID
        SpeechToText.instance.onPartialResultsCallback = OnPartialSpeechResults;
#endif
        SpeechToText.instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.instance.onDoneCallback = OnSpeakStop;
        CheckPermission();
    }

    void CheckPermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif

    }

    #region Text to Speech

    public void StartSpeaking(string message) 
    {
        TextToSpeech.instance.StartSpeak(message);
    }

    public void StopSpeaking() 
    {
        TextToSpeech.instance.StopSpeak();
    }

    public void OnSpeakStart()
    {
        Debug.Log("Talking started...");
    }

    public void OnSpeakStop()
    {
        Debug.Log("Talking stopped...");
    }
    #endregion

    #region Speech to Text

    public void StartListening()
    {
        SpeechToText.instance.StartRecording();
    }

    public void StopListening()
    {
        SpeechToText.instance.StopRecording();
    }

    void OnFinalSpeechResult(string result)
    {
        uiText.text = result;
    }

    void OnPartialSpeechResults(string result)
    {
        uiText.text = result;
    }

    void Setup(string code)
    {
        TextToSpeech.instance.Setting(code, 1, 1);
        SpeechToText.instance.Setting(code);
    }
    #endregion
}
