using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Randomly selects multiple components.
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class MultiSelector<T>
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
    public MultiSelector()
    {
        elements = new SelectorElement<T>[0];
    }

    public MultiSelector(params SelectorElement<T>[] elements)
    {
        this.elements = elements;
    }
    #endregion

    #region Methods
    public IEnumerable<T> DoSelection()
    {
        return elements.RandomSelectMany(x => x.probability)
            .Select(s => s.selection);
    }
    #endregion
}
