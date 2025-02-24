using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A slider/text combo ui element.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class SliderCombo : MonoBehaviour
{
    #region Variables
    #region Settings
    /// <summary>
    /// The text component.
    /// </summary>
    [Tooltip("The text component.")]
    [SerializeField]
    private TMPro.TMP_Text text;

    /// <summary>
    /// The slider component.
    /// </summary>
    [Tooltip("The slider component.")]
    [SerializeField]
    private Slider slider;
    #endregion
    #endregion

    #region Properties
    public float Value
    {
        get => slider.value;
        set
        {
            slider.value = value;

            if (slider.wholeNumbers)
                text.text = value.RoundToInt().ToString("d");
            else
                text.text = value.ToString("f2");
        }
    }
    #endregion

    #region Methods
    #region Instantiation
    private void Awake()
    {
        this.AutofillComponentInChildren(ref text);
        this.AutofillComponentInChildren(ref slider);

        slider.onValueChanged.AddListener((newValue) => Value = newValue);
    }

    private void Start()
    {
        // Yes, this is on purpose.
        Value = Value;
    }
    #endregion
    #endregion
}