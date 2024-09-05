using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VerticalIndicatorControl : MonoBehaviour
{
    public Camera MainCamera; //Note: Attached through editor, no "finding" function used
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        // gets Slider component in the attached GameObject
        slider = this.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        /* The value returned from eulerAngles will be between 0 to 360, however, 0 degrees starts
        from the "forward" direction and increases clockwise or "as you look down". This results
        in an uneven transition from "up" (270) to "forward" (0) to "down" (90). To amend this,
        we subtract 360 from the rotation if >180, this way the slider moves as expected. 
        Note the slider component must have Min = -90 and Max = 90 for this to work */
        float cameraXRotation = MainCamera.transform.rotation.eulerAngles.x;
        slider.value = cameraXRotation > 180 ? cameraXRotation - 360 : cameraXRotation;
    }
}
