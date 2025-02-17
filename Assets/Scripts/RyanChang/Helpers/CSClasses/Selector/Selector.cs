using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows for a selection of a certain thing with an assigned likelihood.
/// </summary>
[System.Serializable]
public class Selector<T>
{
    /// <summary>
    /// Defines the selection mode.
    /// </summary>
    public enum SelectionMode
    {
        [Tooltip("Selects just one element.")]
        JustOne,
        [Tooltip("Selects one or no elements.")]
        OneOrNone,
        [Tooltip("Can select many elements, or none.")]
        ManyOrNone,
        [Tooltip("Selects at least one element, if the number of elements is greater than 0.")]
        ManyOrOne,
    }

    [Tooltip("Defines the selection mode.")]
    public SelectionMode selectionMode;

    [Tooltip("The elements to select from.")]
    public SelectorElement<T>[] elements;

    public bool Empty => elements.Length == 0;


    /// <summary>
    /// Performs the selection.
    /// </summary>
    /// <returns>The selection.</returns>
    /// <exception cref="System.ArgumentException">If selection mode is
    /// invalid.</exception>
    public IEnumerable<T> DoSelection()
    {
        switch (selectionMode)
        {
            case SelectionMode.JustOne:
                var rso = new List<T>()
                {
                    elements.RandomSelectOne(x => x.probability).selection
                };
                return rso;
            case SelectionMode.OneOrNone:
                var rson = new List<T>()
                {
                    elements.RandomSelectOneOrNone(x => x.probability).selection
                };
                return rson;
            case SelectionMode.ManyOrNone:
                return elements.RandomSelectMany(x => x.probability).Select(e => e.selection);
            case SelectionMode.ManyOrOne:
                return elements.RandomSelectAtLeastOne(x => x.probability).Select(e => e.selection);
            default:
                throw new System.ArgumentException($"{selectionMode} is not a valid selection.");
        }
    }
}
