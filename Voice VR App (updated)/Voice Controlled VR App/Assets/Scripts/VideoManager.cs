using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class VideoManager : MonoBehaviour
{
    private const float FASTFORWARD_SPEED = 2.0f;
    private const float REWIND_SECONDS = 0.5f;    // How long each rewind "step" should be in seconds (half a second)
    private const int ZOOM_VIEW = 20;    // Zoomed Field of View
    private const int NORMAL_VIEW = 60;    // Normal Field of View

    private int setZoomView = NORMAL_VIEW;
    private bool isZooming = false, isRewinding = false;
    private float zoomTimeCount, rewindTimeCount;   // "Timers" that count up to x seconds
    private double currentPlayerTime;   // used only to fix rewind bug on WebGL and iOS build
    private VideoPlayer videoPlayer;
    private Image loopButtonImg;

    // Awake is called before any Start function, regardless of whether the script instance is enabled
    private void Awake()
    {
        Time.timeScale = 1; // sets timeScale to "real time" for Time library
    }

    // Start is called before the first frame update
    private void Start()
    {
        /* Script no longer attached to video player. Changed to find object named "Video Player" 
        in the scene, but "Video Player" is of type GameObject so we have to reference the VideoPlayer 
        component attached to the "Video Player" GameObject */
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        videoPlayer.Prepare();

        // Find the Image component of the Loop Toggle button
        loopButtonImg = GameObject.Find("UICanvas").transform.Find("SettingsPanel")
            .transform.Find("Loop Toggle").gameObject.GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        ZoomUpdate();
        RewindUpdate();   
    }

    public void Play()
    {
        isRewinding = false;

        if (videoPlayer.canSetPlaybackSpeed)
            videoPlayer.playbackSpeed = 1;
        else
            Debug.LogWarning("Changing playback speed is not supported on this platform");

        videoPlayer.Play();
    }

    public void Pause()
    {
        isRewinding = false;
        videoPlayer.Pause();
    }

    /// <summary>
    /// Stops a video from playing and causes the video to start at the beginning on next play
    /// </summary>
    public void Stop()
    {
        isRewinding = false;
        videoPlayer.Stop();
        // Consideration: Add logic that resets video progress bar to start on stop
    }

    /// <summary>
    /// Move paused video forward by one frame.
    /// </summary>
    public void StepForward()
    {
        isRewinding = false;
        videoPlayer.StepForward();
    }

    /// <summary>
    /// Move paused video backward by one frame.
    /// </summary>
    /// Lags a bit compared to StepForward().
    public void StepBackward()
    {
        isRewinding = false;
        videoPlayer.frame--;
    }

    public void FastForward()
    {
        isRewinding = false;
        if (videoPlayer.canSetPlaybackSpeed)
            videoPlayer.playbackSpeed = FASTFORWARD_SPEED;
        else
            Debug.LogWarning("Changing playback speed is not supported on this platform");
    }

    public void Rewind()
    {
        videoPlayer.Pause();
        isRewinding = true;
        rewindTimeCount = 0;

        #if UNITY_WEBGL || UNITY_IOS
        currentPlayerTime = videoPlayer.time;
        #endif
    }


    /// <summary>
    /// Toggles between the "Normal" and "Zoomed" FoVs and starts ZoomUpdate
    /// </summary>
    public void Zoom()
    {
        isZooming = true;
        zoomTimeCount = 0;
        if (setZoomView == NORMAL_VIEW)
        {
            setZoomView = ZOOM_VIEW;
        }
        else
        {
            setZoomView = NORMAL_VIEW;
        }
    }

    /// <summary>
    /// Gets the current state of the player, true if it is actively playing a video and false otherwise
    /// </summary>
    /// <returns>if a video is playing</returns>
    public bool isPlaying()
    {
        return videoPlayer.isPlaying;
    }

    /// <summary>
    /// Helper method to update the view to the setZoomView
    /// </summary>
    private void ZoomUpdate()
    {
        if (isZooming)
        {
            if (zoomTimeCount < 1)  // zoomTimeCount is less than 1 second
            {
                zoomTimeCount += Time.deltaTime;

                /* Smoothly changes between the camera's current FoV to the setZoom view using
                zoomTimeCount as the interpolation value between 0 (no change) to 1 (fully changed)
                (i.e. over the course of one second) */
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, setZoomView, zoomTimeCount);
            }
            else
            {
                zoomTimeCount = 0;
                isZooming = false;
            }
        }
    }

    /// <summary>
    /// Every REWIND_SECONDS, reduce the player's time by the same amount of seconds. 
    /// This way the video is rewinded in consistent time "chunks" instead of in variable sized 
    /// chunks as quickly as possible
    /// </summary>
    private void RewindUpdate()
    {
        if (isRewinding)
        {
            rewindTimeCount += Time.deltaTime;

            if (rewindTimeCount > REWIND_SECONDS) // rewindTimeCount is greater than REWIND_SECONDS
            {
                if (videoPlayer.time < REWIND_SECONDS)  // if the time left is less than a REWIND_SECONDS "step"
                {
                    // Set the time to exactly 0, stop rewinding
                    videoPlayer.time = 0;
                    isRewinding = false;
                }
                else
                {
                    /* WebGL and iOS exhibits a bug where rewinding with the code under #else prevents the 
                    rewind from visually appearing and at times from rewinding any at all. Unknown why adding 
                    another variable "fixes" it */
                    #if UNITY_WEBGL || UNITY_IOS
                    currentPlayerTime -= REWIND_SECONDS;
                    videoPlayer.time = currentPlayerTime;
                    #else
                    videoPlayer.time -= REWIND_SECONDS;
                    #endif
                }

                rewindTimeCount = 0;
            }
        }
    }

    /// <summary>
    /// Toggles loop on video. Also makes the respective button change color for better visibility.
    /// This function does not start the video automatically after it is toggled off
    /// </summary>
    public void LoopVideo()
    {
        var tempTime = videoPlayer.time;
        videoPlayer.isLooping = !videoPlayer.isLooping;
        if (videoPlayer.isLooping)
        {
            loopButtonImg.color = Color.cyan;
        }
        else
        {
            loopButtonImg.color = Color.white;
            videoPlayer.time = tempTime;
        }
    }
}
