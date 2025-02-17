using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A pull up menu specific for the demo.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class PullUpMenu : MonoBehaviour
{
    public FileDialogButton showHideButton;

    private void Start()
    {
        this.RequireComponentInChildren(out showHideButton);
        showHideButton.Setup(
            (valid, uri) =>
            {

            },
        ".png, .jpg, .jpeg");
    }
}