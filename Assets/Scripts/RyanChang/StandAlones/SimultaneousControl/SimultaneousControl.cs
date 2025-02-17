using System.Collections;
using UnityEngine;

/// <summary>
/// Used with <see cref="SimultaneousController"/>, allows for the simultaneous
/// (but not concurrent) execution of multiple behaviors.
/// <br/>
/// Authors: Ryan Chang (2023)
/// </summary>
public abstract class SimultaneousControl : MonoBehaviour
{
    #region Variables
    public SimultaneousController Controller { get; set; }
    #endregion

    /// <summary>
    /// Instantiates this control.
    /// </summary>
    public abstract void Instantiate();

    /// <summary>
    /// Resets the control.
    /// </summary>
    public abstract void ResetControl();

    /// <summary>
    /// Called when the control should be disabled.
    /// </summary>
    public abstract void DisableControl();

    /// <summary>
    /// Performs the action set by this control.
    /// </summary>
    public abstract IEnumerator DoAction();
}