using System;
using UnityEngine;

/// <summary>
/// Extensions for <see cref="AnimationCurve"/>.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public static class CurveExt
{
    #region Keys
    /// <inheritdoc cref="ValidateCurve(AnimationCurve)"/>
    /// <summary>
    /// Retrieves the maximal time value in <see cref="AnimationCurve.keys"/> of
    /// <see cref="curve"/>.
    /// </summary>
    public static float MaxTime(this AnimationCurve curve)
    {
        ValidateCurve(curve);
        return curve[curve.length - 1].time;
    }

    /// <inheritdoc cref="ValidateCurve(AnimationCurve)"/>
    /// <summary>
    /// Retrieves the minimal time value in <see cref="AnimationCurve.keys"/> of
    /// <see cref="curve"/>.
    /// </summary>
    public static float MinTime(this AnimationCurve curve)
    {
        ValidateCurve(curve);
        return curve[0].time;
    }

    /// <inheritdoc cref="ValidateCurve(AnimationCurve)"/>
    /// <summary>
    /// Retrieves the key with the maximum value.
    /// </summary>
    public static Keyframe MaxValue(this AnimationCurve curve)
    {
        ValidateCurve(curve);
        return curve.keys.WithMaxValue(k => k.value);
    }
    
    /// <inheritdoc cref="ValidateCurve(AnimationCurve)"/>
    /// <summary>
    /// Retrieves the key with the minimal value.
    /// </summary>
    public static Keyframe MinValue(this AnimationCurve curve)
    {
        ValidateCurve(curve);
        return curve.keys.WithMinValue(k => k.value);
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Validates <paramref name="curve"/>, throwing errors if applicable.
    /// </summary>
    /// <param name="curve">The animation curve.</param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    private static void ValidateCurve(AnimationCurve curve)
    {
        if (curve == null)
            throw new ArgumentNullException(nameof(curve));

        if (curve.length == 0)
            throw new ArgumentOutOfRangeException(
                nameof(curve.length),
                curve.length,
                "Curve has no keys"
            );
    }
    #endregion
}