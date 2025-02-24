using UnityEngine;

/// <summary>
/// A small script that syncs SliderCombo with a configured
/// MaterialParameterSetter.
/// </summary>
///
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class MaterialParameterInitialLink : MonoBehaviour
{
    private void Start()
    {
        this.RequireComponentInChildren(out MaterialParameterSetter setter);
        this.RequireComponentInChildren(out SliderCombo slider);

        object setterParam = setter.GetParameter();

        if (setterParam.IsFloatingPoint())
            slider.Value = (float)setterParam;

        if (setterParam.IsInteger())
            slider.Value = (float)setterParam;
    }
}