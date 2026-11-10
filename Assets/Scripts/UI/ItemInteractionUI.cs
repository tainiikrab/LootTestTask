using System;
using LootTestTask.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LootTestTask.UI
{
public class ItemInteractionUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform _menuPanel;
    [SerializeField] private Button _breakDownButton;
    [SerializeField] private Button _throwAwayButton;

    public InventorySlot Slot { get; private set; }

    public event Action ThrowAwayClicked;
    public event Action BreakDownClicked;

    private void Awake()
    {
        _breakDownButton.onClick.AddListener(OnBreakDownClicked);
        _throwAwayButton.onClick.AddListener(OnThrowAwayClicked);
    }

    private void OnDestroy()
    {
        _breakDownButton.onClick.RemoveListener(OnBreakDownClicked);
        _throwAwayButton.onClick.RemoveListener(OnThrowAwayClicked);
    }

    public void Show(InventorySlot slot, Vector2 screenPosition)
    {
        gameObject.SetActive(true);
        Slot = slot;
        _breakDownButton.gameObject.SetActive(slot?.Item != null && slot.Item.HasBreakdownComponents);
        _menuPanel.position = screenPosition;
    }

    public void Hide()
    {
        Slot = null;
        gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Hide();
    }

    private void OnThrowAwayClicked()
    {
        ThrowAwayClicked?.Invoke();
    }

    private void OnBreakDownClicked()
    {
        BreakDownClicked?.Invoke();
    }
}
}