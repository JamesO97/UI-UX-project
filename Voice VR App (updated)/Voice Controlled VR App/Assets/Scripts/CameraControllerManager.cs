using UnityEngine;

/// <summary>
/// Represents points or directions in 3D space
/// </summary>
/// Explicit integer values index into DIR_ANGLES, ensure to change those if modifying this enum
public enum Direction
{
    NORTH = 0,
    NORTHEAST = 1,
    EAST = 2,
    SOUTHEAST = 3,
    SOUTH = 4,
    SOUTHWEST = 5,
    WEST = 6,
    NORTHWEST = 7,
    UP = 8,
    HALFUP = 9,
    FORWARD = 10,
    HALFDOWN = 11,
    DOWN = 12,
    LEFT,
    RIGHT,
    NONE
}

/// <summary>
/// Predefined speeds that the camera may turn
/// </summary>
public enum TurnSpeed
{
    SNAP, SLOW
}

/// This script is used to make sure only one of the camera controller scripts is enabled depending on the 
/// current running platform and to handle programmatic turning of the camera based on predefined points
public class CameraControllerManager : MonoBehaviour
{
    CameraController cameraController_script;
    MobileTouchCameraController mobileTouchCamera_script;
    private Direction direction = Direction.NONE;
    private Quaternion startRotation, endRotation;
    private float turnSpeed, timeCount, currAngleY, currAngleX, endAngleY;
    private bool isPointTurn = false;
    private const int TURN_SPEED_360 = 10;   // determines turn speed of directions that use Full360Turn

    /* "vertical" extremes "up" and "down" are off 1 degree to avoid unnatural rotations when looking up or down */
    private readonly int[] DIR_ANGLES = new int[] {0, 45, 90, 135, 180, 225, 270, 315, -89, -45, 0, 45, 89};

    /// <summary>
    /// Turns the camera to some direction at a given speed
    /// </summary>
    /// <param name="dir">Direction the camera will turn</param>
    /// <param name="speed">Speed of the turn</param>
    public void Turn(Direction dir, TurnSpeed speed = TurnSpeed.SLOW)
    {
        isPointTurn = !(dir == Direction.LEFT || dir == Direction.RIGHT);
        if (isPointTurn)
        {
            if (speed == TurnSpeed.SLOW)
                turnSpeed = 0.1f;
            else if (speed == TurnSpeed.SNAP)
                turnSpeed = 2.0f;

            timeCount = 0f;
            startRotation = transform.rotation;

            /* The integer value of the dir indexes into DIR_ANGLES and gives the angle of rotation
            that represents the direction. "Vertical" directions (e.g. Up, Down, etc.) are separate 
            because we want to keep the current y rotation and simulate a natural up-down motion */
            if (dir == Direction.UP || dir == Direction.HALFUP || dir == Direction.FORWARD 
                || dir == Direction.HALFDOWN || dir == Direction.DOWN)
            {
                endRotation = Quaternion.Euler(DIR_ANGLES[(int)dir], transform.eulerAngles.y, 0);
            }
            else
            {
                endRotation = Quaternion.Euler(0, DIR_ANGLES[(int)dir], 0);
            }

            // Debug.Log("End Rotation Angle set to: " + DIR_ANGLES[(int)dir]);
        }
        else // Full360Turn (left-right turn)
        {
            // determine direction of rotation (Left = -1, Right = 1)
            int rotationSign = (dir == Direction.LEFT ? -1 : 1);

            turnSpeed = rotationSign * TURN_SPEED_360;
            currAngleX = transform.rotation.eulerAngles.x;
            currAngleY = transform.rotation.eulerAngles.y;
            endAngleY = currAngleY + rotationSign * 360;
        }

        // start turning
        direction = dir;
    }

    /// <summary>
    /// Stops turning the camera if it was in the middle of a programmatic turn
    /// </summary>
    public void StopTurn()
    {
        if (direction != Direction.NONE)
            direction = Direction.NONE;
    }

    /// <summary>
    /// Turns to some predefined point in 3D space every frame
    /// </summary>
    private void PointTurn()
    {
        /* Smoothly rotates between startRotation and endRotation using timeCount
        as the interpolation value between 0 (no rotation) to 1 (fully rotated) */
        timeCount += Time.deltaTime * turnSpeed;
        transform.rotation = Quaternion.Lerp(startRotation, endRotation, timeCount);

        if (timeCount > 1)
            StopTurn();
    }

    /// <summary>
    /// Turns 360 degrees on some axis at a constant speed every frame (currently only Y-axis to simulate "left"-"right" turns)
    /// </summary>
    private void Full360Turn()
    {   
        transform.rotation = Quaternion.Euler(currAngleX, currAngleY, 0);
        currAngleY += Time.deltaTime * turnSpeed;

        // when you have reached the desired Y rotation
        if ((direction == Direction.RIGHT && currAngleY > endAngleY) ||
            (direction == Direction.LEFT && currAngleY < endAngleY))
        {
            // end exactly at endAngle
            transform.rotation = Quaternion.Euler(currAngleX, endAngleY, 0);
            StopTurn();
        }
    }

    // Awake is called before any Start function, regardless of whether the script instance is enabled
    private void Awake()
    {
        Time.timeScale = 1; // sets timeScale to "real time" for Time library
    }

    // Start is called before the first frame update
    void Start()
    {
        //First get a reference to the scripts we want to enable or disable
        cameraController_script = GetComponentInChildren<CameraController>();
        mobileTouchCamera_script = GetComponentInChildren<MobileTouchCameraController>();

        if (cameraController_script == null)
        {
            Debug.LogError("Could not find script component on this object");
        }

        if (mobileTouchCamera_script == null)
        {
            Debug.LogError("Could not find script component on this object");
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            cameraController_script.enabled = false;
        }
        else
        {
            mobileTouchCamera_script.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // DevKeyTest();

        if (direction != Direction.NONE)
        {
            if (isPointTurn)
            {
                PointTurn();
            }
            else
            {
                Full360Turn();
            }
        }
    }

    // Activates Turn controls using Keyboard input. For testing only.
    private void DevKeyTest()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Turn(Direction.NORTH, TurnSpeed.SNAP);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Turn(Direction.EAST, TurnSpeed.SNAP);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Turn(Direction.SOUTH, TurnSpeed.SNAP);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Turn(Direction.WEST, TurnSpeed.SNAP);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Turn(Direction.LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Turn(Direction.RIGHT);
        }
    }
}
