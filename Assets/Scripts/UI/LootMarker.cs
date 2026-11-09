using UnityEngine;
using UnityEngine.UI;

namespace LootTestTask.UI
{
[RequireComponent(typeof(RectTransform))]
public class LootMarker : MonoBehaviour
{
    [field: SerializeField] public RectTransform RectTransform { get; private set; }
    [field: SerializeField] public Graphic MarkerGraphic { get; private set; }

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color activeColor;

    public void SetDefaultColor()
    {
        MarkerGraphic.color = defaultColor;
    }

    public void SetActiveColor()
    {
        MarkerGraphic.color = activeColor;
    }

    public void ToggleVisibility(bool value)
    {
        MarkerGraphic.enabled = value;
    }
}
}