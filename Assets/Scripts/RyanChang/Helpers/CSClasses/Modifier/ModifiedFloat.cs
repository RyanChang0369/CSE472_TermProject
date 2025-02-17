using UnityEngine;

/// <summary>
/// A float that is managed by a modifier chain.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
/// <seealso cref="ModifierChain"/>
[System.Serializable]
public class ModifiedFloat : IModifiedValue
{
    #region Variables
    [Tooltip("The initial value of this modifier float.")]
    [SerializeField]
    private float initialValue;

    [Tooltip("The modifier chain used to modify the initial value.")]
    public ModifierChain modifierChain;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the value that's modified by the modifier chain.
    /// </summary>
    public float Value => modifierChain.Modify(initialValue);
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new <see cref="ModifiedFloat"/>.
    /// </summary>
    /// <param name="initialValue">The initial value for this <see
    /// cref="ModifiedFloat"/>.</param>
    public ModifiedFloat(float initialValue)
    {
        this.initialValue = initialValue;
    }
    #endregion
}
