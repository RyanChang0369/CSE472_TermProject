using System;
using System.Collections.Generic;
using UnityEngine;
using static VectorExt;

/// <summary>
/// Contains methods pertaining to Unity bounds.
/// </summary>
public static class BoundsExt
{
    #region Modifications
    /// <summary>
    /// Translates the bounds by amount. Modifies the bounds in place.
    /// </summary>
    /// <param name="bounds">Bounds to translate.</param>
    /// <param name="amount">Amount to translate by.</param>
    /// <returns></returns>
    public static void Translate(this ref Bounds bounds, Vector3 amount)
    {
        Vector3 center = bounds.center + amount;
        bounds.center = center;
    }
    #endregion

    #region Conversions
    /// <summary>
    /// Converts this bounds to a rect. Follows the logic from
    /// <see cref="VectorExt.ToVector2(Vector3)"/>.
    /// </summary>
    /// <param name="bounds">The bounds used to create the rect.</param>
    /// <returns></returns>
    public static Rect ToRect(this Bounds bounds, Axis axis = DEFAULT_AXIS)
    {
        return new Rect(bounds.min.ToVector2(axis), bounds.size.ToVector2(axis));
    }

    /// <summary>
    /// Converts the specified BoundsInt to a regular Bounds.
    /// </summary>
    /// <param name="bounds">The bounds to convert.</param>
    /// <returns></returns>
    public static Bounds ToBounds(this BoundsInt bounds)
    {
        return new Bounds(bounds.center, bounds.size);
    }

    /// <summary>
    /// Converts the specified BoundsInt to a regular Bounds.
    /// </summary>
    /// <param name="bounds">The bounds to convert.</param>
    /// <returns></returns>
    public static BoundsInt ToBoundsInt(this Bounds bounds)
    {
        return new BoundsInt(bounds.center.ToVector3Int(), bounds.size.ToVector3Int());
    }
    #endregion

    #region Properties
    /// <summary>
    /// Calculates the perimeter of <paramref name="rect"/>.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <returns>The perimeter of <paramref name="rect"/>.</returns>
    public static float Perimeter(this Rect rect)
    {
        return 2f * (rect.width + rect.height);
    }

    /// <summary>
    /// Calculates the surface area of <paramref name="bounds"/>.
    /// </summary>
    /// <param name="bounds">The rectangle.</param>
    /// <returns>The surface area of <paramref name="bounds"/>.</returns>
    public static float SurfaceArea(this Bounds bounds)
    {
        return 2f * (bounds.size.x * bounds.size.y +
            bounds.size.x * bounds.size.z +
            bounds.size.z * bounds.size.y);
    }

    /// <summary>
    /// Enum for a rectangle corner.
    /// </summary>
    public enum RectCorner
    {
        BottomLeft,
        TopLeft,
        TopRight,
        BottomRight,
    }

    /// <summary>
    /// Enum for a rectangle edge.
    /// </summary>
    public enum RectEdge
    {
        Bottom,
        Right,
        Top,
        Left,
    }

    /// <summary>
    /// Visits every corner of <paramref name="rect"/>, starting from the
    /// minimum (bottom left) and moving counterclockwise.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    public static IEnumerable<Vector2> TraceCorners(this Rect rect)
    {
        foreach (var corner in Enum.GetValues(typeof(RectCorner)))
        {
            yield return rect.GetCorner((RectCorner)corner);
        }
    }

    /// <summary>
    /// Visits every edge of <paramref name="rect"/>, starting from the bottom
    /// and moving counterclockwise,
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    public static IEnumerable<Line2> TraceEdges(this Rect rect)
    {
        foreach (var edge in Enum.GetValues(typeof(RectEdge)))
        {
            yield return rect.GetEdge((RectEdge)edge);
        }
    }

    /// <summary>
    /// Retrieves the coordinates of the corner <paramref name="corner"/> of
    /// <paramref name="rect"/>.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <param name="corner">The corner to select.</param>
    public static Vector2 GetCorner(this Rect rect, RectCorner corner) =>
        corner switch
        {
            RectCorner.BottomLeft => rect.min,
            RectCorner.TopLeft => new(rect.xMax, rect.yMin),
            RectCorner.TopRight => rect.max,
            RectCorner.BottomRight => new(rect.xMin, rect.yMax),
            _ => throw new ArgumentOutOfRangeException(
                nameof(corner),
                $"Not a valid corner {corner}."
            ),
        };

    /// <summary>
    /// Retrieves the line of the edge <paramref name="edge"/> of <paramref
    /// name="rect"/>.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <param name="edge">The edge to select.</param>
    public static Line2 GetEdge(this Rect rect, RectEdge edge) =>
        edge switch
        {
            RectEdge.Bottom => new(
                rect.GetCorner(RectCorner.BottomLeft),
                rect.GetCorner(RectCorner.BottomRight)
            ),
            RectEdge.Right => new(
                rect.GetCorner(RectCorner.BottomRight),
                rect.GetCorner(RectCorner.TopRight)
            ),
            RectEdge.Top => new(
                rect.GetCorner(RectCorner.TopRight),
                rect.GetCorner(RectCorner.TopLeft)
            ),
            RectEdge.Left => new(
                rect.GetCorner(RectCorner.TopLeft),
                rect.GetCorner(RectCorner.BottomLeft)
            ),
            _ => throw new ArgumentOutOfRangeException(
                nameof(edge),
                $"Not a valid edge {edge}."
            ),
        };

    /// <summary>
    /// Picks a point that lies at <paramref name="t"/> percent along the edge
    /// of the <paramref name="rect"/>, measured from the lower left corner of
    /// the <paramref name="rect"/> traveling counterclockwise.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <param name="t">The percentage along the perimeter. Must be finite. If
    /// negative, travel clockwise instead of counterclockwise.</param>
    public static Vector2 AlongEdge(this Rect rect, float t)
    {
        if (!float.IsFinite(t))
            throw new ArgumentException(
                $"{t} is not finite.",
                nameof(t)
            );

        // If t is greater than 1 (or less than -1), then only take the
        // fractional component.
        t %= 1f;

        // If t is negative, make it positive.
        if (t < 0) t++;

        // t now is bounded by [0, 1). Declare some more variables.
        float pt = t * rect.Perimeter();
        float deltaT = 0;

        foreach (var edge in rect.TraceEdges())
        {
            // Travel along the edge of the rectangle until we reach the
            // edge where t lies.
            deltaT += edge.Length;

            if (pt < edge.Length)
                return edge.AlongLine(pt / deltaT);

            pt -= edge.Length;
        }

        // The code should never reach this point.
        throw new InvalidOperationException(
            "Invalid t or rectangle.\n" +
            $"Value of t: {t}\n" +
            $"Value of rect: {rect}"
        );
    }
    #endregion
}
