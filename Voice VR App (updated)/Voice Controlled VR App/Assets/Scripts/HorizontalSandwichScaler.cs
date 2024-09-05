using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// "Sandwiches" the attached GameObject between two RectTransforms, scaling it to fit between them when they are close
/// and preserving its size when they are far away. Works only when parent Canvas is using Canvas Scaler component with 
/// mode "Constant Physical Size" and the attached GameObject has no other scaled parent.
/// </summary>
/// For more information on 3D World Space and transform.position, see:
/// https://docs.unity3d.com/2018.4/Documentation/ScriptReference/Transform-position.html
/// 
/// Note: If you want to add padding to your GameObject so that it always has some space between the RectTransforms
/// Make a parent GameObject, create spacing for the child, and then add this component script to the parent
[RequireComponent(typeof(RectTransform))]
public class HorizontalSandwichScaler : MonoBehaviour
{
    /* Note: Attached through editor, no "finding" function used */
    public GameObject ParentCanvas;
    public RectTransform LeftRectTransform, RightRectTransform;

    private RectTransform thisTransform;
    private Vector3 startScale;

    // Start is called before the first frame update
    void Start()
    {
        // Issue Canvas Scaler warnings
        CanvasScaler parentScaler = ParentCanvas.GetComponent<CanvasScaler>();
        if (parentScaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPhysicalSize)
        {
            Debug.LogWarning("Canvas scaling mode is not set to Constant Physical Size, " +
                this.GetType().Name + " may fail");
        }
        else if (parentScaler.physicalUnit != CanvasScaler.Unit.Points)
        {
            Debug.LogWarning("Canvas scaling units not set to Points (1/72 of an inch), " +
                this.GetType().Name + " may fail");
        }

        thisTransform = (RectTransform)this.transform;
        startScale = thisTransform.localScale;  // obtain the starting Scale set in the Editor
        Scale();
    }

    /// <summary>
    /// Scales the attached GameObject between two RectTransforms, scaling down when there is
    /// insufficient space between them for the GameObject's original size and scaling back up
    /// as space is created up to its original size.
    /// </summary>
    /// Called once at the start, and then subsequently can be called by other scripts such as
    /// ResolutionChangeDetector
    public void Scale()
    {
        if (thisTransform == null) return;   // wait until thisTransform initialized

        // Reset x and y scales
        thisTransform.localScale = startScale;

        /* While the right edge of the left rectangle collides with the left edge of this object's rectangle
        OR the left edge of the right rectangle collides with the right edge of this object's rectangle */
        while (rightEdgePosition(LeftRectTransform) > leftEdgePosition(thisTransform) ||
            leftEdgePosition(RightRectTransform) < rightEdgePosition(thisTransform))
        {
            // Decrement x and y scales of this object until it no longer collides
            thisTransform.localScale -= new Vector3(0.01f, 0.01f);
        }

        // Scale the GameObject's anchored position proportionally to the amount it was scaled
        // Reason this is commented out: Doesn't look very good since other UI elements don't perform similar
        // behavior and unsure whether it makes sense to scale position in this script or do it in separate script
        //if (SCALE_POSITION)
        //{
        //    float proportionalScaleX = thisTransform.localScale.x / initialScale.x;
        //    float proportionalScaleY = thisTransform.localScale.y / initialScale.y;
        //    thisTransform.anchoredPosition = new Vector2(startPosition.x * proportionalScaleX, startPosition.y * proportionalScaleY);
        //}
    }

    // Update is called once per frame
    void Update(){}

    /// <summary>
    /// Calculates left edge's position of the given RectTransform in World Space
    /// </summary>
    /// <param name="tform">A GameObject's RectTransform</param>
    /// <returns>The edge's x position in 3D World Space</returns>
    /// Because tform.rect.width gives us the actual value in the Unity Editor, we must
    /// not only scale down this value by it's own scale but also by the scale of its Canvas parent.
    /// It is strongly suspected World Space (given by transform.position) is in pixels;
    /// transform.rect is in points
    /// 
    /// Warning: Will fail if the tform's GameObject has more than one scaled parent.
    private float leftEdgePosition(RectTransform tform)
    {
        return tform.position.x - tform.rect.width / 2 * tform.localScale.x * ParentCanvas.transform.localScale.x;
    }

    /// <summary>
    /// Calculates right edge's position of the given RectTransform in World Space
    /// </summary>
    /// <param name="tform">A GameObject's RectTransform</param>
    /// <returns>The edge's x position in 3D World Space</returns>
    /// Because tform.rect.width gives us the actual value in the Unity Editor, we must
    /// not only scale down this value by it's own scale but also by the scale of its Canvas parent.
    /// It is strongly suspected World Space (given by transform.position) is in pixels;
    /// transform.rect is in points
    /// 
    /// Warning: Will fail if the tform's GameObject has more than one scaled parent.
    private float rightEdgePosition(RectTransform tform)
    {
        return tform.position.x + tform.rect.width / 2 * tform.localScale.x * ParentCanvas.transform.localScale.x;
    }
}
