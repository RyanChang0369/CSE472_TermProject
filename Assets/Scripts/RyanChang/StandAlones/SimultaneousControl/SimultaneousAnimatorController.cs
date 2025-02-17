using UnityEngine;

/// <summary>
/// Controls <see cref="SimultaneousAnimatorControl"/>. Allows each <see
/// cref="SimultaneousAnimatorControl"/> to be to be staggered, so that only a
/// few will be active at a time. Starts each one after the other, with
/// configurable amount of delay.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public class SimultaneousAnimatorController : SimultaneousController
{
    #region Variables
    [Tooltip("The name of the trigger that will be called to start each " +
        "animation. This can be controlled by each individual control as well.")]
    public string triggerName = "Start";

    [Tooltip("The layer index on which to check for animation completion. " +
        "Most likely, this will be 0.")]
    public int layerIndex = 0;

    [Tooltip("The name of the exit state, which the AnimatorControl uses to " +
        "check if the animator is done.")]
    public string endStateName = "Exit";
    #endregion

    protected override SimultaneousControl[] CreateControlsList()
    {
        return GetControls<SimultaneousAnimatorControl, Animator>();
    }
}