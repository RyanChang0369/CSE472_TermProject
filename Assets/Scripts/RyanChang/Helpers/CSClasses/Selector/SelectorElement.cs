using UnityEngine;

/// <summary>
/// An element in selector.
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public struct SelectorElement<T>
{
    #region Variables
    [Tooltip("The thing that could be chosen.")]
    public T selection;

    /// <summary>
    /// The likelihood that this element will be selected. This value may not be
    /// the actual mathematical probability if using <see
    /// cref="MonoSelector{T}"/> or <see cref="Selector{T}"/> with any <see
    /// cref="Selector{T}.SelectionMode"/> other than <see
    /// cref="Selector{T}.SelectionMode.ManyOrNone"/>. This is due to the
    /// offending modes forcing a selection to be made.
    /// </summary>
    [Tooltip("The likelihood that this element will be selected. " +
        "This is not the actual mathematical probability. " +
        "The higher this value, the more likely it is to be selected. " +
        "Elements with probabilities of zero or less MAY be selected IF " +
        "no other elements exists with probabilities of 1 or greater, based " +
        "on the selection mode.")]
    public float probability;
    #endregion


    #region Constructors
    /// <summary>
    /// Creates a new selector element.
    /// </summary>
    /// <param name="selection">The thing that could be chosen.</param>
    /// <param name="probability">The likelihood this element will be selected.
    /// This may not be the actual percentage, see <see
    /// cref="SelectorElement{T}.probability"/>.</param>
    public SelectorElement(T selection, float probability)
    {
        this.selection = selection;
        this.probability = probability;
    }
    #endregion
}