using System;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Represents an angle with greater control than a float.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
[JsonObject(MemberSerialization.OptIn)]
[Serializable]
public class FloatAngle : IEquatable<FloatAngle>
{
    #region Constants
    public const float PI = Mathf.PI;

    public const float PI2 = Mathf.PI * 2;

    public const float PI1_2 = PI / 2f;

    public const float PI3_2 = PI2 + PI1_2;
    #endregion

    #region Enums
    /// <summary>
    /// Defines the possible units that this angle can be represented as
    /// </summary>
    public enum Units
    {
        Degrees, Radians
    }

    /// <summary>
    /// Represents the quadrant this angle is in
    /// </summary>
    public enum Quadrant
    {
        /// <summary>
        /// When value is 0 degrees.
        /// </summary>
        Right = 0,
        Q1 = 1, UpperRight = 1,
        /// <summary>
        /// When value is 90 degrees.
        /// </summary>
        Up = 2,
        Q2 = 3, UpperLeft = 3,
        /// <summary>
        /// When value is 180 degrees.
        /// </summary>
        Left = 4,
        Q3 = 5, LowerLeft = 5,
        /// <summary>
        /// When value is 270 degrees.
        /// </summary>
        Down = 6,
        Q4 = 7, LowerRight = 7
    }
    #endregion

    #region Fields
    /// <summary>
    /// Current unit.
    /// </summary>
    [JsonProperty]
    [SerializeField]
    private Units unit;

    /// <summary>
    /// The internal value of the angle
    /// </summary>
    [JsonProperty]
    [SerializeReference]
    private float value;
    #endregion

    #region Properties
    /// <inheritdoc cref="unit"/>
    public Units Unit
    {
        get => unit;
        set
        {
            if (unit != value)
            {
                this.value *= GetConversionFactor(unit);
                unit = value;
            }
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Create a new angle in degrees or radians.
    /// </summary>
    /// <param name="value">Value of the angle</param>
    /// <param name="unit">The units of the angle.</param>
    public FloatAngle(float value, Units unit = Units.Degrees)
    {
        this.value = value;
        this.unit = unit;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Returns a float representation of this FloatAngle. Ranges from
    /// [0,360] for degrees and [0,2pi] for radians.
    /// </summary>
    public float AsPositive()
    {
        return Unit switch
        {
            Units.Degrees => value.AsPositiveDegrees(),
            _ => value.AsPositiveRadians()
        };
    }

    /// <summary>
    /// Returns a float representation of this FloatAngle. Ranges from
    /// [-360,0] for degrees and [-2pi,0] for radians.
    /// </summary>
    public float AsNegative()
    {
        return Unit switch
        {
            Units.Degrees => value.AsNegativeDegrees(),
            _ => value.AsNegativeRadians()
        };
    }

    /// <summary>
    /// Returns a float representation of this FloatAngle. Ranges from
    /// [-180,180] for degrees and [-pi,pi] for radians.
    /// </summary>
    public float AsCentered()
    {
        return Unit switch
        {
            Units.Degrees => value.AsPlusMinus180(),
            _ => value.AsPlusMinusPi()
        };
    }

    /// <summary>
    /// Gets the direction to other. +1 for counterclockwise, -1 for clockwise,
    /// 0 for if <paramref name="other"/> matches <see cref="this"/>.
    /// </summary>
    /// <param name="other">The other angle you are comparing.</param>
    public int GetDirectionTo(FloatAngle other,
        SignBehavior signBehavior)
    {
        return (this - other).AsCentered().Sign(signBehavior);
    }

    /// <summary>
    /// Returns the conversion factor to use to convert this angle's units to
    /// <paramref name="convertTo"/>.
    /// </summary>
    /// <param name="convertTo">The unit to convert this angle to.</param>
    /// <returns></returns>
    public float GetConversionFactor(Units convertTo)
    {
        return Unit.GetConversionFactor(convertTo);
    }

    /// <summary>
    /// Returns the conversion factor to use to convert this angle's units to
    /// the units of <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other angle.</param>
    public float GetConversionFactor(FloatAngle other) =>
        GetConversionFactor(other.Unit);

    /// <summary>
    /// Returns the current quadrant this angle is in.
    /// </summary>
    /// <returns></returns>
    public Quadrant GetQuadrant()
    {
        float val = AsPositive();

        if (Unit == Units.Degrees)
        {
            if (val.Approx(0) || val.Approx(360))
                return Quadrant.Right;
            else if (val < 90)
                return Quadrant.Q1;
            else if (val.Approx(90))
                return Quadrant.Up;
            else if (val < 180)
                return Quadrant.Q2;
            else if (val.Approx(180))
                return Quadrant.Left;
            else if (val < 270)
                return Quadrant.Q3;
            else if (val.Approx(270))
                return Quadrant.Down;
            else
                return Quadrant.Q4;
        }
        else
        {
            if (val.Approx(0) || val.Approx(PI2))
                return Quadrant.Right;
            else if (val < PI1_2)
                return Quadrant.Q1;
            else if (val.Approx(PI1_2))
                return Quadrant.Up;
            else if (val < PI)
                return Quadrant.Q2;
            else if (val.Approx(PI))
                return Quadrant.Left;
            else if (val < PI3_2)
                return Quadrant.Q3;
            else if (val.Approx(PI3_2))
                return Quadrant.Down;
            else
                return Quadrant.Q4;
        }
    }

    /// <summary>
    /// Returns true if this angle lies between right and left. The check starts
    /// at <paramref name="right"/>, then moves counterclockwise until it
    /// reaches <paramref name="left"/>.
    /// </summary>
    /// <param name="right">The minimum angle.</param>
    /// <param name="left">The maximal angle.</param>
    /// <returns></returns>
    public bool IsBetween(FloatAngle right, FloatAngle left)
    {
        float val = AsPositive();

        float r = right.AsPositive();
        float l = left.AsPositive();

        if (l < r)
        {
            // If l is less than r, then we need to offset the two other values.
            float incr = 360f.ConvertToUnit(unit, Units.Degrees);
            val += incr;
            l += incr;
        }

        return r <= val && val <= l;
    }

    #region Operators
    private float ConvertVal(FloatAngle other)
    {
        return Unit == other.Unit ?
            other.value :
            other.value * other.GetConversionFactor(Unit);
    }

    /// <summary>
    /// Adds 2 angles.
    /// </summary>
    /// <remarks>
    /// If both angles have the same units, then preserve both units. Otherwise,
    /// <paramref name="b"/> will be converted to <paramref name="a"/>'s units.
    /// </remarks>
    /// <param name="a">First angle</param>
    /// <param name="b">Second angle</param>
    public static FloatAngle operator +(FloatAngle a, FloatAngle b) =>
        new(a.value + a.ConvertVal(b), a.Unit);

    /// <summary>
    /// Subtracts 2 angles.
    /// </summary>
    /// <inheritdoc cref="operator +(FloatAngle, FloatAngle)"/>
    public static FloatAngle operator -(FloatAngle a, FloatAngle b) =>
        new(a.value - a.ConvertVal(b), a.Unit);

    /// <summary>
    /// Multiplies 2 angles.
    /// </summary>
    /// <inheritdoc cref="operator +(FloatAngle, FloatAngle)"/>
    public static FloatAngle operator *(FloatAngle a, FloatAngle b) =>
        new(a.value * a.ConvertVal(b), a.Unit);

    /// <summary>
    /// Divide 2 angles.
    /// </summary>
    /// <inheritdoc cref="operator +(FloatAngle, FloatAngle)"/>
    public static FloatAngle operator /(FloatAngle a, FloatAngle b) =>
        new(a.value / a.ConvertVal(b), a.Unit);

    /// <summary>
    /// Multiplies a FloatAngle with a float, resulting in a new FloatAngle b
    /// times the size. Units do not matter.
    /// </summary>
    /// <param name="a">The FloatAngle</param>
    /// <param name="b">The float to multiply a by</param>
    /// <returns>The new FloatAngle, units are preserved.</returns>
    public static FloatAngle operator *(FloatAngle a, float b)
    {
        return new FloatAngle(a.value * b, a.Unit);
    }

    /// <summary>
    /// Divides a FloatAngle by a float, resulting in a new FloatAngle 1/b times
    /// the size. Units do not matter.
    /// </summary>
    /// <param name="a">The FloatAngle</param>
    /// <param name="b">The float to divide a by</param>
    /// <returns>The new FloatAngle, units are preserved.</returns>
    public static FloatAngle operator /(FloatAngle a, float b)
    {
        return new FloatAngle(a.value / b, a.Unit);
    }
    #endregion

    public override bool Equals(object obj) =>
        obj is FloatAngle angle && Equals(angle);

    public bool Equals(FloatAngle other) =>
        value.Approx(other.ConvertVal(this));

    public override int GetHashCode() =>
        HashCode.Combine(value, Unit);

    public override string ToString() =>
        $"({base.ToString()}) {value} {Unit}";
    #endregion
}
