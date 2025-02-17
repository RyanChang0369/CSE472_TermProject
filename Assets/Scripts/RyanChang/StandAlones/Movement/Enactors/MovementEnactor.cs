using UnityEngine;

/// <summary>
/// Standalone behaviors that continuously applies a non-rigidbody motion to
/// some gameobject.
/// </summary>
///
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public abstract class MovementEnactor : MonoBehaviour
{
    #region Properties
#pragma warning disable IDE1006 // Naming Styles
    protected float _T => Time.time;

    protected float _dT => Time.deltaTime;
#pragma warning restore IDE1006 // Naming Styles
    #endregion

    #region Methods
    private void Update()
    {
        Enact();
    }

    /// <summary>
    /// Performs the movement action.
    /// </summary>
    protected abstract void Enact();
    #endregion
}