using UnityEngine;

/// <summary>
/// Randomly selects one component.
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class MonoSelector<T>
{
    #region Variables
    /// <summary>
    /// The elements to select from.
    /// </summary>
    [Tooltip("The elements to select from.")]
    [SerializeField]
    private SelectorElement<T>[] elements;
    #endregion

    #region Constructors
    public MonoSelector()
    {
        elements = new SelectorElement<T>[0];
    }

    public MonoSelector(params SelectorElement<T>[] elements)
    {
        this.elements = elements;
    }
    #endregion

    #region Methods
    public T DoSelection()
    {
        return elements.RandomSelectOne(x => x.probability).selection;
    }
    #endregion
}
