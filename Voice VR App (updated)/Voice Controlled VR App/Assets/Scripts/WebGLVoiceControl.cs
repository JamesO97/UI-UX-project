#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

//alfonso: for testing

public class WebGLVoiceControl 
{
    /* javascript methods
     * 
     * Theses methods are the functions declared on the JavaScript plugin file.
     * These are declared here using the "[DllImport("__Internal")]", which needs
     * to be placed in a per-function basis. 
     */
    [DllImport("__Internal")]
    private static extern void startVoiceRec();

    [DllImport("__Internal")]
    private static extern void stopVoiceRec();

    [DllImport("__Internal")]
    private static extern void reStartVoiceRec();

    public WebGLVoiceControl()
    {
        /*
         * Constructor does not declare or initialize any variables but is 
         * needed in order to call the JavaScript functions. Constructor may be 
         * as intended in future development of the app
         */
    }

    /// <summary>
    /// starts the function on the javascript plugin. Javascript plugin must be placed under the 
    /// "plugins" folder on project and is importent using "Dllimport"
    /// </summary>
    public void Listen()
    {
        startVoiceRec();
    }


    /// <summary>
    /// Function used to enable or disable voice recognition.
    /// Accepts a boolean to either stop or restart the plugin.
    /// </summary>
    /// <param name="flag"></param>
    public void ToggleRecognition(bool flag)
    {
        if (flag)
        {
            reStartVoiceRec();
        }
        else
        {
            stopVoiceRec();
        }
    }
}

#endif
