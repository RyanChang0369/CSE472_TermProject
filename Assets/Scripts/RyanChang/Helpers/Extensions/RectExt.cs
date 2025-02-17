using UnityEngine;

/// <summary>
/// Contains methods pertaining to Unity rectangles.
/// </summary>
public static class RectExt
{
    #region Modifications
    /// <summary>
    /// Translates the rectangle by amount. Modifies the rectangle in place.
    /// </summary>
    /// <param name="rect">Rectangle to translate.</param>
    /// <param name="amount">Amount to translate by.</param>
    /// <returns></returns>
    public static void Translate(this ref Rect rect, Vector2 amount)
    {
        Vector2 position = rect.position + amount;
        rect.position = position;
    }

    /// <param name="x">The x component of the translation vector.</param>
    /// <param name="y">The y component of the translation vector.</param>
    /// <inheritdoc cref="Translate(ref Rect, Vector2)"/>
    public static void Translate(this ref Rect rect, float x, float y)
    {
        rect.Translate(new(x, y));
    }

    /// <inheritdoc cref="Translate(ref Rect, Vector2)"/>
    public static void Translate(this ref RectInt rect, Vector2Int amount)
    {
        Vector2Int position = rect.position + amount;
        rect.position = position;
    }

    /// <inheritdoc cref="Translate(ref Rect, float, float)"/>
    public static void Translate(this ref RectInt rect, int x, int y)
    {
        rect.Translate(new(x, y));
    }

    /// <summary>
    /// Translates the rectangle in the x axis by <paramref name="x"/> amount.
    /// </summary>
    /// <inheritdoc cref="Translate(ref Rect, float, float)"/>
    public static void TranslateX(this ref Rect rect, float x) =>
        rect.Translate(x, 0);

    /// <inheritdoc cref="TranslateX(ref Rect, float)"/>
    public static void TranslateX(this ref RectInt rect, int x) =>
        rect.Translate(x, 0);

    /// <summary>
    /// Translates the rectangle in the y axis by <paramref name="y"/> amount.
    /// </summary>
    /// <inheritdoc cref="Translate(ref Rect, float, float)"/>
    public static void TranslateY(this ref Rect rect, float y) =>
        rect.Translate(0, y);

    /// <inheritdoc cref="TranslateY(ref Rect, float)"/>
    public static void TranslateY(this ref RectInt rect, int y) =>
        rect.Translate(0, y);
    #endregion

    #region Conversions
    /// <summary>
    /// Converts this rect to a bounds. Follows the logic from
    /// <see cref="VectorExt.ToVector3(Vector2, float)"/>, so the x and z
    /// components of the new bounds will be the x and y components of rect.
    /// </summary>
    /// <param name="rect">The rect used to create the bounds.</param>
    /// <param name="y">The optional y component that will be used as the y
    /// component of the minimum corner of the new bounds.</param>
    /// <param name="height">The optional height of the new bounds.</param>
    /// <returns></returns>
    public static Bounds ToBounds(this Rect rect, float y = 0, float height = 0)
    {
        return new Bounds(rect.min.ToVector3(y), rect.size.ToVector3(height));
    }
    #endregion

    #region "Constructors"
    #region Centered At
    /// <summary>
    /// Creates a new Rect centered at the provided values.
    /// </summary>
    /// <param name="center">Coordinates of the center.</param>
    /// <param name="dimensions">Width and height of the rect.</param>
    public static Rect CenteredAt(this Vector2 center, Vector2 dimensions)
    {
        return CenteredAt(center.x, center.y, dimensions.x, dimensions.y);
    }

    /// <summary>
    /// Creates a new Rect centered at the provided values.
    /// </summary>
    /// <inheritdoc cref="CenteredAt(Vector2, Vector2)"/>
    /// <inheritdoc cref="CenteredAt(float, float, float, float)"/>
    public static Rect CenteredAt(this Vector2 center,
        float width, float height) =>
        CenteredAt(center.x, center.y, width, height);

    /// <summary>
    /// Creates a new square centered at the provided values.
    /// </summary>
    /// <inheritdoc cref="CenteredAt(Vector2, Vector2)"/>
    /// <param name="length">Width and height of the rect.</param>
    public static Rect CenteredAt(this Vector2 center,
        float length) =>
        CenteredAt(center.x, center.y, length, length);

    /// <summary>
    /// Creates a new Rect centered at the provided values.
    /// </summary>
    /// <param name="centerX">X coordinate of the center.</param>
    /// <param name="centerY">Y coordinate of the center.</param>
    /// <param name="width">Width of the rect.</param>
    /// <param name="height">Height of the rect.</param>
    public static Rect CenteredAt(float centerX, float centerY,
        float width, float height)
    {
        return new Rect(
            centerX - (width / 2), centerY - (height / 2),
            width, height
        );
    }
    #endregion
    #endregion
}
