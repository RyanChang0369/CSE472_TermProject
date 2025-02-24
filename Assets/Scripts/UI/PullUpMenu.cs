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
    #region Settings
    public float animationDuration = 1;
    #endregion

    #region References
    public Button showHideMenuButton;

    private TMPro.TMP_Text showHideMenuButtonText;

    public TMPro.TMP_InputField uriInputField;

    public Button uriSearchButton;
    #endregion

    #region Members
    private float animationTimer = 1;
    #endregion

    #region Events
    public UnityEvent<Texture> textureUpdateAvailable;

    #endregion
    #endregion

    #region Properties
    public int AnimationDirection { get; private set; } = 0;
    #endregion

    private void Start()
    {
        showHideMenuButton.onClick.AddListener(ToggleAnimationDirection);
        showHideMenuButton.RequireComponentInChildren(out showHideMenuButtonText);
        uriSearchButton.onClick.AddListener(UriSearchButton_OnClick);
    }

    private void UriSearchButton_OnClick()
    {
        WebClientExt.GetTexture(
            uriInputField.text,
            (texture) =>
            {
                textureUpdateAvailable.Invoke(texture);
                uriInputField.text = "";
            },
            HandleUriError
        );
    }

    private void OnFileReadDialogOpen(bool valid, string uri)
    {
        if (valid)
        {
            WebClientExt.GetTexture(
                "file://" + uri,
                (texture) => textureUpdateAvailable.Invoke(texture),
                HandleUriError
            );
        }
    }

    private void ToggleAnimationDirection()
    {
        if (AnimationDirection == 0)
        {
            // If more than halfway open, then close.
            // Otherwise, open.
            AnimationDirection = animationTimer >= 0.5f ? -1 : 1;
        }
        else
        {
            AnimationDirection = -AnimationDirection;
        }
    }

    private void HandleUriError(string message)
    {
        Debug.LogWarning(message);
    }

    private void Update()
    {
        if (AnimationDirection != 0)
        {
            animationTimer += Time.deltaTime * AnimationDirection / animationDuration;

            if (animationTimer >= 1)
            {
                // Now fully open. Stop animating.
                animationTimer = 1;
                AnimationDirection = 0;
                showHideMenuButtonText.text = "▼";
            }
            else if (animationTimer <= 0)
            {
                // Now fully closed. Stop animating.
                animationTimer = 0;
                AnimationDirection = 0;
                showHideMenuButtonText.text = "▲";
            }

            RectTransform rTransform = (RectTransform)transform;
            Vector3 position = rTransform.anchoredPosition;
            float btnHeight = ((RectTransform)showHideMenuButton.transform).sizeDelta.y;
            float a = -rTransform.sizeDelta.y + btnHeight;
            position.y = Mathf.Lerp(a, 0, animationTimer);
            rTransform.anchoredPosition = position;
        }
    }
}