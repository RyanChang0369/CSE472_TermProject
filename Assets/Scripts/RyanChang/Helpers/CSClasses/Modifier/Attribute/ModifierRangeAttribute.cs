using UnityEngine;

public class ModifierRangeAttribute : PropertyAttribute
{
    public readonly float multiplierMin;

    public readonly float multiplierMax;

    public readonly float additionMin;

    public readonly float additionMax;

    /// <summary>
    /// Constructs a modifier that uses sliders instead of values.
    /// </summary>
    /// <param name="multiplierMin">Minimum value for the multiplier.</param>
    /// <param name="multiplierMax">Maximum value for the multiplier.</param>
    /// <param name="additionMin">Minimum value for the addition.</param>
    /// <param name="additionMax">Maximum value for the addition.</param>
    public ModifierRangeAttribute(float additionMin, float additionMax,
        float multiplierMin, float multiplierMax)
    {
        this.multiplierMin = multiplierMin;
        this.multiplierMax = multiplierMax;
        this.additionMin = additionMin;
        this.additionMax = additionMax;
    }
}