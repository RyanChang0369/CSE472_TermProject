using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RNGExt
{
    #region Variables/Properties
    private static System.Random rand = new();

    public static int Seed
    {
        set => rand = new(value);
    }
    #endregion

    #region Boolean
    /// <summary>
    /// Returns a random boolean value.
    /// </summary>
    public static bool RandomBool()
    {
        return rand.NextDouble() >= 0.5;
    }

    /// <inheritdoc cref="RandomBool()"/>
    public static bool NextBool => RandomBool();

    /// <inheritdoc cref="RandomBool()"/>
    public static bool CoinFlip() => RandomBool();

    /// <summary>
    /// Returns true based on percent chance given.
    /// </summary>
    /// <param name="percentChance">A float [0 - 1]</param>
    /// <returns>True based on percent chance given.</returns>
    public static bool PercentChance(float percentChance)
    {
        return rand.NextDouble() < percentChance;
    }
    #endregion

    #region Byte
    /// <summary>
    /// Gets a random byte string.
    /// </summary>
    /// <param name="bytes">How many bytes of RNG to generate?</param>
    /// <returns></returns>
    public static byte[] RandomBytes(int bytes = 16)
    {
        byte[] arr = new byte[bytes];
        rand.NextBytes(arr);
        return arr;
    }

    /// <inheritdoc cref="RandomInt(int, int)"/>
    public static byte RandomByte(byte min, byte max) =>
        (byte)(rand.Next() * (max - min) + min);

    /// <inheritdoc cref="RandomInt(int)"/>
    public static byte RandomByte(byte max) => RandomByte(0, max);

    /// <inheritdoc cref="RandomInt()"/>
    public static byte RandomByte() => RandomByte(byte.MaxValue);

    /// <inheritdoc cref="RandomByte()"/>
    public static byte NextByte => RandomByte();

    /// <inheritdoc cref="RandomBytes(int)"/>
    /// <param name="buffer">The buffer to fill with bytes.</param>
    public static void RandomBytes(byte[] buffer) =>
        rand.NextBytes(buffer);

    /// <inheritdoc cref="RandomBytes(byte[])"/>
    public static void RandomBytes(Span<byte> buffer) =>
        rand.NextBytes(buffer);

    #region Hash String
    [Flags]
    public enum HashStringOptions
    {
        None = 0b0000,
        /// <summary>
        /// Include the dashes within the hash string.
        /// </summary>
        WithDashes = 0b0001,
        /// <summary>
        /// Surround the hash string with brackets "{}".
        /// </summary>
        Bracketed = 0b0010
    }

    /// <inheritdoc cref="RandomBytes(int)"/>
    /// <summary>
    /// Gets a random byte string as a hexadecimal hash.
    /// </summary>
    /// <param name="options">Options for string formatting.</param>
    public static string RandomHashString(int bytes = 16,
        HashStringOptions options = HashStringOptions.None)
    {
        string hash = BitConverter.ToString(RandomBytes(bytes));

        if (options.HasFlag(HashStringOptions.WithDashes))
            hash = hash.Replace("-", "");
        if (options.HasFlag(HashStringOptions.Bracketed))
            hash = '{' + hash + '}';

        return hash;
    }
    #endregion
    #endregion

    #region Integer
    /// <summary>
    /// Returns a int ranging from <paramref name="min"/>, inclusive, to
    /// <paramref name="max"/>, exclusive.
    /// </summary>
    /// <param name="min">The minimal value, inclusive.</param>
    /// <param name="max">The maximal value, exclusive.</param>
    public static int RandomInt(int min, int max) =>
        rand.Next(min, max);

    /// <summary>
    /// Returns a int ranging from 0, inclusive, to <paramref name="max"/>,
    /// exclusive.
    /// </summary>
    /// <inheritdoc cref="RandomInt(int, int)"/>
    public static int RandomInt(int max) => rand.Next(max);

    /// <summary>
    /// Returns a int ranging from 0, inclusive, to the maximum value of the
    /// data type, exclusive.
    /// </summary>
    public static int RandomInt() => rand.Next();

    /// <inheritdoc cref="RandomInt()"/>
    public static int NextInt => RandomInt();

    #region Dice
    /// <summary>
    /// Rolls a 4 sided dice (bounded by [1, 4]).
    /// </summary>
    public static int D4 => NSidedDice(4);

    /// <summary>
    /// Rolls a 6 sided dice (bounded by [1, 6]).
    /// </summary>
    public static int D6 => NSidedDice(6);

    /// <summary>
    /// Rolls a 8 sided dice (bounded by [1, 8]).
    /// </summary>
    public static int D8 => NSidedDice(8);

    /// <summary>
    /// Rolls a 10 sided dice (bounded by [1, 10]).
    /// </summary>
    public static int D10 => NSidedDice(10);

    /// <summary>
    /// Rolls a 12 sided dice (bounded by [1, 12]).
    /// </summary>
    public static int D12 => NSidedDice(12);

    /// <summary>
    /// Rolls a 20 sided dice (bounded by [1, 20]).
    /// </summary>
    public static int D20 => NSidedDice(20);

    /// <summary>
    /// Rolls a dice with the specified number of <paramref name="sides"/>
    /// (bounded by [1, <paramref name="sides"/>]).
    /// </summary>
    /// <param name="sides">Number of sides of the dice.</param>
    public static int NSidedDice(int sides) => RandomInt(sides) + 1;
    #endregion
    #endregion

    #region Long
    /// <inheritdoc cref="RandomInt(int, int)"/>
    public static long RandomLong(long min, long max) =>
        rand.Next() * (max - min) + min;

    /// <inheritdoc cref="RandomInt(int)"/>
    public static long RandomLong(long max) => RandomLong(0, max);

    /// <inheritdoc cref="RandomInt()"/>
    public static long RandomLong() => RandomLong(long.MaxValue);

    /// <inheritdoc cref="RandomLong()"/>
    public static long NextLong => RandomLong();
    #endregion

    #region Float
    /// <summary>
    /// Returns a value ranging from <paramref name="min"/>, inclusive, to
    /// <paramref name="max"/>, exclusive.
    /// </summary>
    public static float RandomFloat(float min, float max) =>
        ((float)rand.NextDouble() * (max - min)) + min;

    /// <summary>
    /// Returns a value ranging from 0, inclusive, to <paramref name="max"/>,
    /// exclusive.
    /// </summary>
    public static float RandomFloat(float max) => RandomFloat(0, max);

    /// <summary>
    /// Returns a value ranging from 0, inclusive, to 1, exclusive.
    /// </summary>
    public static float RandomFloat() => (float)rand.NextDouble();

    /// <inheritdoc cref="RandomFloat()"/>
    public static float NextFloat => RandomFloat();

    /// <summary>
    /// Returns a value ranging from 0, exclusive, to 1, exclusive.
    /// </summary>
    public static float NextExclusiveFloat
    {
        get
        {
            float r;
            do
            {
                r = NextFloat;
                return r;
            } while (r == 0);
        }
    }

    /// <summary>
    /// Returns a value ranging from 0, exclusive, to 1, inclusive.
    /// </summary>
    public static float NextFloatInclusive => 1f - NextFloat;

    #region Angle
    /// <summary>
    /// Returns a random angle.
    /// </summary>
    /// <param name="unit">The unit of the angle.</param>
    public static float RandomAngle(FloatAngle.Units unit) => unit switch
    {
        FloatAngle.Units.Degrees => RandomFloat(0, 360),
        FloatAngle.Units.Radians => RandomFloat(0, FloatAngle.PI2),
        _ => throw new NotImplementedException()
    };
    #endregion

    #region Standard Deviation
    /// <summary>
    /// Uses the Box-Muller Transform to estimate a normally-distributed random
    /// value.
    /// </summary>
    /// <param name="mean">The mean (center) of the normal distribution.</param>
    /// <param name="deviation">The standard deviation of the normal
    /// distribution.</param>
    /// <remarks>
    /// Source: https://w.wiki/BkRg
    /// </remarks>
    public static Tuple<float, float> BoxMuller(float mean, float deviation)
    {
        float r = Mathf.Sqrt(-2 * Mathf.Log(NextExclusiveFloat));
        float t = FloatAngle.PI2 * NextExclusiveFloat;
        return new(
            r * Mathf.Cos(t) * deviation + mean,
            r * Mathf.Sin(t) * deviation + mean
        );
    }

    /// <summary>
    /// Uses the Box-Muller Transform to estimate a normally-distributed random
    /// value. Discards one of the generated random values.
    /// </summary>
    /// <inheritdoc cref="BoxMuller(float, float)"/>
    public static float BoxMullerSingular(float mean, float deviation)
    {
        float r = Mathf.Sqrt(-2 * Mathf.Log(NextExclusiveFloat));
        float t = FloatAngle.PI2 * NextExclusiveFloat;
        return r * Mathf.Cos(t) * deviation + mean;
    }
    #endregion
    #endregion

    #region Double
    /// <inheritdoc cref="RandomFloat(float, float)"/>
    public static double RandomDouble(double min, double max) =>
        (rand.NextDouble() * (max - min)) + min;

    /// <inheritdoc cref="RandomFloat(float)"/>
    public static double RandomDouble(double max) => RandomDouble(0, max);

    /// <inheritdoc cref="RandomFloat()"/>
    public static double RandomDouble() => rand.NextDouble();

    /// <inheritdoc cref="RandomDouble()"/>
    public static double NextDouble => RandomDouble();
    #endregion

    #region Decimal
    /// <inheritdoc cref="RandomFloat(float, float)"/>
    public static decimal RandomDecimal(decimal min, decimal max) =>
        (decimal)rand.NextDouble() * (max - min) + min;

    /// <inheritdoc cref="RandomFloat(float)"/>
    public static decimal RandomDecimal(decimal max) =>
        RandomDecimal(0, max);

    /// <inheritdoc cref="RandomFloat()"/>
    public static decimal RandomDecimal() => RandomDecimal(0, 1);

    /// <inheritdoc cref="RandomDecimal()"/>
    public static decimal NextDecimal => RandomDecimal();
    #endregion

    #region Vector2
    /// <summary>
    /// Returns a Vector2 with all components ranging from -<paramref
    /// name="val"/> (inclusive) to <paramref name="val"/> (exclusive).
    /// </summary>
    /// <param name="val">The bounds of the Vector2.</param>
    public static Vector2 RandomVector2(float val)
    {
        return RandomVector2(-val, val);
    }

    /// <summary>
    /// Returns a Vector2 with both components ranging from <paramref
    /// name="min"/> to <paramref name="max"/>.
    /// </summary>
    /// <param name="min">Minimum value of the x and y components,
    /// inclusive.</param>
    /// <param name="max">Maximum value of the x and y components,
    /// exclusive.</param>
    /// <returns>A random Vector2.</returns>
    public static Vector2 RandomVector2(float min, float max)
    {
        return RandomVector2(min, min, max, max);
    }

    /// <summary>
    /// Returns a random Vector2 with components ranging between the respective
    /// components of <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="min">The lower bound of the random Vector2,
    /// inclusive.</param>
    /// <param name="max">The upper bound of the random Vector2,
    /// exclusive.</param>
    /// <inheritdoc cref="RandomVector2(float, float, float, float)"/>
    public static Vector2 RandomVector2(Vector2 min, Vector2 max)
    {
        return RandomVector2(min.x, min.y, max.x, max.y);
    }

    /// <summary>
    /// Returns a random Vector2 with the x component ranging from <paramref
    /// name="minX"/> to <paramref name="maxX"/> and the y component ranging
    /// from <paramref name="minY"/> to <paramref name="maxY"/>.
    /// </summary>
    /// <param name="minX">Minimum value of the x component, inclusive.</param>
    /// <param name="minY">Minimum value of the y component, inclusive.</param>
    /// <param name="maxX">Maximum value of the x component, exclusive.</param>
    /// <param name="maxY">Maximum value of the y component, exclusive.</param>
    /// <returns>A random Vector2.</returns>
    public static Vector2 RandomVector2(float minX, float minY, float maxX,
        float maxY)
    {
        return new Vector2(RandomFloat(minX, maxX), RandomFloat(minY, maxY));
    }
    #endregion

    #region Vector3
    /// <summary>
    /// Returns a Vector3 with all components as random values (determined by
    /// <see cref="RandomFloat()"/>).
    /// </summary>
    /// <returns>A random Vector3.</returns>
    public static Vector3 RandomVector3()
    {
        return new(RandomFloat(), RandomFloat(), RandomFloat());
    }

    /// <summary>
    /// Returns a Vector3 with all components ranging from -val (inclusive) to
    /// val (exclusive).
    /// </summary>
    /// <param name="val">The bounds of the Vector3.</param>
    /// <returns>A random Vector3.</returns>
    public static Vector3 RandomVector3(float val)
    {
        return RandomVector3(-val, val);
    }

    /// <summary>
    /// Returns a Vector3 with all components ranging between min and max.
    /// </summary>
    /// <param name="min">
    /// Minimum value of the x, y, and z components, inclusive.
    /// </param>
    /// <param name="max">
    /// Maximum value of the x, y, and z components, exclusive.
    /// </param>
    /// <returns>A random Vector3.</returns>
    public static Vector3 RandomVector3(float min, float max)
    {
        return RandomVector3(min, min, min, max, max, max);
    }

    /// <summary>
    /// Returns a random Vector3 with components ranging between the respective
    /// components of min and max.
    /// </summary>
    /// <param name="min">
    /// The lower bound of the random Vector3, inclusive.
    /// </param>
    /// <param name="max">
    /// The upper bound of the random Vector3, exclusive.
    /// </param>
    /// <returns>A random Vector3.</returns>
    public static Vector3 RandomVector3(Vector3 min, Vector3 max)
    {
        return RandomVector3(min.x, min.y, min.z, max.x, max.y, max.z);
    }

    /// <summary>
    /// Returns a random Vector3 with the x component ranging from minX to maxX,
    /// the y component ranging from minY to maxY, and the z component ranging
    /// from minZ to maxZ.
    /// </summary>
    /// <param name="minX">Minimum value of the x component, inclusive.</param>
    /// <param name="minY">Minimum value of the y component, inclusive.</param>
    /// <param name="minZ">Minimum value of the z component, inclusive.</param>
    /// <param name="maxX">Maximum value of the x component, exclusive.</param>
    /// <param name="maxY">Maximum value of the y component, exclusive.</param>
    /// <param name="maxZ">Maximum value of the z component, exclusive.</param>
    /// <returns>A random Vector3.</returns>
    public static Vector3 RandomVector3(float minX, float minY,
        float minZ, float maxX, float maxY, float maxZ)
    {
        return new Vector3(
            RandomFloat(minX, maxX),
            RandomFloat(minY, maxY),
            RandomFloat(minZ, maxZ)
            );
    }
    #endregion

    #region Vector4
    /// <summary>
    /// Returns a Vector4 with all components as random values (determined by
    /// <see cref="RandomFloat()"/>).
    /// </summary>
    /// <returns>A random Vector4.</returns>
    public static Vector4 RandomVector4()
    {
        return new(RandomFloat(), RandomFloat(), RandomFloat(), RandomFloat());
    }

    /// <summary>
    /// Returns a Vector4 with all components ranging from -val (inclusive) to
    /// val (exclusive).
    /// </summary>
    /// <param name="val">The bounds of the Vector4.</param>
    /// <returns>A random Vector4.</returns>
    public static Vector4 RandomVector4(float val)
    {
        return RandomVector4(-val, val);
    }

    /// <summary>
    /// Returns a Vector4 with all components ranging between min and max.
    /// </summary>
    /// <param name="min">
    /// Minimum value of the x, y, z, and w components, inclusive.
    /// </param>
    /// <param name="max">
    /// Maximum value of the x, y, z, and w components, exclusive.
    /// </param>
    /// <returns>A random Vector4.</returns>
    public static Vector4 RandomVector4(float min, float max)
    {
        return RandomVector4(min, min, min, min, max, max, max, max);
    }

    /// <summary>
    /// Returns a random Vector4 with components ranging between the respective
    /// components of min and max.
    /// </summary>
    /// <param name="min">The lower bound of the random Vector4,
    /// inclusive.</param>
    /// <param name="max">The upper bound of the random Vector4,
    /// exclusive.</param>
    /// <returns>A random Vector4.</returns>
    public static Vector4 RandomVector4(Vector4 min, Vector4 max)
    {
        return RandomVector4(min.x, min.y, min.z, min.w,
            max.x, max.y, max.z, max.w);
    }

    /// <summary>
    /// Returns a random Vector4 with the x component ranging from <paramref
    /// name="minX"/> to <paramref name="maxX"/>, the y component ranging from
    /// <paramref name="minY"/> to <paramref name="maxY"/>, the z component
    /// ranging from <paramref name="minZ"/> to <paramref name="maxZ"/>, and the
    /// w component ranging from <paramref name="minW"/> to <paramref
    /// name="maxW"/>.
    /// </summary>
    /// <param name="minX">Minimum value of the x component, inclusive.</param>
    /// <param name="minY">Minimum value of the y component, inclusive.</param>
    /// <param name="minZ">Minimum value of the z component, inclusive.</param>
    /// <param name="minW">Minimum value of the w component, inclusive.</param>
    /// <param name="maxX">Maximum value of the x component, exclusive.</param>
    /// <param name="maxY">Maximum value of the y component, exclusive.</param>
    /// <param name="maxZ">Maximum value of the z component, exclusive.</param>
    /// <param name="maxW">Maximum value of the w component, exclusive.</param>
    /// <returns>A random Vector4.</returns>
    public static Vector4 RandomVector4(float minX, float minY,
        float minZ, float minW, float maxX, float maxY, float maxZ, float maxW)
    {
        return new Vector4(
            RandomFloat(minX, maxX),
            RandomFloat(minY, maxY),
            RandomFloat(minZ, maxZ),
            RandomFloat(minW, maxW)
            );
    }
    #endregion

    #region Rectangle
    #region Point Within
    /// <summary>
    /// Returns a random Vector2 with its respective components ranging from
    /// rect.min (inclusive) to rect.max (exclusive).
    /// </summary>
    /// <param name="rect">The bounds.</param>
    /// <returns></returns>
    public static Vector2 WithinRect(Rect rect) =>
        RandomVector2(rect.min.x, rect.min.y, rect.max.x, rect.max.y);
    #endregion

    #region Point On
    /// <summary>
    /// Gets a point on the perimeter of a rectangle.
    /// </summary>
    /// <param name="rect">The rectangle in question.</param>
    /// <returns>A point on the rectangle.</returns>
    public static Vector2 OnRect(Rect rect) => rect.AlongEdge(NextFloat);
    #endregion

    #region Value
    /// <summary>
    /// Generates a rectangle with random components.
    /// </summary>
    /// <param name="minX">Minimum value of the x component, inclusive.</param>
    /// <param name="minY">Minimum value of the y component, inclusive.</param>
    /// <param name="minWidth">Minimum value of the width component,
    /// inclusive.</param>
    /// <param name="minHeight">Minimum value of the height component,
    /// inclusive.</param>
    /// <param name="maxX">Maximum value of the x component, exclusive.</param>
    /// <param name="maxY">Maximum value of the y component, exclusive.</param>
    /// <param name="maxWidth">Maximum value of the width component,
    /// exclusive.</param>
    /// <param name="maxHeight">Maximum value of the height component,
    /// exclusive.</param>
    /// <returns>The random rectangle.</returns>
    public static Rect RandomRect(
        float minX, float minY, float minWidth, float minHeight,
        float maxX, float maxY, float maxWidth, float maxHeight) =>
        new(
            RandomFloat(minX, maxX),
            RandomFloat(minY, maxY),
            RandomFloat(minWidth, maxWidth),
            RandomFloat(minHeight, maxHeight)
        );

    /// <inheritdoc cref="RandomRect(float, float, float, float, float, float,
    /// float, float)"/>
    /// <summary>
    /// Generates a random rectangle that lies between the rectangle bounded by
    /// <paramref name="minimum"/> and <paramref name="maximum"/>.
    /// </summary>
    /// <param name="minimum">The lower bound.</param>
    /// <param name="maximum">The higher bound.</param>
    public static Rect RandomRect(Vector2 minimum, Vector2 maximum)
    {
        var lowerX = Mathf.Min(minimum.x, maximum.x);
        var lowerY = Mathf.Min(minimum.y, maximum.y);
        var higherX = Mathf.Max(minimum.x, maximum.x);
        var higherY = Mathf.Max(minimum.y, maximum.y);

        return RandomRect(
            lowerX, lowerY, 0, 0,
            higherX, higherY, higherX - lowerX, higherY - lowerY
        );
    }

    /// <inheritdoc cref="RandomRect(float, float, float, float, float, float,
    /// float, float)"/>
    /// <summary>
    /// Generates a random rectangle that lies within <paramref name="bound"/>.
    /// </summary>
    /// <param name="bound">The bounds of the random rectangle.</param>
    public static Rect RandomRect(Rect bound) =>
        RandomRect(bound.min, bound.max);
    #endregion
    #endregion

    #region Square
    #region Point On
    /// <summary>
    /// Gets a point on the perimeter of a square, centered on <paramref
    /// name="origin"/>.
    /// </summary>
    /// <param name="origin">The center of the square.</param>
    /// <param name="length">The length of one of the square's sides.</param>
    /// <returns>A point on the square.</returns>
    public static Vector2 OnSquare(Vector2 origin, float length = 1) =>
        RectExt.CenteredAt(origin, length, length).AlongEdge(NextFloat);

    /// <summary>
    /// Gets a point on the perimeter of a square centered at (0, 0).
    /// </summary>
    /// <inheritdoc cref="OnSquare(float, Vector2)"/>
    public static Vector2 OnSquare(float length = 1) =>
        OnSquare(Vector2.zero, length);
    #endregion

    #region Point Within
    /// <summary>
    /// Gets a point within a square centered on <paramref name="origin"/>.
    /// </summary>
    /// <param name="origin">The center of the square.</param>
    /// <param name="length">The length of one of the square's sides.</param>
    /// <returns>A point within the square.</returns>
    public static Vector2 WithinSquare(Vector2 origin, float length = 1) =>
        RandomVector2(length / 2f) + origin;

    /// <summary>
    /// Gets a point within a square centered at (0, 0).
    /// </summary>
    /// <inheritdoc cref="WithinSquare(float)"/>
    public static Vector2 WithinSquare(float length = 1) =>
        WithinSquare(Vector2.zero, length);
    #endregion
    #endregion

    #region Circle
    #region Point On
    /// <summary>
    /// Gets a random point on the perimeter of a circle.
    /// </summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="center">The center of the circle.</param>
    /// <returns>An uniformly distributed point on the circle.</returns>
    public static Vector2 OnCircle(Vector2 center, float radius = 1) =>
        OnCircle(radius) + center;

    /// <inheritdoc cref="OnCircle(Vector2, float)"/>
    public static Vector2 OnCircle(float radius = 1)
    {
        float theta = RandomAngle(FloatAngle.Units.Radians);

        return new(
            radius * Mathf.Cos(theta),
            radius * Mathf.Sin(theta)
        );
    }
    #endregion

    #region Point Within
    /// <summary>
    /// Gets a point within a circle.
    /// </summary>
    /// <returns>An uniformly distributed point within the circle.</returns>
    /// <inheritdoc cref="OnCircle(Vector2, float)"/>
    public static Vector2 WithinCircle(Vector2 center, float radius = 1) =>
        WithinCircle(radius) + center;

    /// <inheritdoc cref="WithinCircle(Vector2, float)"/>
    public static Vector2 WithinCircle(float radius = 1) =>
        OnCircle(radius * Mathf.Sqrt(NextFloat));
    #endregion
    #endregion

    #region Bounds
    /// <summary>
    /// Returns a random Vector3 with its respective components ranging from
    /// bounds.min (inclusive) to bounds.max (exclusive).
    /// </summary>
    /// <param name="bounds">The bounds.</param>
    public static Vector3 RandomWithinBounds(Bounds bounds)
    {
        return RandomVector3(bounds.max, bounds.min);
    }
    #endregion

    #region Cube
    /// <summary>
    /// Returns a random Vector3 that lies on the surface of the cube,
    /// centered at (0,0,0), with the specified <paramref name="length"/>.
    /// </summary>
    /// <param name="length">Length of one of the cube's sides.</param>
    public static Vector3 OnCube(float length = 1)
    {
        var vector = RandomVector3();
        var mag = vector.sqrMagnitude;

        if (mag.Approx(0))
            return Vector3.zero;

        return vector / mag * length;
    }

    /// <summary>
    /// Returns a random Vector3 that lies on the surface of the cube,
    /// centered at (0,0,0), with the specified <paramref name="length"/>.
    /// </summary>
    /// <param name="length">Length of one of the cube's sides.</param>
    public static Vector3 WithinCube(float length = 1) =>
        OnCube(RandomFloat(length));
    #endregion

    #region Sphere
    #region Point On
    /// <summary>
    /// Returns a random Vector3 that lies on the surface of the sphere centered
    /// at (0,0,0), with the specified <paramref name="radius"/>.
    /// </summary>
    /// <inheritdoc cref="OnSphere(Vector3, float)"/>
    public static Vector3 OnSphere(float radius = 1) =>
        RandomVector3().normalized * radius;

    /// <summary>
    /// Returns a random Vector3 that lies on the surface of the sphere centered
    /// at <paramref name="origin"/>, with the specified <paramref
    /// name="radius"/>.
    /// </summary>
    /// <param name="origin">The center of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    public static Vector3 OnSphere(Vector3 origin, float radius = 1) =>
        OnSphere(radius) + origin;
    #endregion
    #endregion

    #region Curve
    /// <summary>
    /// Retrieves a random value on the animation curve, allowing for the biased
    /// selection of random values.
    /// </summary>
    /// <param name="curve">The animation curve.</param>
    /// <param name="floor">The absolute minimal value this method can
    /// return. This is disabled if the value given is not finite.</param>
    /// <param name="ceiling">The absolute maximal value this method can
    /// return. This is disabled if the value given is not finite.</param>
    public static float RandomValueOnCurve(this AnimationCurve curve,
        float floor, float ceiling)
    {
        if (curve == null)
            throw new ArgumentNullException(
                "Value of curve not set."
            );
        else if (curve.keys.IsEmpty())
            throw new ArgumentOutOfRangeException(
                "Curve does not have any keys."
            );

        float minT = curve.keys.First().time;
        float maxT = curve.keys.Last().time;

        if (minT > maxT)
            throw new ArgumentException(
                "Curve is malformed (first key occurs before the last key)"
            );

        float val = curve.Evaluate(RandomFloat(minT, maxT));

        if (float.IsFinite(floor))
            val = Mathf.Max(val, floor);
        if (float.IsFinite(ceiling))
            val = Mathf.Min(val, ceiling);

        return val;
    }

    /// <inheritdoc cref="RandomValueOnCurve(AnimationCurve, float, float)"/>
    public static float RandomValueOnCurve(this AnimationCurve curve) =>
        RandomValueOnCurve(curve, float.NaN, float.NaN);
    #endregion

    #region IEnumerable & Related
    /// <summary>
    /// Returns a random value from the provided enumeration of values.
    /// </summary>
    /// <param name="defaultOK">If true, then return default value if values
    /// length is 0. Else throw an index error.</param>
    public static T GetRandomValue<T>(this IEnumerable<T> values,
        bool defaultOK = true)
    {
        int length = values.Count();

        if (length < 1 && defaultOK)
        {
            return default;
        }

        return values.ElementAt(RandomInt(length));
    }

    /// <inheritdoc cref="GetRandomValue{T}(IEnumerable{T}, bool)"/>
    public static T GetRandomValue<T>(params T[] values) =>
        values.GetRandomValue();

    /// <inheritdoc cref="GetRandomValue{T}(IEnumerable{T}, bool)"/>
    /// <summary>
    /// Returns a random value from the provided enumeration of values. Alias
    /// for <see cref="GetRandomValue{T}(IEnumerable{T}, bool)"/>.
    /// </summary>
    public static T RandomSelectOne<T>(this IEnumerable<T> values,
        bool defaultOK = true)
    {
        return values.GetRandomValue(defaultOK);
    }

    /// <summary>
    /// Selects multiple items from an IEnumerable. Can also select none.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs"></param>
    /// <param name="key">How to get to the percentage [0-1] from any
    /// obj.</param>
    /// <returns></returns>
    public static IEnumerable<T> RandomSelectMany<T>(this IEnumerable<T> objs,
        Func<T, float> key)
    {
        return objs
            .Where(obj => PercentChance(key(obj)));
    }

    /// <summary>
    /// Randomly selects multiple items from an IEnumerable. Guaranteed to
    /// select at least one item.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static IEnumerable<T> RandomSelectAtLeastOne<T>(
        this IEnumerable<T> objs, Func<T, float> key)
    {
        var selected = objs.RandomSelectMany(key);

        if (selected.Count() < 1)
        {
            T[] arr = { objs.GetRandomValue() };
            selected = arr.AsEnumerable();
        }

        return selected;
    }

    /// <summary>
    /// Selects one or no items from an IEnumerable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs"></param>
    /// <param name="key">
    /// How to get to the percentage [0-1] from any obj.
    /// </param>
    /// <returns></returns>
    public static T RandomSelectOneOrNone<T>(this IEnumerable<T> objs,
        Func<T, float> key)
    {
        IEnumerable<T> selected = objs.RandomSelectMany(key);

        if (selected.Count() < 1)
        {
            return objs.GetRandomValue();
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Selects one item from an IEnumerable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objs"></param>
    /// <param name="key">How to get to the percentage [0-1] from any
    /// obj.</param>
    /// <returns></returns>
    public static T RandomSelectOne<T>(this IEnumerable<T> objs, Func<T, float> key)
    {
        IEnumerable<T> selected = objs.RandomSelectMany(key);

        if (selected.Count() < 1)
        {
            return objs.GetRandomValue();
        }
        else
        {
            return selected.GetRandomValue();
        }
    }


    /// <summary>
    /// Shuffles the list in place. This is an O(n) operation. Adapted from
    /// https://stackoverflow.com/a/1262619.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    /// <param name="list">List to shuffle.</param>
    public static void ShuffleInPlace<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = RandomInt(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    /// <summary>
    /// Selects one item from a param list of items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items">List to select from.</param>
    /// <returns>A random item from items.</returns>
    public static T SelectOneFrom<T>(params T[] items)
    {
        return items.GetRandomValue<T>();
    }
    #endregion
}
