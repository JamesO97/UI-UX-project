using UnityEngine;

// This script was found here: https://answers.unity.com/questions/805630/how-can-%C4%B1-rotate-camera-with-touch.html
// Added speed variable and reversed signs of angles added to rotation so that rotation is inverted
public class MobileTouchCameraController : MonoBehaviour
{
    private Vector2 firstPoint;
    private Vector2 secondPoint;
    private float startAngleX = 0.0f;
    private float startAngleY = 0.0f;
    private CameraControllerManager cameraManager;

    // Start is called before the first frame update
    private void Start()
    {
        cameraManager = GetComponentInChildren<CameraControllerManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Check count touches
        if (Input.touchCount > 0)
        {
            //Touch began, save position
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                firstPoint = Input.GetTouch(0).position;
                startAngleX = this.transform.rotation.eulerAngles.x;
                startAngleY = this.transform.rotation.eulerAngles.y;

                // If "command" turning, stop
                cameraManager.StopTurn();
            }
            //Move finger by screen
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                secondPoint = Input.GetTouch(0).position;

                /*
                 * To better understand the calculation here, first take note of the quantity
                 * "(secondPoint - firstPoint) / Screen.dimension". Conceptually this is a fraction/proportion
                 * of the 2D device screen that has been moved by user input. Then, this fraction is multiplied
                 * by 360.0f to obtain an equivalent fraction out of a 360 degree circle (representing the 360 view of
                 * the camera) that should be moved. This is added to the startAngle to calculate the camera's new rotation.
                 * 
                 * Notice that movement on the device screen's x-axis is used to calculate the camera's y-axis rotation
                 * and vice-versa
                 */
                float newAngleX = startAngleX + (secondPoint.y - firstPoint.y) / Screen.width * 360.0f;
                float newAngleY = startAngleY - (secondPoint.x - firstPoint.x) / Screen.height * 360.0f;

                //Rotate camera
                this.transform.rotation = Quaternion.Euler(newAngleX, newAngleY, 0.0f);
            }
        }
    }
}
