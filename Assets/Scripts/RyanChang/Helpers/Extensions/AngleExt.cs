using System;
using UnityEngine;

/// <summary>
/// Contains methods pertaining to angles represented as simple floats.
/// </summary>
public static class AngleExt
{
    #region Angle Representation Conversion
    /// <summary>
    /// Returns an angle between [-360, 360] degrees
    /// </summary>
    /// <param name="theta">The angle to consider</param>
    /// <returns>An angle between [-360, 360] degrees</returns>
    public static float AsPlusMinus360(this float theta)
    {
        while (theta < -360)
            theta += 360;

        while (theta > 360)
            theta -= 360;

        return theta;
    }

    /// <summary>
    /// Returns an angle between [0, 360) degrees
    /// </summary>
    /// <param name="theta">The angle to consider</param>
    /// <returns>An angle between [0, 360) degrees</returns>
    public static float AsPositiveDegrees(this float theta)
    {
        while (theta < 0)
            theta += 360;

        while (theta > 360)
            theta -= 360;

        return theta;
    }

    /// <summary>
    /// Returns an angle between [0, 2pi) radians
    /// </summary>
    /// <param name="theta">The angle to consider</param>
    /// <returns>An angle between [0, 2pi) radians</returns>
    public static float AsPositiveRadians(this float theta)
    {
        while (theta < 0)
            theta += FloatAngle.PI2;

        while (theta > FloatAngle.PI2)
            theta -= FloatAngle.PI2;

        return theta;
    }

    /// <summary>
    /// Returns an angle between (-360, 0] degrees
    /// </summary>
    /// <param name="theta">The angle to consider</param>
    /// <returns>An angle between (-360, 0] degrees</returns>
    public static float AsNegativeDegrees(this float theta)
    {
        theta = theta.AsPositiveDegrees();

        if (theta == 0)
            return 0;
        else
            return theta - 360;
    }

    /// <summary>
    /// Returns an angle between (-2pi, 0] degrees
    /// </summary>
    /// <param name="theta">The angle to consider</param>
    /// <returns>An angle between (-2pi, 0] degrees</returns>
    public static float AsNegativeRadians(this float theta)
    {
        theta = theta.AsPositiveRadians();

        if (theta == 0)
            return 0;
        else
            return theta - FloatAngle.PI2;
    }

    /// <summary>
    /// Returns an angle between [-180, 180] degrees
    /// </summary>
    /// <param name="theta"></param>
    /// <returns>An angle between [-180, 180] degrees</returns>
    public static float AsPlusMinus180(this float theta)
    {
        while (theta < -180)
            theta += 360;

        while (theta > 180)
            theta -= 360;

        return theta;
    }

    /// <summary>
    /// Returns an angle between [-pi, pi] radians
    /// </summary>
    /// <param name="theta"></param>
    /// <returns>An angle between [-pi, pi] radians</returns>
    public static float AsPlusMinusPi(this float theta)
    {
        while (theta < -FloatAngle.PI)
            theta += FloatAngle.PI2;

        while (theta > FloatAngle.PI)
            theta -= FloatAngle.PI2;

        return theta;
    }
    #endregion

    #region Comparisons
    /// <summary>
    /// Returns true if both angles represents the same angle
    /// </summary>
    /// <param name="angle1"></param>
    /// <param name="angle2"></param>
    /// <returns></returns>
    public static bool AngleEqual(this float angle1, float angle2)
    {
        return Mathf.DeltaAngle(angle1, angle2) == 0;
    }

    /// <summary>
    /// Returns true if angle is between theta1 and theta2, that is, angle lies
    /// in the smallest arc formed by theta1 and theta2.
    /// </summary>
    /// <param name="angle">The angle to evaluate</param>
    /// <param name="theta1"></param>
    /// <param name="theta2"></param>
    /// <returns>True if <paramref name="angle"/> lies between the smallest arc
    /// formed by theta1 and theta2..</returns>
    public static bool AngleIsBetween(this float angle, float theta1, float theta2)
    {
        float min = Mathf.Min(theta1, theta2);
        float max = Mathf.Max(theta1, theta2);

        //Debug.Log($"{min} {max} {angle}");
        //Debug.Log($"min.GetDeltaTheta(angle): {min.GetDeltaTheta(angle)}");
        //Debug.Log($"max.GetDeltaTheta(angle): {max.GetDeltaTheta(angle)}");

        return min.GetDeltaTheta(angle) >= 0 && max.GetDeltaTheta(angle) <= 0;
    }
    #endregion

    #region Alterations
    /// <summary>
    /// Returns angle if it falls within the smallest arc formed by theta1 and theta2.
    /// Else, returns either theta1 or theta2.
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="theta1"></param>
    /// <param name="theta2"></param>
    /// <returns></returns>
    public static float ClampAngle(this float angle, float theta1, float theta2)
    {
        float min = Mathf.Min(theta1, theta2);
        float max = Mathf.Max(theta1, theta2);

        if (max - min == 180)
        {
            throw new ArgumentException($"Unable to clamp angle ({angle}) as theta 1 ({theta1}) and theta2 ({theta2}) " +
                $"differ by 180 degrees. There are two locations to clamp to.");
        }

        switch (min.GetDeltaTheta(angle))
        {
            case < 0:
                return min;
            case > 0:
                return max;
            default:
                return angle;
        }
    }
    #endregion

    #region Delta Theta
    /// <summary>
    /// Gets the direction of travel from actualTheta to targetTheta.
    /// If return value is -1, turn clockwise.
    /// If return value is 1, turn counterclockwise.
    /// </summary>
    /// <param name="actualTheta"></param>
    /// <param name="targetTheta"></param>
    /// <returns></returns>
    public static int DirectionToAngle(this float actualTheta, float targetTheta)
    {
        actualTheta = actualTheta.AsPositiveDegrees();
        targetTheta = targetTheta.AsPositiveDegrees();

        if (actualTheta.Approx(targetTheta))
            return 0;
        else
            return (Mathf.DeltaAngle(actualTheta, targetTheta) < 0) ? 1 : -1;
    }

    /// <summary>
    /// Gets the direction of travel from actualTheta to targetTheta.
    /// If return value is -1, turn clockwise.
    /// If return value is 1, turn counterclockwise.
    /// </summary>
    /// <param name="actualTheta"></param>
    /// <param name="targetTheta"></param>
    /// <returns></returns>
    public static int DirectionToAngle(this float actualTheta, float targetTheta, float margin)
    {
        actualTheta = actualTheta.AsPositiveDegrees();
        targetTheta = targetTheta.AsPositiveDegrees();

        if (actualTheta.Approx(targetTheta, margin))
            return 0;
        else
            return (Mathf.DeltaAngle(actualTheta, targetTheta) < 0) ? 1 : -1;
    }

    /// <summary>
    /// Returns the shortest angle between actualTheta and targetTheta.
    /// </summary>
    /// <param name="actualTheta"></param>
    /// <param name="targetTheta"></param>
    /// <returns></returns>
    public static float GetDeltaTheta(this float actualTheta, float targetTheta)
    {
        return Mathf.DeltaAngle(actualTheta, targetTheta);
    }
    #endregion

    /// <summary>
    /// Returns the conversion factor to convert from the unit that is not
    /// <paramref name="convertFrom"/> to <paramref name="convertTo"/>.
    /// </summary>
    /// <param name="convertFrom">The units to convert from.</param>
    /// <param name="convertTo">The units to convert to.</param>
    /// <returns></returns>
    public static float GetConversionFactor(this FloatAngle.Units convertFrom,
        FloatAngle.Units convertTo)
    {
        if (convertFrom == convertTo)
            return 1f;

        return convertTo switch
        {
            FloatAngle.Units.Degrees => Mathf.Rad2Deg,
            FloatAngle.Units.Radians => Mathf.Deg2Rad,
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Converts <paramref name="angle"/> to the specified units.
    /// </summary>
    /// <param name="angle">The angle.</param>
    /// <returns></returns>
    /// <inheritdoc cref="GetConversionFactor(FloatAngle.Units, FloatAngle.Units)"/>
    public static float ConvertToUnit(this float angle,
        FloatAngle.Units convertFrom, FloatAngle.Units convertTo)
    {
        return angle * convertFrom.GetConversionFactor(convertTo);
    }
}
