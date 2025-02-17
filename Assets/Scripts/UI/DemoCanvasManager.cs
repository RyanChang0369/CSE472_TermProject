using System;
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
    #endregion
    #endregion

    private void Awake()
    {
        this.InstantiateComponent();
    }

    private void Start()
    {
        this.RequireComponentInChildren(out pullUpMenu);
    }
}