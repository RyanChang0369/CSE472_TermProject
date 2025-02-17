using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Contains a min and max value.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
[System.Serializable]
public struct MinMax
{
    #region Enums
    /// <summary>
    /// The mode of which the minmax operates.
    /// </summary>
    public enum Mode
    {
        Between,
        NotBetween,
        GreaterThan,
        LessThan
    }
    #endregion

    #region Variables
    public Mode mode;

    [AllowNesting]
    [ShowIf(nameof(Min_ShowIf))]
    [SerializeField]
    private float min;

    [AllowNesting]
    [ShowIf(nameof(Max_ShowIf))]
    [SerializeField]
    private float max;

    #region Validation
    private readonly bool Min_ShowIf => mode != Mode.LessThan;
    private readonly bool Max_ShowIf => mode != Mode.GreaterThan;
    #endregion
    #endregion

    #region Constructors
    public MinMax(float min, float max) : this(min, max, Mode.Between)
    {

    }

    public MinMax(float min, float max, Mode mode)
    {
        this.min = Mathf.Min(min, max);
        this.max = Mathf.Max(min, max);
        this.mode = mode;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Determines if <paramref name="val"/> satisfies this range.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public readonly bool Evaluate(float val)
    {
        return mode switch
        {
            Mode.Between => min <= val && val <= max,
            Mode.LessThan => val <= max,
            Mode.GreaterThan => val >= min,
            Mode.NotBetween => min > val && val > max,
            _ => false
        };
    }
    #endregion

    #region ToString
    public override readonly string ToString() => mode switch
    {
        Mode.Between => $"Between [{min}, {max}]",
        Mode.LessThan => $"Less than or equal to {max}",
        Mode.GreaterThan => $"Greater than or equal to {min}",
        _ => $"Not between ({min}, {max})"
    };
    #endregion
}