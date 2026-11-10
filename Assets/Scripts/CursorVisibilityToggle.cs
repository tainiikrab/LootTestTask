using UnityEngine;

namespace LootTestTask
{
public class CursorVisibilityToggle : MonoBehaviour
{
    private bool _isCursorVisible;
    private bool _gameplayLocked;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private bool _hideOnStart = true;

    private void Start()
    {
        _isCursorVisible = !_hideOnStart;
        Apply();
    }

    private void Update()
    {
        if (_inputManager == null || !_inputManager.IsHideCursorTriggered())
            return;

        _isCursorVisible = !_isCursorVisible;
        Apply();
    }

    public void SetGameplayLocked(bool locked)
    {
        _gameplayLocked = locked;
        Apply();
    }

    private void Apply()
    {
        bool visible = _gameplayLocked || _isCursorVisible;
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
}
