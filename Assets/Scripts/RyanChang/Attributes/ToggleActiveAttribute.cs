using System;
using UnityEngine;

/// <summary>
/// Attribute that allows a field to be enabled.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
[AttributeUsage(AttributeTargets.Field |
    AttributeTargets.Property | AttributeTargets.Method)]
[Serializable]
public class ToggleActiveAttribute : PropertyAttribute
{
    #region Variables / Properties
    /// <summary>
    /// The name of the attribute that is enabled/disabled by the boolean.
    /// </summary>
    private readonly string toggled;

    public string ToggledName => toggled;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a toggle active attribute.
    /// </summary>
    /// <param name="toggled">The name of the boolean that activates the
    /// attribute.</param>
    public ToggleActiveAttribute(string toggled)
    {
        this.toggled = toggled;
    }
    #endregion
}