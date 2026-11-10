using System;
using LootTestTask.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LootTestTask.UI
{
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    private InputManager _inputManager;

    [SerializeField] private TextMeshProUGUI _itemNameLabel;
    [SerializeField] private TextMeshProUGUI _itemAmountLabel;
    [SerializeField] private TextMeshProUGUI _itemWeightLabel;

    [SerializeField] private string _amountStringFormat = "Кол-во: {0}/{1}";
    [SerializeField] private string _weightStringFormat = "Вес: {0}";

    public InventorySlot Slot { get; private set; }

    public event Action<InventorySlotUI, Vector2> Clicked;

    public void SetInputManager(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    public void InitSlot(InventorySlot slot)
    {
        Slot = slot;
        UpdateUI();
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void HideSlot()
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        _itemNameLabel.text = Slot.Item.Name;
        _itemAmountLabel.SetText(_amountStringFormat, Slot.Amount, Slot.Item.MaxStack);
        _itemWeightLabel.SetText(_weightStringFormat, Slot.Item.Weight * Slot.Amount);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Slot?.Item == null || _inputManager == null) return;
        if (!_inputManager.IsUIClickTriggered()) return;

        Clicked?.Invoke(this, eventData.position);
    }
}
}