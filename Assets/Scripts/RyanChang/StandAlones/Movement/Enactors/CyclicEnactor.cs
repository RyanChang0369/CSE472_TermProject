using System.Linq;
using UnityEngine;

/// <summary>
/// Defines a cyclic movement enactor.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public class CyclicEnactor : MovementEnactor
{
    #region Variables
    /// <summary>
    /// The axis of movement. This vector will be normal to the plane of
    /// movement, such that the initial positive movement would be
    /// counter-clockwise relative to the axis.
    /// </summary>
    [Tooltip("The axis of movement. This vector will be normal to the plane " +
        "of movement, such that the initial positive movement would be " +
        "counter-clockwise relative to the axis.")]
    [SerializeField]
    private Vector3 axis = Vector3.up;

    /// <summary>
    /// Whether the movement occurs in local or global space.
    /// </summary>
    [Tooltip("Whether the movement occurs in local or global space.")]
    [SerializeField]
    private Space space = Space.Self;

    /// <summary>
    /// The speed of the motion.
    /// </summary>
    [Tooltip("The speed of the motion.")]
    [SerializeField]
    private AnimationCurve speed = AnimationCurve.Constant(0, 1, 1);
    #endregion

    #region Methods
    protected override void Enact()
    {
        if (speed.keys.NotEmpty())
        {
            float maxT = speed.keys.Last().time;

            transform.Rotate(
                axis,
                speed.Evaluate(_T % speed.MaxTime()),
                space
            );
        }
    }
    #endregion
}