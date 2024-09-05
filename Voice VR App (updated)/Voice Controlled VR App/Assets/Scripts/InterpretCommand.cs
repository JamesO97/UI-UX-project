using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterpretCommand
{
    private Dictionary<string, Action> commandArray;
    private VideoManager videoManager;
    private CameraControllerManager camera;
    private const int MAXLENGTH = 20;

    public Dictionary<string, Action> CommandArray { get => commandArray; }

    public InterpretCommand()
    {
        videoManager = GameObject.Find("VideoManagerScript").GetComponent<VideoManager>();
        camera = GameObject.Find("Camera").GetComponent<CameraControllerManager>();
        commandArray = new Dictionary<string, Action>();

        /* Video Controls */
        commandArray.Add("play", videoManager.Play);
        commandArray.Add("pause", videoManager.Pause);
        commandArray.Add("stop", videoManager.Stop);
        commandArray.Add("zoom", videoManager.Zoom);
        commandArray.Add("fast forward", videoManager.FastForward);
        commandArray.Add("rewind", videoManager.Rewind);

        /* Direction Controls ("Look <direction>" or "Snap <"point" direction>") */
        // "Natural" Directions
        commandArray.Add("stay", camera.StopTurn); // Testing this out
        commandArray.Add("look left", () => camera.Turn(Direction.LEFT));   // snap doesn't make sense for Left or Right
        commandArray.Add("look right", () => camera.Turn(Direction.RIGHT)); // since they are not "points"
        commandArray.Add("look up", () => camera.Turn(Direction.UP));
        commandArray.Add("look down", () => camera.Turn(Direction.DOWN));
        commandArray.Add("look half up", () => camera.Turn(Direction.HALFUP));
        commandArray.Add("look half down", () => camera.Turn(Direction.HALFDOWN));
        commandArray.Add("look forward", () => camera.Turn(Direction.FORWARD));
        commandArray.Add("snap up", () => camera.Turn(Direction.UP, TurnSpeed.SNAP));
        commandArray.Add("snap down", () => camera.Turn(Direction.DOWN, TurnSpeed.SNAP));
        commandArray.Add("snap half up", () => camera.Turn(Direction.HALFUP, TurnSpeed.SNAP));
        commandArray.Add("snap half down", () => camera.Turn(Direction.HALFDOWN, TurnSpeed.SNAP));
        commandArray.Add("snap forward", () => camera.Turn(Direction.FORWARD, TurnSpeed.SNAP));

        // Cardinal Directions - "In-betweens" must be parsed first due to limitation with "String.Contains" method
        commandArray.Add("look northeast", () => camera.Turn(Direction.NORTHEAST));
        commandArray.Add("snap northeast", () => camera.Turn(Direction.NORTHEAST, TurnSpeed.SNAP));
        commandArray.Add("look northwest", () => camera.Turn(Direction.NORTHWEST));
        commandArray.Add("snap northwest", () => camera.Turn(Direction.NORTHWEST, TurnSpeed.SNAP));
        commandArray.Add("look southeast", () => camera.Turn(Direction.SOUTHEAST));
        commandArray.Add("snap southeast", () => camera.Turn(Direction.SOUTHEAST, TurnSpeed.SNAP));
        commandArray.Add("look southwest", () => camera.Turn(Direction.SOUTHWEST));
        commandArray.Add("snap southwest", () => camera.Turn(Direction.SOUTHWEST, TurnSpeed.SNAP));
        commandArray.Add("look north", () => camera.Turn(Direction.NORTH));
        commandArray.Add("snap north", () => camera.Turn(Direction.NORTH, TurnSpeed.SNAP));
        commandArray.Add("look south", () => camera.Turn(Direction.SOUTH));
        commandArray.Add("snap south", () => camera.Turn(Direction.SOUTH, TurnSpeed.SNAP));
        commandArray.Add("look east", () => camera.Turn(Direction.EAST));
        commandArray.Add("snap east", () => camera.Turn(Direction.EAST, TurnSpeed.SNAP));
        commandArray.Add("look west", () => camera.Turn(Direction.WEST));
        commandArray.Add("snap west", () => camera.Turn(Direction.WEST, TurnSpeed.SNAP));
    }

    /// <summary>
    /// If the string passed contains one of the keywords for playback, it triggers the command on
    /// the video player. The string must have a max length of 20 chars
    /// </summary>
    /// MAXLENGTH only really affects platforms which pass full speech strings into parse (Android, WebGL)
    /// <param name="command">the voice command string</param>
    /// <param name="isExactCommand">if the command is exactly worded</param>
    public void Parse(string command, bool isExactCommand = false)
    {
        if (command.Length > MAXLENGTH)
        {
            // Debug.Log("Exceeded " + MAXLENGTH + " characters, likely not a command. Discarded.");
            return;
        }
        // if (Debug.isDebugBuild) PrintToScene(command); // Debug.Log(command);

        /* Small optimization for platforms that return the exact string commands and not full
        freeform strings of text (Windows); makes it so we use the command as a key into the
        dictionary instead of iterating over the entire dictionary */
        if (isExactCommand)
        {
            commandArray[command].Invoke();
            return; // complete parsing
        }

        /* Iterate over the dictionary and find the correct command to invoke/call. Although
        less performant, it is necessary for platforms which pass in full freeform strings of
        text to Parse */
        command = command.ToLower();
        foreach (KeyValuePair<string, Action> entry in commandArray)
        {
            if (command.Contains(entry.Key))
            {
                entry.Value.Invoke();
                return; // complete parsing
            }
        }
    }

    /// <summary>
    /// For debugging purposes. Enables text component in the scen and then passes a string to it
    /// </summary>
    /// <param name="str"></param>
    public void PrintToScene(string str)
    {
        GameObject uiCanvas = GameObject.Find("UICanvas");

        if (uiCanvas.GetComponent<Text>().enabled == false)
        {
            uiCanvas.GetComponent<Text>().enabled = true;
        }

        uiCanvas.GetComponent<Text>().text = str;

    }

}

