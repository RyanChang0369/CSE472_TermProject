using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[Serializable]
public class FixedList<T> : IEnumerable<T>, IList<T>
{
    public List<T> internalList;

    public int Count => ((ICollection<T>)internalList).Count;

    public bool IsReadOnly => ((ICollection<T>)internalList).IsReadOnly;

    public T this[int index] { get => ((IList<T>)internalList)[index]; set => ((IList<T>)internalList)[index] = value; }

    public FixedList()
    {
        internalList = new();
    }

    public FixedList(List<T> list)
    {
        internalList = list;
    }

    public FixedList(IEnumerable<T> collection)
    {
        internalList = collection.ToList();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)internalList).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)internalList).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return ((IList<T>)internalList).IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        ((IList<T>)internalList).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ((IList<T>)internalList).RemoveAt(index);
    }

    public void Add(T item)
    {
        ((ICollection<T>)internalList).Add(item);
    }

    public void Clear()
    {
        ((ICollection<T>)internalList).Clear();
    }

    public bool Contains(T item)
    {
        return ((ICollection<T>)internalList).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)internalList).CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)internalList).Remove(item);
    }
}