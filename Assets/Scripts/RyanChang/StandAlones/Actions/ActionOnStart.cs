using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Performs an action when the gameobject starts or awakes.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public class ActionOnStart : MonoBehaviour
{
    #region Enums
    /// <summary>
    /// Determines when the action will be taken.
    /// </summary>
    [System.Flags]
    public enum When
    {
        /// <summary>
        /// Runs the action on <see cref="Start()"/>.
        /// </summary>
        OnStart,
        /// <summary>
        /// Runs the action on <see cref="Awake()"/>.
        /// </summary>
        OnAwake,
        /// <summary>
        /// Runs the action one FixedUpdate after <see cref="Start()"/>.
        /// </summary>
        OnLateStart
    }
    #endregion

    #region Variables
    [Tooltip("Determines when the action will be taken.")]
    [SerializeField]
    private When when;

    [Tooltip("The action to perform.")]
    [SerializeField]
    private UnityEvent action;
    #endregion

    #region Instantiate
    private void Start()
    {
        if (when.HasFlag(When.OnStart))
            action.Invoke();

        if (when.HasFlag(When.OnLateStart))
            this.AfterFixedUpdate(() => action.Invoke());
    }

    private void Awake()
    {
        if (when.HasFlag(When.OnAwake))
            action.Invoke();
    }
    #endregion
}