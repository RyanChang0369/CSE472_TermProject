using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Some element that can bring up a tooltip.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class TooltipContext : MonoBehaviour,
    IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    #region Variables
    #region Settings
    /// <summary>
    /// A reference to a tooltip element.
    /// </summary>
    [Tooltip("A reference to a tooltip element.")]
    public Tooltip tooltipPrefab;

    /// <summary>
    /// The text of the tooltip.
    /// </summary>
    [Tooltip("The text of the tooltip.")]
    [SerializeField]
    [TextArea]
    private string tooltipText;
    #endregion

    #region Members
    private Tooltip tooltipInstance;
    #endregion
    #endregion

    #region Properties
    public string TooltipText
    {
        get => tooltipText;
        set
        {
            if (tooltipInstance)
                tooltipInstance.Text = value;

            tooltipText = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        if (!tooltipPrefab)
            throw new MissingReferenceException(
                "Missing reference for " + nameof(tooltipPrefab)
            );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!tooltipInstance && !TooltipText.IsNullOrEmpty())
        {
            tooltipInstance = Instantiate(tooltipPrefab, transform);
            tooltipInstance.Text = TooltipText;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (tooltipInstance)
        {
            tooltipInstance.PointerUpdate(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipInstance)
        {
            tooltipInstance.PointerExit();
        }
    }
    #endregion
}