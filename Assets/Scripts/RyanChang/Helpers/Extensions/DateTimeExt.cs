using System;

/// <summary>
/// Contains methods that extend <see cref="System.DateTime"/>.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public static class DateTimeExt
{
    /// <summary>
    /// Returns a string representation of <paramref name="dt"/> that can be
    /// used as a file name (does not have any illegal characters).
    /// </summary>
    /// <param name="dt">The <see cref="DateTime"/>.</param>
    /// <returns></returns>
    public static string ToFileName(this DateTime dt)
    {
        return dt.ToString("yyyy_MM_dd_HH_mm");
    }

    /// <summary>
    /// Returns a string representation of <paramref name="dt"/> that is the
    /// combination of <see cref="DateTime.ToLongDateString"/> and <see
    /// cref="DateTime.ToLongTimeString"/>.
    /// </summary>
    /// <param name="dt">The <see cref="DateTime"/>.</param>
    /// <returns></returns>
    public static string ToLongDateTimeString(this DateTime dt)
    {
        return $"{dt.ToLongDateString()} {dt.ToLongTimeString()}";
    }

    /// <summary>
    /// Returns a string representation of <paramref name="dt"/> that is the
    /// combination of <see cref="DateTime.ToShortDateString"/> and <see
    /// cref="DateTime.ToShortTimeString"/>.
    /// </summary>
    /// <param name="dt">The <see cref="DateTime"/>.</param>
    /// <returns></returns>
    public static string ToShortDateTimeString(this DateTime dt)
    {
        return $"{dt.ToShortDateString()} {dt.ToShortTimeString()}";
    }
}