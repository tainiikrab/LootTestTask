using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LootTestTask
{
public class InputManager : MonoBehaviour
{
    public event Action InventoryPressed;

    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.UI.Enable();
        _inputActions.UI.Inventory.performed += OnInventoryPerformed;
    }

    public void TogglePlayerInput(bool enabled)
    {
        if (enabled) _inputActions.Player.Enable();
        else _inputActions.Player.Disable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _inputActions.UI.Disable();
        _inputActions.UI.Inventory.performed -= OnInventoryPerformed;
    }

    private void OnDestroy()
    {
        _inputActions.Dispose();
    }

    public Vector3 GetMovement()
    {
        var vec2 = _inputActions.Player.Move.ReadValue<Vector2>();
        return new Vector3(vec2.x, 0, vec2.y);
    }

    public bool IsJumpTriggered()
    {
        return _inputActions.Player.Jump.triggered;
    }

    public bool IsPickUpTriggered()
    {
        return _inputActions.Player.PickUp.triggered;
    }

    public bool IsReloadLevelTriggered()
    {
        return _inputActions.Player.ReloadLevel.triggered;
    }

    public bool IsHideCursorTriggered()
    {
        return _inputActions.Player.HideCursor.triggered;
    }

    public bool IsUIClickTriggered()
    {
        InputAction click = _inputActions.UI.Click;
        return click.WasPerformedThisFrame();
    }

    private void OnInventoryPerformed(InputAction.CallbackContext _)
    {
        InventoryPressed?.Invoke();
    }
}
}
