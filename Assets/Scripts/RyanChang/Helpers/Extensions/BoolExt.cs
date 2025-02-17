
using System.Linq;


/// <summary>
/// Extension methods relating to booleans.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public static class BoolExt
{
    #region Null Check
    /// <summary>
    /// True if any of <paramref name="obj"/> is null.
    /// </summary>
    /// <param name="obj">The objects to check for null-ness.</param>
    /// <returns>True if any of <paramref name="obj"/> is null.</returns>
    public static bool AnyNull(params object[] obj)
    {
        return obj.Any(o => o == null);
    }
    #endregion   
}