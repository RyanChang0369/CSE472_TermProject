using System;
using UnityEngine;
using UnityEngine.Events;
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
    #region Variables
    public float animationDuration = 1;

    private float animationTimer = 1;

    /// <summary>
    /// 1 = opening, -1 = closing, 0 = neither.
    /// </summary>
    private int animationDirection = 0;

    public Button showHideMenuButton;

    public Button fileUploadButton;
    #endregion

    public UnityEvent<Texture> textureUpdateAvailable;

    private void Start()
    {
        showHideMenuButton.onClick.AddListener(ToggleAnimationDirection);
        fileUploadButton.onClick.AddListener(FileUploadButton_OnClick);
    }

    private void FileUploadButton_OnClick()
    {
        FileDialogManager.Instance.OpenFileDialog(
            OnFileReadDialogOpen,
            "image/*"
        );
    }

    private void OnFileReadDialogOpen(bool valid, string uri)
    {
        if (valid)
        {
            WebClientExt.GetTexture(
                "file://" + uri,
                (texture) => textureUpdateAvailable.Invoke(texture),
                (message) => Debug.LogError(message)
            );
        }
    }

    private void ToggleAnimationDirection()
    {
        if (animationDirection == 0)
        {
            // If more than halfway open, then close.
            // Otherwise, open.
            animationDirection = animationTimer >= 0.5f ? -1 : 1;
        }
        else
        {
            animationDirection = -animationDirection;
        }
    }

    private void Update()
    {
        if (animationDirection != 0)
        {
            animationTimer = Time.deltaTime * animationDirection / animationDuration;

            if (animationTimer >= 1)
            {
                // Now fully open. Stop animating.
                animationTimer = 1;
                animationDirection = 0;
            }
            else if (animationTimer <= 0)
            {
                // Now fully closed. Stop animating.
                animationTimer = 0;
                animationDirection = 0;
            }
        }
    }
}