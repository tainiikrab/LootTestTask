using TMPro;
using UnityEngine;

namespace LootTestTask.UI
{
[RequireComponent(typeof(RectTransform))]
public class LootMarkerUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _label;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color _overweightColor = Color.red;

    public void SetDefaultColor()
    {
        _label.color = defaultColor;
    }

    public void SetActiveColor()
    {
        _label.color = activeColor;
    }

    public void ToggleVisibility(bool value)
    {
        _rectTransform.gameObject.SetActive(value);
    }

    public void SetText(string name, int amount, int totalWeight, bool weightOverloaded = false)
    {
        string weightLine = $"{totalWeight} кг.";
        if (weightOverloaded)
        {
            weightLine = $"<color=#{ColorUtility.ToHtmlStringRGB(_overweightColor)}>{weightLine}</color>";
        }

        _label.text = $"{{...}}\n{name} ({amount})\n{weightLine}";
    }

    public void SetPosition(Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
    }
}
}
