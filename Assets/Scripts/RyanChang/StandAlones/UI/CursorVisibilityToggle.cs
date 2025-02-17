using UnityEngine;

/// <summary>
/// A StandAlone that hides or shows the cursor when called.
/// </summary>
public class CursorVisibilityToggle : MonoBehaviour
{
    public bool showOnStart = false;

    private void Start()
    {
        if (showOnStart)
        {
            Cursor.visible = true;
        }
    }

    public void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
    }
}