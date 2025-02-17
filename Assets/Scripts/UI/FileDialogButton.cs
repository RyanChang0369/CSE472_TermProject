using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button for file opening.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class FileDialogButton : MonoBehaviour
{
    private Button button;
    private FileDialogManager.OnPathRetrieved callOnRetrieved;
    private string extensions;

    private void Start()
    {
        this.RequireComponent(out button);
        button.onClick.AddListener(FileDialogClicked);
    }

    private void FileDialogClicked()
    {
        FileDialogManager.Instance.OpenFileDialog(callOnRetrieved, extensions);
    }

    /// <summary>Setup the file dialog.</summary>
    /// <inheritdoc
    /// cref="FileDialogManager.OpenFileDialog(FileDialogManager.OnPathRetrieved,
    /// string)"/>
    public void Setup(FileDialogManager.OnPathRetrieved callOnRetrieved,
        string extensions)
    {
        this.callOnRetrieved = callOnRetrieved;
        this.extensions = extensions;
    }
}