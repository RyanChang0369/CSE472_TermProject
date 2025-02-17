using System.Collections;
using System.Collections.Generic;


public abstract class PopulationTree<T> : ICollection<T>
{
    private HashSet<T> branches = new();

    #region Properties
    public int Depth { get; private set; }

    public int Population { get; private set; }

    #region ICollection Implementation
    public int Count => ((ICollection<T>)branches).Count;

    public bool IsReadOnly => ((ICollection<T>)branches).IsReadOnly;
    #endregion
    #endregion

    #region Methods
    #region ICollection Implementation
    public void Add(T item)
    {
        ((ICollection<T>)branches).Add(item);
    }

    public void Clear()
    {
        ((ICollection<T>)branches).Clear();
    }

    public bool Contains(T item)
    {
        return ((ICollection<T>)branches).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)branches).CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)branches).GetEnumerator();
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)branches).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)branches).GetEnumerator();
    }
    #endregion
    #endregion
}

// public class PopulationTree
// {
    
// }