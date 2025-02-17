using System;
using UnityEngine;

/// <summary>
/// Contains methods pertaining to C# floats, doubles, and ints.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public static class NumericalExt
{
    #region Comparison
    /// <summary>
    /// True if a differs from b by no more than <paramref name="margin"/>.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <param name="margin">The margin.</param>
    public static bool Approx(this float a, float b, float margin)
    {
        return Mathf.Abs(a - b) <= margin;
    }

    /// <inheritdoc cref="Approx(float, float, float)"/>
    /// <summary>
    /// Alias to <see cref="Mathf.Approximately(float, float)"/>
    /// </summary>
    public static bool Approx(this float a, float b)
    {
        return Mathf.Approximately(a, b);
    }

    /// <summary>
    /// Returns true if number is in between bounds A and B, inclusive
    /// </summary>
    /// <param name="number">The number to evaluate</param>
    /// <param name="boundsA">The lower bound</param>
    /// <param name="boundsB">The upper bound</param>
    /// <param name="fixRange">If true, swap bounds A and B if B < A.</param>
    public static bool IsBetween(this float number, float boundsA,
        float boundsB, bool fixRange = true)
    {
        if (fixRange)
        {
            float temp = boundsA;

            boundsA = Mathf.Min(boundsA, boundsB);
            boundsB = Mathf.Max(boundsB, temp);
        }

        return boundsA <= number && number <= boundsB;
    }
    #endregion

    #region Sign
    #region Positive
    /// <summary>
    /// Returns true if <paramref name="number"/> is greater than 0.
    /// </summary>
    /// <inheritdoc cref="Sign(int, SignBehavior)"/>
    /// <returns>True if <paramref name="number"/> is greater than 0.</returns>
    public static bool IsPositive(this int number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        behavior switch
        {
            SignBehavior.ZeroIsFalse => number > 0,
            SignBehavior.ZeroIsNegative => number > 0,
            SignBehavior.ZeroIsTrue => number >= 0,
            SignBehavior.ZeroIsPositive => number >= 0,
            _ => throw new NotImplementedException(),
        };

    /// <inheritdoc cref="IsPositive(int, SignBehavior)"/>
    public static bool IsPositive(this byte number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsPositive(behavior);

    /// <inheritdoc cref="IsPositive(int, SignBehavior)"/>
    public static bool IsPositive(this short number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsPositive(behavior);

    /// <inheritdoc cref="IsPositive(int, SignBehavior)"/>
    public static bool IsPositive(this long number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsPositive(behavior);

    /// <inheritdoc cref="IsPositive(int, SignBehavior)"/>
    public static bool IsPositive(this float number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsPositive(behavior);

    /// <inheritdoc cref="IsPositive(int, SignBehavior)"/>
    public static bool IsPositive(this double number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsPositive(behavior);

    /// <inheritdoc cref="IsPositive(int, SignBehavior)"/>
    public static bool IsPositive(this decimal number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsPositive(behavior);
    #endregion

    #region Negative
    /// <summary>
    /// Returns true if <paramref name="number"/> is less than 0.
    /// </summary>
    /// <inheritdoc cref="Sign(int, SignBehavior)"/>
    /// <returns>True if <paramref name="number"/> is less than 0.</returns>
    public static bool IsNegative(this int number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        behavior switch
        {
            SignBehavior.ZeroIsFalse => number < 0,
            SignBehavior.ZeroIsPositive => number < 0,
            SignBehavior.ZeroIsTrue => number <= 0,
            SignBehavior.ZeroIsNegative => number <= 0,
            _ => throw new NotImplementedException(),
        };

    /// <inheritdoc cref="IsNegative(int, SignBehavior)"/>
    public static bool IsNegative(this byte number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsNegative(behavior);

    /// <inheritdoc cref="IsNegative(int, SignBehavior)"/>
    public static bool IsNegative(this short number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsNegative(behavior);

    /// <inheritdoc cref="IsNegative(int, SignBehavior)"/>
    public static bool IsNegative(this long number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsNegative(behavior);

    /// <inheritdoc cref="IsNegative(int, SignBehavior)"/>
    public static bool IsNegative(this float number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsNegative(behavior);

    /// <inheritdoc cref="IsNegative(int, SignBehavior)"/>
    public static bool IsNegative(this double number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsNegative(behavior);

    /// <inheritdoc cref="IsNegative(int, SignBehavior)"/>
    public static bool IsNegative(this decimal number,
        SignBehavior behavior = SignBehavior.ZeroIsPositive) =>
        number.CompareTo(0).IsNegative(behavior);
    #endregion

    #region Sign
    /// <summary>
    /// Returns the sign of number.
    /// </summary>
    /// <param name="number">The sign of the number.</param>
    /// <param name="behavior">The behavior of the method.</param>
    /// <returns>-1, 0, or 1, depending on the value of <paramref
    /// name="behavior"/>.</returns>
    public static int Sign(this int number,
        SignBehavior behavior = SignBehavior.ZeroIsTrue) =>
        behavior switch
        {
            SignBehavior.ZeroIsFalse or SignBehavior.ZeroIsTrue =>
                (number == 0) ? 0 : (number < 0 ? -1 : 1),
            SignBehavior.ZeroIsNegative => number <= 0 ? -1 : 1,
            SignBehavior.ZeroIsPositive => number >= 0 ? 1 : -1,
            _ => throw new NotImplementedException(),
        };

    public static int Sign(this byte number,
        SignBehavior behavior = SignBehavior.ZeroIsTrue) =>
        number.CompareTo(0).Sign(behavior);

    public static int Sign(this short number,
        SignBehavior behavior = SignBehavior.ZeroIsTrue) =>
        number.CompareTo(0).Sign(behavior);

    public static int Sign(this long number,
        SignBehavior behavior = SignBehavior.ZeroIsTrue) =>
        number.CompareTo(0).Sign(behavior);

    public static int Sign(this float number,
        SignBehavior behavior = SignBehavior.ZeroIsTrue) =>
        number.CompareTo(0).Sign(behavior);

    public static int Sign(this double number,
        SignBehavior behavior = SignBehavior.ZeroIsTrue) =>
        number.CompareTo(0).Sign(behavior);

    public static int Sign(this decimal number,
        SignBehavior behavior = SignBehavior.ZeroIsTrue) =>
        number.CompareTo(0).Sign(behavior);
    #endregion
    #endregion

    #region Deltas
    /// <summary>
    /// Returns value such that the change of value is towards target and is no
    /// greater than margin;
    /// </summary>
    /// <param name="value">The value to change.</param>
    /// <param name="target">The number to change towards.</param>
    /// <param name="margin">The maximal change.</param>
    /// <returns></returns>
    public static float GetMinimumDelta(this float value, float target,
        float margin)
    {
        return value + Mathf.Sign(target)
            * Mathf.Min(Mathf.Abs(target), Mathf.Abs(margin));
    }
    #endregion

    #region Rounding
    #region Enum
    public enum RoundMode
    {
        /// <summary>
        /// Rounds to the nearest int.
        /// </summary>
        NearestInt,
        /// <summary>
        /// Rounds to the nearest int greater than the value.
        /// </summary>
        Ceiling,
        /// <summary>
        /// Rounds to the nearest int lesser than the value.
        /// </summary>
        Floor,
        /// <summary>
        /// If value is positive, round to the nearest int greater than value.
        /// Else, round to the nearest int lesser than value.
        /// </summary>
        IncreaseAbs,
        /// <summary>
        /// If value is positive, round to the nearest int lesser than value.
        /// Else, round to the nearest int greater than value.
        /// </summary>
        DecreaseAbs
    }
    #endregion

    /// <summary>
    /// Rounds the float using the provided rounding method.
    /// </summary>
    /// <param name="number">The float to round.</param>
    /// <param name="mode">How to round <paramref name="number"/>.</param>
    /// <returns></returns>
    public static int RoundToInt(this float number,
        RoundMode mode = RoundMode.NearestInt)
    {
        switch (mode)
        {
            case RoundMode.NearestInt:
                return Mathf.RoundToInt(number);
            case RoundMode.Ceiling:
                return Mathf.CeilToInt(number);
            case RoundMode.Floor:
                return Mathf.FloorToInt(number);
            case RoundMode.IncreaseAbs:
                if (number < 0)
                    return Mathf.FloorToInt(number);
                else if (number > 0)
                    return Mathf.CeilToInt(number);
                else
                    return 0;
            case RoundMode.DecreaseAbs:
            default:
                if (number > 0)
                    return Mathf.FloorToInt(number);
                else if (number < 0)
                    return Mathf.CeilToInt(number);
                else
                    return 0;
        }
    }

    /// <summary>
    /// Alias for <see cref="Mathf.Round(float)"/>.
    /// </summary>
    /// <param name="number">Number to round.</param>
    /// <param name="digits">Places after zero to round to.</param>
    /// <returns>The rounded value.</returns>
    public static float Round(this float number, int digits = 0) =>
        (float)Math.Round(number, digits);

    /// <summary>
    /// Alias for <see cref="Math.Round(decimal, int)"/>
    /// </summary>
    /// <inheritdoc cref="Round(float, int)"/>
    public static decimal Round(this decimal number, int digits = 0) =>
        Math.Round(number, digits);    
    #endregion

    #region Misc Operations
    /// <summary>
    /// Returns the square of value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float Squared(this float value)
    {
        return value * value;
    }

    /// <summary>
    /// Returns the square of value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int Squared(this int value)
    {
        return value * value;
    }
    #endregion
}

#region Enums
/// <summary>
/// How zero is handled by sign-determining functions (ie <see
/// cref="IsPositive"/>, <see cref="IsNegative"/>, <see cref="Sign"/>, etc).
/// </summary>
public enum SignBehavior
{
    /// <summary>
    /// If number is zero, a boolean function will return false, and an
    /// integer function will return zero.
    /// </summary>
    ZeroIsFalse,

    /// <summary>
    /// If number is zero, a boolean function will return true, and an
    /// integer function will return zero.
    /// </summary>
    ZeroIsTrue,

    /// <summary>
    /// If number is zero, this function will treat the number as positive.
    /// </summary>
    ZeroIsPositive,

    /// <summary>
    /// If number is zero, this function will treat the number as negative.
    /// </summary>
    ZeroIsNegative
}
#endregion
