using System.Collections;
using UnityEngine;

public readonly struct Ray2
{
    #region Properties
    /// <summary>
    /// Point where the line starts.
    /// </summary>
    public Vector2 From { get; }

    /// <summary>
    /// Normalized direction from <see cref="From"/> to <see cref="To"/>.
    /// </summary>
    public Vector2 Direction { get; }

    /// <summary>
    /// Positive angle from 0 radians. Bounded by [0, 2pi).
    /// </summary>
    public float Angle { get; }
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a ray starting at <paramref name="from"/> in the direction of
    /// <paramref name="direction"/>.
    /// </summary>
    /// <param name="from">Starting point of ray.</param>
    /// <param name="direction">Direction of the ray.</param>
    public Ray2(Vector2 from, Vector2 direction)
    {
        From = from;
        Direction = direction.normalized;
        Angle = Mathf.Atan2(direction.y, direction.x);
    }

    /// <summary>
    /// Creates a ray starting at <paramref name="from"/> with an angle
    /// <paramref name="angle"/> radians from 0.
    /// </summary>
    /// <param name="from">Starting point of line.</param>
    /// <param name="angle">Radians counterclockwise from 0 radians. [0,
    /// 2pi).</param>
    public Ray2(Vector2 from, float angle)
    {
        From = from;
        Angle = angle;
        Direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    #endregion
}