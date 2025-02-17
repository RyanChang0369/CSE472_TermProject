using System.Collections;
using NaughtyAttributes;
using UnityEngine;

/// <summary>
/// Controls an <see cref="Animator"/>. Combined with <see
/// cref="SimultaneousAnimatorController"/>, multiple animators can be chained
/// together such that a few run together. The <see cref="Animator"/> must
/// contain a trigger with the name "Start", but this can be configured.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
/// <seealso cref="SimultaneousAnimatorController"/>
public class SimultaneousAnimatorControl : SimultaneousControl
{
    #region Variables
    [Tooltip("The animator. If not specified, will be derived from any " +
        "animator found in this gameobject.")]
    public Animator animator;

    [Tooltip("If true, then override the trigger name provided by the " +
        "controller.")]
    [SerializeField]
    private bool overrideTriggerName;

    [Tooltip("The overridden trigger name.")]
    [ShowIf(nameof(overrideTriggerName))]
    [SerializeField]
    private string triggerName = "Start";

    [Tooltip("If true, then override the layer index provided by the " +
        "controller.")]
    [SerializeField]
    private bool overrideLayerIndex;

    [Tooltip("The overridden layer index.")]
    [ShowIf(nameof(overrideLayerIndex))]
    [SerializeField]
    private int layerIndex = 0;

    [Tooltip("If true, then override the end state name provided by the " +
        "controller.")]
    [SerializeField]
    private bool overrideEndStateName;

    [Tooltip("The overridden end state name.")]
    [EnableIf(nameof(overrideEndStateName))]
    [SerializeField]
    private string endStateName = "Exit";
    #endregion

    public override void Instantiate()
    {
        if (!overrideTriggerName)
        {
            triggerName = ((SimultaneousAnimatorController)Controller)
                .triggerName;
        }

        if (!overrideLayerIndex)
        {
            layerIndex = ((SimultaneousAnimatorController)Controller)
                .layerIndex;
        }

        if (!overrideEndStateName)
        {
            endStateName = ((SimultaneousAnimatorController)Controller)
                .endStateName;
        }

        if (!animator)
            this.RequireComponent(out animator);
    }

    public override IEnumerator DoAction()
    {
        animator.SetTrigger(triggerName);

        // Wait for animator to end.
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Exit"));
    }

    public override void ResetControl()
    {
        // Do nothing.
    }

    public override void DisableControl()
    {
        // Do nothing.
    }
}