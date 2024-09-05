using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadVideoURL : MonoBehaviour
{
    //VideoPlayer videoPlayer;
    RenderTexture renderTexture;
    //Shader shader;
    //Skybox skybox;
    Material skyboxMaterial;
    //Camera mainCamera;

    uint height;
    uint width;

    public string filePath = string.Empty;
    //public string[] videoTypes = { "360", "180" };
    //public string[] vrTypes = { "Over Under", "Side by Side", "2D" };
    public string currentVideoType = "360";
    public string currentVrType = "2D";
    public int typeNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine( PrepareVideoPlayer() );
        //if the platform is webgl, load from absolute url
        //absolute url must be: "pathToWebglIndex.html?completeLinkToVideoFile"
        //the '?' character is important
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            string url = Application.absoluteURL;
            string[] urlsplit = url.Split('?');
            string videourl = urlsplit[1];
            filePath = videourl;
            LoadVideoFile();
        }


    }

    //This is called from the BrowseFiles script after a file is clicked in the file browser
    public void LoadVideoFile()
    {
        StartCoroutine( PrepareVideoPlayer() );
    }

    //Instead of dragging and dropping many components in the editor, this will 
    //set the required components through code. The must already be a Video Player in the scene
    IEnumerator PrepareVideoPlayer()
    {
        //Debug.Log("In Method: LoadVideoURL.PrepareVideoPlayer()");
        VideoPlayer videoPlayer = FindObjectOfType<VideoPlayer>();

        if (filePath == string.Empty)
        {
            Debug.Log("No video file selected, cancelling player prep");
            yield break;
        }

        videoPlayer.url = filePath;   
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        //Retrieve resolution from current file
        height = videoPlayer.height;
        width = videoPlayer.width;
        //Debug.Log("Video Resolution: ( " + height + " x " + width + " )");

        //Set rendermode to RenderTexture
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        Debug.Log(videoPlayer.renderMode.ToString());

        //Instantiate RenderTexture using retrieved resolution
        renderTexture = new RenderTexture((int)width, (int)height, 0);

        //Set videoPlayer targetTexture property to the renderTexture we just made
        videoPlayer.targetTexture = renderTexture;

        //Load prepared skybox material from resources:
        //This will load one of the skybox materials in the Resources folder
        //The combination of currentVideoType and currentVrType must match the names of one of those materials
        //currentVideoType and currentVrType are set when any of the buttons in the player settings is clicked
        //If you click the 180 button, then currentVideType becomes the string "180".
        //If you click the Side by Side button, then currentVrType becomes the string "Side by Side"
        //String concatenation is used to combine them and insert a space so that one of the resource names is matched
        skyboxMaterial = (Material)Resources.Load(currentVideoType + " " + currentVrType);

        //Set the skybox texture - Give it arbitrary name _MainTex
        skyboxMaterial.SetTexture("_MainTex", renderTexture);

        //Set the scene skybox to the skybox loaded from resources 
        RenderSettings.skybox = skyboxMaterial;
    }

    //Called by the onClick() method of the buttons in player settings
    public void setCurrentVideoType(string videoType)
    {
        currentVideoType = videoType;
        //Refactor so that the video player is prepared from scratch
        //Instead, just update the Skybox settings and reapply it to the scene
        StartCoroutine(PrepareVideoPlayer());
    }

    //Called by the onClick() method of the buttons in player settings
    public void setCurrentVrType(string vrType)
    {
        currentVrType = vrType;
        //Refactor so that the video player is prepared from scratch
        //Instead, just update the Skybox settings and reapply it to the scene
        StartCoroutine(PrepareVideoPlayer());
    }

    public void SetFilePath(string path)
    {
        filePath = path;
        LoadVideoFile();
    }
}
