using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LootTestTask.UI
{
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory.Inventory _inventory;

    [SerializeField] private TextMeshProUGUI _weightLabel;
    [SerializeField] private TextMeshProUGUI _overencumberedLabel;
    [SerializeField] private InventorySlotUI _slotPrefab;
    [SerializeField] private ItemInteractionUI _itemInteraction;

    [SerializeField] private CanvasGroup _inventoryCanvasGroup;
    [SerializeField] private GridLayoutGroup _slotsGrid;
    [SerializeField] private string _weightStringFormat = "Вес: {0}/{1}";
    [SerializeField, Range(1f, 2f)] private float _overencumberedWarningSeconds = 1.5f;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private GameplayLock _gameplayLock;

    private bool _isVisible;
    private readonly List<InventorySlotUI> _slotsUI = new();
    private Coroutine _overencumberedRoutine;

    private void Awake()
    {
        _inventoryCanvasGroup.gameObject.SetActive(true);
        SetPanelVisible(false);
        _overencumberedLabel.gameObject.SetActive(false);
        _itemInteraction.Hide();
    }

    private void OnEnable()
    {
        _inputManager.InventoryPressed += OnInventoryPressed;
        _inventory.OverencumberedEvent += OnOverencumbered;
        _inventory.InventoryChangedEvent += OnInventoryChanged;
        _itemInteraction.ThrowAwayClicked += OnThrowAwayClicked;
        _itemInteraction.BreakDownClicked += OnBreakDownClicked;
    }

    private void OnDisable()
    {
        _inputManager.InventoryPressed -= OnInventoryPressed;
        _inventory.OverencumberedEvent -= OnOverencumbered;
        _inventory.InventoryChangedEvent -= OnInventoryChanged;
        _itemInteraction.ThrowAwayClicked -= OnThrowAwayClicked;
        _itemInteraction.BreakDownClicked -= OnBreakDownClicked;

        if (_overencumberedRoutine != null)
        {
            StopCoroutine(_overencumberedRoutine);
            _overencumberedRoutine = null;
        }

        _overencumberedLabel.gameObject.SetActive(false);
        _itemInteraction.Hide();
    }

    private void Start()
    {
        _weightLabel.SetText(_weightStringFormat, _inventory.CurrentWeight, _inventory.MaxWeight);
    }

    private void OnInventoryChanged()
    {
        _weightLabel.SetText(_weightStringFormat, _inventory.CurrentWeight, _inventory.MaxWeight);
        if (_isVisible)
            UpdateInventoryUI();
    }

    private void OnOverencumbered()
    {
        if (_overencumberedRoutine != null) return;
        _overencumberedRoutine = StartCoroutine(ShowOverencumberedWarning());
    }

    private IEnumerator ShowOverencumberedWarning()
    {
        _overencumberedLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(_overencumberedWarningSeconds);
        _overencumberedLabel.gameObject.SetActive(false);
        _overencumberedRoutine = null;
    }

    private void OnInventoryPressed()
    {
        ToggleInventoryUI(!_isVisible);
    }

    private void ToggleInventoryUI(bool toggle)
    {
        SetPanelVisible(toggle);
        _gameplayLock.SetLocked(toggle);
        if (!toggle)
        {
            _itemInteraction.Hide();
            return;
        }

        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        int i;
        for (i = 0; i < _inventory.Slots.Count; i++)
        {
            if (_slotsUI.Count <= i)
            {
                InventorySlotUI slot = Instantiate(_slotPrefab, _slotsGrid.transform);
                slot.SetInputManager(_inputManager);
                slot.Clicked += OnSlotClicked;
                _slotsUI.Add(slot);
            }

            _slotsUI[i].InitSlot(_inventory.Slots[i]);
        }

        for (int j = i; j < _slotsUI.Count; j++)
        {
            _slotsUI[j].HideSlot();
        }
    }

    private void OnSlotClicked(InventorySlotUI slotUI, Vector2 screenPosition)
    {
        _itemInteraction.Show(slotUI.Slot, screenPosition);
    }

    private void OnThrowAwayClicked()
    {
        _inventory.TryRemoveFromSlot(_itemInteraction.Slot);
        _itemInteraction.Hide();
    }

    private void OnBreakDownClicked()
    {
        _inventory.TryBreakDown(_itemInteraction.Slot);
        _itemInteraction.Hide();
    }

    private void SetPanelVisible(bool toggle)
    {
        _isVisible = toggle;
        _inventoryCanvasGroup.alpha = toggle ? 1f : 0f;
        _inventoryCanvasGroup.interactable = toggle;
        _inventoryCanvasGroup.blocksRaycasts = toggle;
    }
}
}