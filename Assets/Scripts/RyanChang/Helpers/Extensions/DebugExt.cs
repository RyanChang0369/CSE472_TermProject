using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Contains methods pertaining to debug drawing, gizmos drawing, and other
/// methods that may be useful for debugging.
/// </summary>
public static class DebugExt
{
    #region Drawing
    #region Variables
    public static Color primaryColor;
    public static Color secondaryColor;
    public static Color tertiaryColor;
    public static Color quaternaryColor;

    public static float drawDuration;

    public enum DrawingMode
    {
        /// <summary>
        /// Draws using methods from Debug.
        /// </summary>
        Debug,
        /// <summary>
        /// Draws using methods from Gizmos. duration parameters are ignored.
        /// </summary>
        Gizmos
    }

    private static DrawingMode drawingMode;
    #endregion

    #region Setup
    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    /// <param name="tertiary">The second accent color to use.</param>
    /// <param name="quaternary">The third accent color to use.</param>
    /// <param name="duration">Time to show the drawn object.</param>
    public static void UseDebug(Color primary, Color secondary, Color tertiary,
        Color quaternary, float duration)
    {
        primaryColor = primary;
        secondaryColor = secondary;
        tertiaryColor = tertiary;
        quaternaryColor = quaternary;
        drawingMode = DrawingMode.Debug;
        drawDuration = duration;
    }

    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    /// <param name="tertiary">The second accent color to use.</param>
    /// <param name="quaternary">The third accent color to use.</param>
    public static void UseDebug(Color primary, Color secondary, Color tertiary,
        Color quaternary)
    {
        UseDebug(primary, secondary, tertiary, quaternary, Time.deltaTime);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    /// <param name="tertiary">The second accent color to use.</param>
    /// <param name="duration">Time to show the drawn object.</param>
    public static void UseDebug(Color primary, Color secondary, Color tertiary,
        float duration)
    {
        UseDebug(primary, secondary, tertiary, tertiary, duration);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    /// <param name="tertiary">The second accent color to use.</param>
    public static void UseDebug(Color primary, Color secondary, Color tertiary)
    {
        UseDebug(primary, secondary, tertiary, tertiary);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    /// <param name="duration">Time to show the drawn object.</param>
    public static void UseDebug(Color primary, Color secondary, float duration)
    {
        UseDebug(primary, secondary, secondary, duration);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    public static void UseDebug(Color primary, Color secondary)
    {
        UseDebug(primary, secondary, secondary);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="duration">Time to show the drawn object.</param>
    public static void UseDebug(Color primary, float duration)
    {
        UseDebug(primary, primary, duration);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Debug methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    public static void UseDebug(Color primary)
    {
        UseDebug(primary, primary);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Gizmos methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    /// <param name="tertiary">The second accent color to use.</param>
    /// <param name="quaternary">The third accent color to use.</param>
    public static void UseGizmos(Color primary, Color secondary, Color tertiary,
        Color quaternary)
    {
        primaryColor = primary;
        secondaryColor = secondary;
        tertiaryColor = tertiary;
        quaternaryColor = quaternary;
        drawingMode = DrawingMode.Gizmos;
    }

    /// <summary>
    /// Sets up DebugExt to draw with Gizmos methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    /// <param name="tertiary">The second accent color to use.</param>
    public static void UseGizmos(Color primary, Color secondary, Color tertiary)
    {
        UseGizmos(primary, secondary, tertiary, tertiary);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Gizmos methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    /// <param name="secondary">The first accent color to use.</param>
    public static void UseGizmos(Color primary, Color secondary)
    {
        UseGizmos(primary, secondary, secondary);
    }

    /// <summary>
    /// Sets up DebugExt to draw with Gizmos methods.
    /// </summary>
    /// <param name="primary">The primary color used to draw.</param>
    public static void UseGizmos(Color primary)
    {
        UseGizmos(primary, primary);
    }
    #endregion

    #region Internal
    private static void DrawFace(Vector3[] corners)
    {
        // Draw outer box
        for (int i = 1; i < corners.Length; i++)
        {
            DrawLine(corners[i - 1], corners[i], primaryColor);
        }
        DrawLine(corners[0], corners[^1], primaryColor);

        // Draw cross
        DrawLine(corners[0], corners[2], secondaryColor);
        DrawLine(corners[1], corners[3], secondaryColor);
    }

    /// <summary>
    /// Draws a rectangular prism.
    /// </summary>
    /// <param name="corners"></param>
    private static void Draw8Face(Vector3[] corners)
    {
        // Draw outer box
        for (int i = 1; i < corners.Length; i++)
        {
            DrawLine(corners[i - 1], corners[i], primaryColor);
        }
        DrawLine(corners[0], corners[^1], primaryColor);

        // Other corners
        DrawLine(corners[1], corners[6], primaryColor);
        DrawLine(corners[2], corners[7], primaryColor);
        DrawLine(corners[3], corners[8], primaryColor);

        // Draw cross
        DrawLine(corners[0], corners[7], secondaryColor);
        DrawLine(corners[1], corners[8], secondaryColor);
        DrawLine(corners[2], corners[9], secondaryColor);
        DrawLine(corners[3], corners[6], secondaryColor);
    }

    private static void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        switch (drawingMode)
        {
            case DrawingMode.Debug:
                Debug.DrawLine(from, to, color, drawDuration);
                break;
            case DrawingMode.Gizmos:
                Gizmos.color = color;
                Gizmos.DrawLine(from, to);
                break;
        }
    }
    #endregion

    #region Cross Square
    /// <summary>
    /// Draws a crossed square.
    /// </summary>
    /// <param name="position">Center of the cross.</param>
    /// <param name="rotation">Rotation of the drawn item.</param>
    /// <param name="size">Length of the box.</param>
    public static void DrawCrossSquare(Vector3 position, Quaternion rotation,
        float size)
    {
        float half = size / 2;

        // Define corners
        Vector3[] corners = {
            // First face
            new(half, half, 0),
            new(-half, half, 0),
            new(-half, -half, 0),
            new(half, -half, 0),
            new(half, half, 0),
        };

        // Apply rotation, then transformation.
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * corners[i] + position;
        }

        DrawFace(corners);
    }

    /// <summary>
    /// Draws a crossed square.
    /// </summary>
    /// <param name="position">Center of the cross.</param>
    /// <param name="size">Length of the box.</param>
    public static void DrawCrossSquare(Vector3 position, float size)
    {
        DrawCrossSquare(position, Quaternion.identity, size);
    }
    #endregion

    #region Cross Cube
    /// <summary>
    /// Draws a crossed cube.
    /// </summary>
    /// <param name="position">Center of the cross, at world position.</param>
    /// <param name="rotation">Rotation of the drawn item.</param>
    /// <param name="size">Length of the cube.</param>
    public static void DrawCrossCube(Vector3 position, Quaternion rotation,
        float size)
    {
        float half = size / 2;

        // Define corners
        Vector3[] corners = {
            // First face
            new(half, half, half),
            new(-half, half, half),
            new(-half, -half, half),
            new(half, -half, half),
            new(half, half, half),
            //Cross over to other face
            new(half, half, -half),
            new(-half, half, -half),
            new(-half, -half, -half),
            new(half, -half, -half),
            new(half, half, -half),
        };

        // Apply rotation, then transformation.
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * corners[i] + position;
        }

        Draw8Face(corners);
    }

    /// <summary>
    /// Draws a crossed cube.
    /// </summary>
    /// <param name="position">Center of the cross.</param>
    /// <param name="size">Length of the box.</param>
    public static void DrawCrossCube(Vector3 position, float size)
    {
        DrawCrossCube(position, Quaternion.identity, size);
    }
    #endregion

    #region Cross Rect
    /// <summary>
    /// Draws a crossed rectangle.
    /// </summary>
    /// <param name="min">Minimum corner of the rectangle.</param>
    /// <param name="max">Maximum corner of the rectangle.</param>
    /// <param name="rotation">Rotation of the drawn object.</param>
    public static void DrawCrossRect(Vector2 min, Vector2 max,
        Quaternion rotation, float y = 0)
    {
        // Define corners
        Vector3[] corners = {
            new(max.x, y, max.y),
            new(min.x, y, max.y),
            new(min.x, y, min.y),
            new(max.x, y, min.y)
        };

        // Center rect at origin, rotate, then transform back to original
        // position.
        Vector3 center = min.Average(max).ToVector3();
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * (corners[i] - center) + center;
        }

        DrawFace(corners);
    }

    /// <summary>
    /// Draws a crossed rectangle.
    /// </summary>
    /// <param name="min">Minimum corner of the rectangle.</param>
    /// <param name="max">Maximum corner of the rectangle.</param>
    public static void DrawCrossRect(Vector2 min, Vector2 max, float y = 0)
    {
        DrawCrossRect(min, max, Quaternion.identity, y);
    }

    /// <summary>
    /// Draws a crossed rectangle.
    /// </summary>
    /// <param name="rect">Rectangle to draw.</param>
    /// <param name="rotation">Rotation of the drawn object.</param>
    public static void DrawCrossRect(Rect rect, Quaternion rotation, float y = 0)
    {
        // Define corners
        Vector3[] corners = {
            new(rect.xMax, y, rect.yMax),
            new(rect.xMin, y, rect.yMax),
            new(rect.xMin, y, rect.yMin),
            new(rect.xMax, y, rect.yMin)
        };

        // Center rect at origin, rotate, then transform back to original
        // position.
        Vector3 center = rect.center.ToVector3();
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * (corners[i] - center) + center;
        }

        DrawFace(corners);
    }

    /// <summary>
    /// Draws a crossed rectangle.
    /// </summary>
    /// <param name="rect">Rectangle to draw.</param>
    public static void DrawCrossRect(Rect rect, float y = 0)
    {
        DrawCrossRect(rect, Quaternion.identity, y);
    }
    #endregion

    #region Cross Bounds
    /// <summary>
    /// Draws a crossed bound.
    /// </summary>
    /// <param name="bounds">Bounds to draw.</param>
    /// <param name="rotation">Rotation of the drawn item.</param>
    public static void DrawCrossBounds(Bounds bounds, Quaternion rotation)
    {
        Vector3 max = bounds.max;
        Vector3 min = bounds.min;

        // Define corners
        Vector3[] corners = {
            // First face
            new(max.x, max.y, max.z),
            new(min.x, max.y, max.z),
            new(min.x, min.y, max.z),
            new(max.x, min.y, max.z),
            new(max.x, max.y, max.z),
            //Cross over to other face
            new(max.x, max.y, min.z),
            new(min.x, max.y, min.z),
            new(min.x, min.y, min.z),
            new(max.x, min.y, min.z),
            new(max.x, max.y, min.z),
        };

        // Center rect at origin, rotate, then transform back to original
        // position.
        Vector3 center = bounds.center;
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * (corners[i] - center) + center;
        }

        Draw8Face(corners);
    }

    /// <summary>
    /// Draws a crossed bound.
    /// </summary>
    /// <param name="bounds">Bounds to draw.</param>
    public static void DrawCrossBounds(Bounds bounds)
    {
        DrawCrossBounds(bounds, Quaternion.identity);
    }

    /// <inheritdoc cref="DrawCrossBounds(Bounds, Quaternion)"/>
    public static void DrawCrossBounds(BoundsInt bounds, Quaternion rotation)
    {
        DrawCrossBounds(bounds.ToBounds(), rotation);
    }


    /// <inheritdoc cref="DrawCrossBounds(Bounds)"/>
    public static void DrawCrossBounds(BoundsInt bounds)
    {
        DrawCrossBounds(bounds, Quaternion.identity);
    }
    #endregion
    #endregion

    #region Print to Console
    /// <summary>
    /// Only log messages flagged with one of the flags defined in the
    /// whitelist.
    /// </summary>
    public static DebugGroup Whitelist => DebugGroup.All;

    /// <inheritdoc cref="Whitelist"/>
    [Flags]
    public enum DebugGroup
    {
        GameState = 0b0000_0001,
        ArtificialPlayer = 0b0000_0010,
        GenomeAndFish = 0b0000_0100,
        Config = 0b0000_1000,
        Tower = 0b0001_0000,
        Briefing = 0b0010_0000,
        Addressable = 0b0100_0000,
        Audio = 0b1000_0000,
        All = 0b1111_1111
    }

    /// <summary>
    /// Prints <paramref name="message"/> to the Unity debug console.
    /// </summary>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="grouping">Type of message to log, filtered by <see
    /// cref="Whitelist"/>.</param>
    public static void Log(
        this UnityEngine.Object context,
        DebugGroup grouping,
        string message)
    {
        if (Whitelist.HasAnyFlag(grouping))
        {
            Debug.Log($"{grouping}: {message}", context);
        }
    }

    /// <inheritdoc cref="Log(UnityEngine.Object, DebugGroup, string)"/>
    public static void Log(DebugGroup grouping, string message) =>
        Log(null, grouping, message);

    /// <summary>
    /// Prints <paramref name="messages"/> to the debug console. This methods
    /// mimics the 'print' function from python.
    /// </summary>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="messages"></param>
    public static void Print(this UnityEngine.Object context,
        params object[] messages)
    {
        Debug.Log(string.Join(' ', messages), context);
    }

    /// <inheritdoc cref="Print(UnityEngine.Object, object[])"/>
    public static void Print(params object[] messages)
    {
        Debug.Log(string.Join(' ', messages));
    }
    #endregion

    #region Assertions
    #region Collection
    /// <summary>
    /// Determines if all elements in <paramref name="collection"/> meets
    /// <paramref name="condition"/>.
    /// </summary>
    /// <param name="collection">Collection to test.</param>
    /// <param name="condition">Condition each element in <paramref
    /// name="collection"/> must pass. An element passes if this returns true,
    /// and fails otherwise.</param>
    /// <param name="message">Message to display if assertion fails.</param>
    /// <typeparam name="T"></typeparam>
    public static void AssertAll<T>(this IEnumerable<T> collection,
        Func<T, bool> condition, string message = "")
    {
        foreach (var item in collection)
        {
            if (!condition(item))
            {
                throw new AssertionException(
                    $"AssertAll failed on {item} in {collection}", message);
            }
        }
    }
    #endregion

    #region Not Empty
    /// <summary>
    /// Generates the Exception message for the <see cref="AssertNotEmpty"/>
    /// methods.
    /// </summary>
    /// <param name="reason"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string GenEmptyOrNullMessage(string reason, string name)
    {
        return string.IsNullOrWhiteSpace(name) ?
            $"Collection was {reason}" :
            $"Collection {name.Trim()} was {reason}";
    }

    /// <summary>
    /// Asserts that <paramref name="collection"/> is neither null nor empty.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <param name="nullMessage">The message to display if the collection was
    /// null.</param>
    /// <param name="emptyMessage">The message to display if the collection was
    /// empty.</param>
    /// <param name="name">Name of the collection.</param>
    public static void AssertNotEmpty<T>(this ICollection<T> collection,
        string nullMessage, string emptyMessage, string name)
    {
        if (collection == null)
            throw new AssertionException(
                GenEmptyOrNullMessage("null", name),
                nullMessage
            );
        else if (collection.Count <= 0)
            throw new AssertionException(
                GenEmptyOrNullMessage("empty", name),
                emptyMessage
            );
    }

    /// <inheritdoc cref="AssertNotEmpty{T}(ICollection{T}, string, string,
    /// string)"/>
    /// <param name="message">The message to display if either <paramref
    /// name="collection"/> is null or empty.</param>
    public static void AssertNotEmpty<T>(this ICollection<T> collection,
        string message, string name) =>
        AssertNotEmpty(collection, message, message, name);

    /// <inheritdoc cref="AssertNotEmpty{T}(ICollection{T}, string, string,
    public static void AssertNotEmpty<T>(this ICollection<T> collection,
        string message) =>
        AssertNotEmpty(collection, message, null);

    /// <inheritdoc cref="AssertNotEmpty{T}(ICollection{T}, string, string,
    /// string)"/>
    /// <param name="values">The enumeration to check.</param>
    public static void AssertNotEmpty<T>(this IEnumerable<T> values,
        string nullMessage, string emptyMessage, string name)
    {
        if (values == null)
            throw new AssertionException(
                GenEmptyOrNullMessage("null", name),
                nullMessage
            );
        else if (!values.Any())
            throw new AssertionException(
                GenEmptyOrNullMessage("empty", name),
                emptyMessage
            );
    }

    /// <inheritdoc cref="AssertNotEmpty{T}(IEnumerable{T}, string, string,
    /// string)"/>
    /// <param name="message">The message to display if either <paramref
    /// name="collection"/> is null or empty.</param>
    public static void AssertNotEmpty<T>(this IEnumerable<T> collection,
        string message, string name) =>
        AssertNotEmpty(collection, message, message, name);

    /// <inheritdoc cref="AssertNotEmpty{T}(IEnumerable{T}, string, string,
    public static void AssertNotEmpty<T>(this IEnumerable<T> collection,
        string message) =>
        AssertNotEmpty(collection, message, null);
    #endregion

    #region Nullity
    #region Not Null
    /// <summary>
    /// Asserts that <see cref="value"/> is not null.
    /// </summary>
    /// <typeparam name="T">Any nullable value.</typeparam>
    /// <param name="value">Value to test.</param>
    /// <param name="message">Message to display if <paramref name="value"/> was
    /// null.</param>
    /// <param name="name">Variable name of the <paramref
    /// name="value"/>.</param>
    public static void AssertNotNull<T>(this T value,
        string message, string name) where T : class
    {
        if (value == null)
            throw new AssertionException(
                $"{name} was null. Expected: not null.",
                message
            );
    }

    /// <inheritdoc cref="AssertNotNull{T}(T, string, string)"/>
    public static void AssertNotNull<T>(this T value,
        string message) where T : class =>
        AssertNotNull(value, message, "Value");

    /// <inheritdoc cref="AssertNotNull{T}(T, string, string)"/>
    public static void AssertNotNull<T>(this T value) where T : class =>
        AssertNotNull(value, "", "Value");
    #endregion

    #region Is Null
    /// <summary>
    /// Asserts that <see cref="value"/> is null.
    /// </summary>
    /// <typeparam name="T">Any nullable value.</typeparam>
    /// <param name="value">Value to test.</param>
    /// <param name="message">Message to display if <paramref name="value"/> was
    /// not null.</param>
    /// <param name="name">Variable name of the <paramref
    /// name="value"/>.</param>
    public static void AssertIsNull<T>(this T value,
        string message, string name) where T : class
    {
        if (value != null)
            throw new AssertionException(
                $"{name} was not null. Expected: null.",
                message
            );
    }

    /// <inheritdoc cref="AssertIsNull{T}(T, string, string)"/>
    public static void AssertIsNull<T>(this T value,
        string message) where T : class =>
        AssertIsNull(value, message, "Value");

    /// <inheritdoc cref="AssertIsNull{T}(T, string, string)"/>
    public static void AssertIsNull<T>(this T value) where T : class =>
        AssertIsNull(value, "", "Value");
    #endregion
    #endregion
    #endregion
}
