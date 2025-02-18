using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager for the demo canvas.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class DemoCanvasManager : MonoBehaviour
{
    #region Instance
    /// <summary>
    /// The static reference to a(n) 
    /// <see cref="DemoCanvasManager"/> instance.
    /// </summary>
    private static DemoCanvasManager instance;

    /// <inheritdoc cref="instance"/>
    public static DemoCanvasManager Instance => instance;
    #endregion

    #region Variables
    #region Settings
    private PullUpMenu pullUpMenu;

    [SerializeField]
    private RawImage canvasImage;

    [SerializeField]
    private RectTransform popupContainer;

    [SerializeField]
    private RectTransform tooltipContainer;
    #endregion
    #endregion

    private void Awake()
    {
        this.InstantiateSingleton(ref instance);
    }

    private void Start()
    {
        this.RequireComponentInChildren(out pullUpMenu);
        pullUpMenu.textureUpdateAvailable.AddListener(UpdateTexture);
    }

    private void UpdateTexture(Texture texture)
    {
        canvasImage.texture = texture;
    }
}