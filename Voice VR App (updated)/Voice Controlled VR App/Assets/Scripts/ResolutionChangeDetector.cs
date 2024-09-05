using UnityEngine.EventSystems;

/// <summary>
/// Detects change in the Screen Resolution by reading the Canvas' RectTransform's dimensions
/// </summary>
public class ResolutionChangeDetector : UIBehaviour
{
    public HorizontalSandwichScaler DirectionalIndicatorScaler; //Note: Attached through editor, no "finding" function used

    // Called only when the attached GameObject's RectTransform's dimensions change
    protected override void OnRectTransformDimensionsChange()
    {
        DirectionalIndicatorScaler.Scale();
    }
}
