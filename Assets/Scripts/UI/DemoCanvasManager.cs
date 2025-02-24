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

        if (canvasImage.HasComponent(out ImageAspectRatioFitter fitter))
        {
            fitter.SetLayoutVertical();
            // Jostle the canvas image around to update the image ratio fitter.
            ((RectTransform)canvasImage.transform).anchoredPosition = Vector2.zero;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #region Get Material Parameter
    public object GetMaterialFloat(string parameter)
    {
        return canvasImage.material.GetFloat(parameter);
    }

    public object GetMaterialInteger(string parameter)
    {
        return canvasImage.material.GetInteger(parameter);
    }


    public object GetMaterialTexture(string parameter)
    {
        return canvasImage.material.GetTexture(parameter);
    }


    public object GetMaterialColor(string parameter)
    {
        return canvasImage.material.GetColor(parameter);
    }
    #endregion


    #region Set Material Parameter
    public void SetMaterialParameter(string parameter, float value)
    {
        canvasImage.material.SetFloat(parameter, value);
    }

    public void SetMaterialParameter(string parameter, int value)
    {
        canvasImage.material.SetInteger(parameter, value);
    }

    public void SetMaterialParameter(string parameter, Texture value)
    {
        canvasImage.material.SetTexture(parameter, value);
    }

    public void SetMaterialParameter(string parameter, Color value)
    {
        canvasImage.material.SetColor(parameter, value);
    }
    #endregion
}