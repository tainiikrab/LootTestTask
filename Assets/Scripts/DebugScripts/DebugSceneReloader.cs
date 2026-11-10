using LootTestTask;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LootTestTask.DebugScripts
{
public class DebugSceneReloader : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;

    public void Update()
    {
        if (_inputManager != null && _inputManager.IsReloadLevelTriggered())
            ReloadScene();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
}