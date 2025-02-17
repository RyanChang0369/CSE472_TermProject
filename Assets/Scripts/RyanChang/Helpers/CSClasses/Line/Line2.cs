using UnityEngine;

/// <summary>
/// Defines a two-dimensional line.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public readonly struct Line2
{
    #region Properties
    /// <summary>
    /// Point where the line starts.
    /// </summary>
    public Vector2 From { get; }

    /// <summary>
    /// Point where the line ends.
    /// </summary>
    public Vector2 To { get; }

    /// <summary>
    /// Normalized direction from <see cref="From"/> to <see cref="To"/>.
    /// </summary>
    public Vector2 Direction { get; }

    /// <summary>
    /// The length of the line.
    /// </summary>
    public float Length { get; }

    /// <summary>
    /// Positive angle from 0 radians. Bounded by [0, 2pi).
    /// </summary>
    public float Angle { get; }

    /// <summary>
    /// Point at the center of the line.
    /// </summary>
    public Vector2 Center => AlongLine(0.5f);

    /// <summary>
    /// Returns the line starting at <see cref="Center"/> with an angle normal to
    /// this line.
    /// </summary>
    public Line2 Normal => new(Center, Angle + FloatAngle.PI1_2, 1);
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a line starting at <paramref name="from"/> and ending at
    /// <paramref name="to"/>.
    /// </summary>
    /// <param name="from">Starting point of line.</param>
    /// <param name="to">Ending point of the line</param>
    public Line2(Vector2 from, Vector2 to)
    {
        From = from;
        To = to;

        // Save a sqrt operation.
        Vector2 d = to - from;
        float mag = d.magnitude;
        Length = mag;
        Direction = mag.Approx(0) ? Vector2.zero : d / mag;

        Angle = Mathf.Atan2(Direction.y, Direction.x);
    }

    /// <summary>
    /// Creates a line starting at <paramref name="from"/> with length <paramref
    /// name="length"/> and angle from 0 radians <paramref name="angle"/> in
    /// radians.
    /// </summary>
    /// <param name="from">Starting point of line.</param>
    /// <param name="angle">Radians counterclockwise from 0 radians. [0, 2pi).</param>
    /// <param name="length">Length of the line.</param>
    public Line2(Vector2 from, float angle, float length)
    {
        From = from;
        Angle = angle;
        Length = length;

        Direction = new(Mathf.Cos(angle), Mathf.Sin(angle));
        To = from + Direction * length;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Returns a point that is <see cref="t"/> percent along the line starting
    /// at <see cref="From"/>.
    /// </summary>
    /// <param name="t">The percentage along the line. If under 0, then this
    /// will return some point behind <see cref="From"/>. If over 1, then this
    /// will return some point ahead of <see cref="To"/>.</param>
    public Vector2 AlongLine(float t) =>
        From + (Direction * (float)(Length * t));
    #endregion
}