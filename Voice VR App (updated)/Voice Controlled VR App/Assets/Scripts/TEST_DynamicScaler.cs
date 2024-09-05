using UnityEngine;
/// <summary>
/// A proof of concept dynamic scaling script designed to be used on GameObjects that are children
/// of Canvas using a Canvas Scaler set to physical unit of Points. Currently, it will only scale down 
/// the attached GameObject to 3/4 its original size and position.
/// </summary>
/// 
/// **** READ ME ****
/// At the time of writing this, the biggest challenge in designing a responsive UI was deciding which
/// Canvas Scaler mode was best to achieve a good look and feel for each platform. Seemingly,
/// the best Canvas Scaler was "Constant Physical Size" as the other two would either make the
/// UI too small on mobile or too large on desktop when adjusting GameObject sizes. This script, along
/// with HorizontalSandwichScaler.cs and ResolutionChangeDetector were an exploration in keeping 
/// the same UI for every platform but resizing individual elements as the screen size changed. 
/// 
/// I have a suspicion scripts like this are going to be difficult to write and maintain, so
/// I suggest you explore the idea of having a different Unity Scene for each platform you wish
/// to support (i.e. DesktopScene, MobileScene, VRScene, etc). This way you only need to design a
/// UI with one platform in mind at a time, and potentially use a different configuration of the
/// Canvas Scaler for each (Constant Physical Size on mobile, Scale with Screen Size on desktop, etc).
[RequireComponent(typeof(RectTransform))]
public class TEST_DynamicScaler : MonoBehaviour
{
    private const int STANDALONE_BUTTON = 1, PLAYBACK_CONTROLS = 2, POINTS_PER_INCH = 72;
    private const float SCALE_FACTOR = 0.75f; // Arbitrarily scale down by 1/4
    private float PIXELS_PER_POINT;
    private int scaleType = 0;
    private RectTransform thisTransform;
    private Vector3 startScale, startPosition;

    // Start is called before the first frame update
    void Start()
    {
        switch (this.gameObject.name)
        {
            // Hardcoded GameObject references
            case "HamburgerMenuButtonContainer":
            case "VRToggleButtonContainer":
                scaleType = STANDALONE_BUTTON;
                break;
            case "PlaybackControls":
                scaleType = PLAYBACK_CONTROLS;
                break;
            default:
                Debug.LogError(this.GetType().Name + " is attached to unsupported GameObject!");
                break;
        }

        PIXELS_PER_POINT = Screen.dpi / POINTS_PER_INCH;
        thisTransform = (RectTransform)this.gameObject.transform;

        // Obtain the starting values for each property
        startScale = thisTransform.localScale;
        startPosition = thisTransform.anchoredPosition;

        Scale();
    }

    public void Scale()
    {
        if (thisTransform == null) return; // wait until thisTransform initialized

        // Reset x and y scales
        thisTransform.localScale = startScale;
        // Reset x and y positions
        thisTransform.anchoredPosition = startPosition;

        float thisPixelWidth = thisTransform.rect.width * PIXELS_PER_POINT;
        if (scaleType == STANDALONE_BUTTON)
        {
            if (thisPixelWidth > Screen.width * .2f)    // greater than 1/5 of screen width
            {
                // Arbitrarily scale down scale and anchored position by 1/4 (SCALE_FACTOR)
                thisTransform.localScale = new Vector3(thisTransform.localScale.x * SCALE_FACTOR, thisTransform.localScale.y * SCALE_FACTOR, 1f);
                thisTransform.anchoredPosition = new Vector2(thisTransform.anchoredPosition.x * SCALE_FACTOR, thisTransform.anchoredPosition.y * SCALE_FACTOR);
            }
        }
        else if (scaleType == PLAYBACK_CONTROLS)
        {
            if (thisPixelWidth > Screen.width)
            {
                // Arbitrarily scale down scale and anchored position by 1/4 (SCALE_FACTOR)
                thisTransform.localScale = new Vector3(thisTransform.localScale.x * SCALE_FACTOR, thisTransform.localScale.y * SCALE_FACTOR, 1f);
                thisTransform.anchoredPosition = new Vector2(thisTransform.anchoredPosition.x * SCALE_FACTOR, thisTransform.anchoredPosition.y * SCALE_FACTOR);
            }
        }
    }

    // Update is called once per frame
    void Update(){}
}
