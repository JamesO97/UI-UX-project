using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class CompassControl : MonoBehaviour
{
    public Camera MainCamera; //Note: Attached through editor, no "finding" function used
    private RawImage compassImage;

    
    // Start is called before the first frame update
    void Start()
    {
        // gets RawImage component in the attached GameObject
        compassImage = this.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        // updates texture's position based off camera's y-rotation
        compassImage.uvRect = new Rect(MainCamera.transform.eulerAngles.y / 360, 0, 1, 1);
    }
}
