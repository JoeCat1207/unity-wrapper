using UnityEngine;
// Support both legacy Input Manager and new Input System
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace SpectacularAI.Examples.Common
{
    /// <summary>
    /// Quits the application when escape is pressed.
    /// </summary>
    public class ApplicationQuitOnEscape : MonoBehaviour
    {
    void Update()
    {
        bool quitTriggered = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            quitTriggered = true;
#else
        if (Input.GetKeyDown(KeyCode.Escape))
            quitTriggered = true;
#endif

        if (quitTriggered)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
    }
}
