using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A tooltip UI element.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class Tooltip : UIBehaviour
{
    #region Variables
    #region Settings
    /// <summary>
    /// The text UI element.
    /// </summary>
    [Tooltip("The text UI element.")]
    [SerializeField]
    private TMPro.TMP_Text textUI;
    #endregion

    #region Members
    private Canvas canvas;
    #endregion
    #endregion

    #region Properties
    /// <summary>
    /// The text of this tooltip.
    /// </summary>
    public string Text
    {
        get => textUI.text;
        set => textUI.text = value;
    }
    #endregion

    #region Methods
    protected override void Awake()
    {
        base.Awake();

        this.AutofillComponentInChildren(ref textUI);

        if (!this.HasComponentInAnyParent(out canvas))
        {
            throw new MissingComponentException(
                "Missing component " + nameof(canvas)
            );
        }

        // This forces the tooltip to appear in front of everything.
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void PointerUpdate(PointerEventData eventData)
    {
        // Set position to cursor location.
        RectTransform rectTransform = (RectTransform)transform;
        rectTransform.anchoredPosition = 
            eventData.position / canvas.scaleFactor + Vector2.one;
        print(rectTransform.anchoredPosition);
    }

    public void PointerExit()
    {
        Destroy(gameObject);
    }
    #endregion
}