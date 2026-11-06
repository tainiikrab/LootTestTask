using UnityEngine;
using UnityEngine.InputSystem;

namespace LootTestTask
{
public class CursorVisibilityToggle : MonoBehaviour
{
    private bool _isCursorVisible;
    [SerializeField] private bool _hideOnStart = true;

    private void Start()
    {
        _isCursorVisible = !_hideOnStart;
        ToggleCursor(_isCursorVisible);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            _isCursorVisible = !_isCursorVisible;
            ToggleCursor(_isCursorVisible);
        }
    }

    private void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
}