using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// Priority queue with a min heap. Allows for updates to any priority.
/// Assumes all values are unique.
/// </summary>
/// <typeparam name="TVal"></typeparam>
/// <remarks>Authors: Ryan Chang (2022)</remarks>
public class PriorityQueue<TKey, TVal> where TKey : IComparable<TKey>
{
    #region Structs
    /// <summary>
    /// Priority queue element.
    /// </summary>
    public struct PriorityElement : IComparable<PriorityElement>, IComparable
    {
        #region Fields
        /// <summary>
        /// Priority of the element. Smaller = higher priority.
        /// </summary>
        public TKey priority;

        /// <summary>
        /// The element's value.
        /// </summary>
        public TVal value;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new priority queue element.
        /// </summary>
        /// <param name="priority">The priority to assign to this.</param>
        /// <param name="value">The element's value.</param>
        public PriorityElement(TKey priority, TVal value)
        {
            this.priority = priority;
            this.value = value;
        }
        #endregion

        #region Methods
        public override readonly string ToString()
        {
            return $"PQElement: priority {priority}, {value}";
        }

        public int CompareTo(PriorityElement other)
        {
            return priority.CompareTo(other.priority);
        }

        public int CompareTo(object obj)
        {
            return obj != null && obj is PriorityElement other ? CompareTo(other) : 1;
        }
        #endregion
    }

    /// <summary>
    /// Used to locate <see cref="PriorityElement"/>.
    /// </summary>
    private struct LocatorElement : IComparable
    {
        #region Fields
        /// <summary>
        /// Element this locator is pointing at.
        /// </summary>
        public PriorityElement pqElement;

        /// <summary>
        /// A counter value to do tie breaks.
        /// </summary>
        private readonly int tieBreaker;

        /// <summary>
        /// The static counter value. See <paramref name="tieBreaker"/>.
        /// </summary>
        private static int tieBreakerCounter = 0;

        /// <summary>
        /// Index of <paramref name="pqElement"/> in <paramref name="data"/>.
        /// </summary>
        public int index;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new locator.
        /// </summary>
        /// <param name="pqElement">The priority queue element this points to.</param>
        /// <param name="index">Index of <paramref name="pqElement"/> in <paramref name="data"/>.</param>
        public LocatorElement(PriorityElement pqElement, int index)
        {
            this.pqElement = pqElement;
            this.index = index;

            tieBreaker = tieBreakerCounter;
            tieBreakerCounter++;
        }
        #endregion

        #region Methods
        public readonly int CompareTo(object other)
        {
            if (other != null && other is LocatorElement otherLocator)
            {
                int compared = pqElement.CompareTo(otherLocator);

                return compared switch
                {
                    0 => tieBreaker.CompareTo(otherLocator.tieBreaker),
                    _ => compared,
                };
            }

            return 1;
        }

        public override readonly string ToString()
        {
            return $"LocatorElement: index {index}, {pqElement}";
        } 
        #endregion
    }
    #endregion

    #region Fields
    /// <summary>
    /// Underlying container for this queue.
    /// </summary>
    private readonly List<PriorityElement> data = new();

    /// <summary>
    /// Dictionary to locate the data to update.
    /// </summary>
    private readonly Dictionary<TVal, LocatorElement> locator = new();
    #endregion

    #region Properties
    /// <summary>
    /// Objects in this queue.
    /// </summary>
    public int Count => data.Count;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new empty <see cref="PriorityQueue{T}"/>.
    /// </summary>
    public PriorityQueue()
    {
        data = new();
        locator = new();
    }

    /// <summary>
    /// Creates a <see cref="PriorityQueue{T}"/> with the specified elements.
    /// </summary>
    /// <param name="elements">The collection of elements to add.</param>
    public PriorityQueue(IEnumerable<Tuple<TKey, TVal>> elements)
    {
        foreach (var elem in elements)
        {
            Enqueue(elem.Item1, elem.Item2);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Enqueues a value with a priority.
    /// </summary>
    /// <param name="priority">Priority of the value. Smaller = more
    /// priority.</param>
    /// <param name="value">Value to insert.</param>
    public void Enqueue(TKey priority, TVal value)
    {
        var newPQElem = new PriorityElement(priority, value);
        data.Add(newPQElem);
        locator.Add(value, new LocatorElement(newPQElem, Count - 1));
        PercolateUp(Count - 1);
    }

    /// <summary>
    /// Returns the element at the front of the queue.
    /// </summary>
    /// <returns></returns>
    public TVal Peek() => data[0].value;

    /// <summary>
    /// Removes and returns the element with its value and priority at the front
    /// of the queue.
    /// </summary>
    /// <returns>A tuple of (priority, value).</returns>
    public PriorityElement Dequeue()
    {
        TVal value = data[0].value;
        var valueElement = data[0];

        // Get values of the datas.
        var currData = data[0];
        var swapData = data[Count - 1];

        // Get values of the locators so they can be updated later.
        var currLocator = locator[currData.value];
        var swapLocator = locator[swapData.value];
        currLocator.index = Count - 1;  // Ends up in the end of list
        swapLocator.index = 0;          // Ends up in the front of list

        // Swap elements so that the top is now at the bottom
        data[0] = swapData;
        data[Count - 1] = currData;

        // Reassign locators
        locator[currData.value] = currLocator;
        locator[swapData.value] = swapLocator;

        // Pop last element.
        data.RemoveAt(Count - 1);
        locator.Remove(value);

        // Swapped element needs to be correctly ordered.
        PercolateDown(0);

        return valueElement;
    }

    /// <summary>
    /// Removes and returns the elements value at the front of the queue.
    /// </summary>
    /// <returns>The value that was dequeued.</returns>
    public TVal DequeueValue() => Dequeue().value;

    /// <summary>
    /// Tries to dequeue an element.
    /// </summary>
    /// <param name="element">The retrieved priority element if an element was
    /// able to be dequeued; otherwise undetermined.</param>
    /// <returns>True if dequeue was successful, false otherwise. If false, the
    /// value of <paramref name="element"/> is undefined.</returns>
    public bool TryDequeue(out PriorityElement element)
    {
        if (Count > 0)
        {
            element = Dequeue();
            return true;
        }
        else
        {
            element = default;
            return false;
        }
    }

    /// <summary>
    /// Tries to dequeue a value.
    /// </summary>
    /// <param name="value">The retrieved value if an element was able to be
    /// dequeued; otherwise undetermined.</param>
    /// <returns>True if dequeue was successful, false otherwise. If false, the
    /// value of <paramref name="value"/> is undefined.</returns>
    public bool TryDequeue(out TVal value)
    {
        bool retVal = TryDequeue(out PriorityElement elem);
        value = elem.value;
        return retVal;
    }

    /// <summary>
    /// Updates the key.
    /// </summary>
    /// <param name="newPriority">The new priority to assign.</param>
    /// <param name="key">The value to assign the new priority to.</param>
    /// <returns>True if update successful.</returns>
    public bool Update(TKey newPriority, TVal key)
    {
        if (!locator.ContainsKey(key))
            return false;

        var currLocator = locator[key];
        var swapLocator = locator[data[0].value];

        var currData = data[currLocator.index];
        var swapData = data[0];
        data[currLocator.index] = swapData;
        data[0] = currData;

        swapLocator.index = currLocator.index;
        currLocator.index = 0;
        locator[key] = currLocator;
        locator[swapData.value] = swapLocator;

        Dequeue();

        Enqueue(newPriority, currLocator.pqElement.value);

        PercolateDown(0);
        PercolateUp(swapLocator.index);

        return true;
    }

    /// <summary>
    /// Continuously dequeues values until there's no values left.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TVal> Expunge()
    {
        while (TryDequeue(out TVal value))
            yield return value;
    }

    /// <summary>
    /// Given an <paramref name="input"/>, create a <see
    /// cref="PriorityQueue{TKey, TVal}"/>, then immediately dequeues the sorted
    /// values until there's no values left.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns></returns>
    public IEnumerable<TVal> Slurp(IEnumerable<Tuple<TKey, TVal>> input)
    {
        PriorityQueue<TKey, TVal> pq = new(input);
        return pq.Expunge();
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Returns the left child index of the element at <paramref
    /// name="currentIndex"/>.
    /// </summary>
    /// <param name="currentIndex">Current index of an element.</param>
    /// <returns>The left child index, if it exists. Otherwise throws
    /// IndexOutOfRangeException.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the left child of
    /// the element at <paramref name="currentIndex"/> does not
    /// exist.</exception>
    private int GetLeftChildIndex(int currentIndex)
    {
        int childI = 2 * currentIndex + 1;

        if (childI < Count)
            return childI;
        else
            throw new IndexOutOfRangeException(
                $"Child index {childI} of current index {currentIndex} is out " +
                $"of range of list with length {Count}."
                );
    }

    /// <summary>
    /// Returns the right child index of the element at <paramref
    /// name="currentIndex"/>.
    /// </summary>
    /// <param name="currentIndex">Current index of an element.</param>
    /// <returns>The right child index, if it exists. Otherwise throws
    /// IndexOutOfRangeException.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the right child
    /// of the element at <paramref name="currentIndex"/> does not
    /// exist.</exception>
    private int GetRightChildIndex(int currentIndex)
    {
        int childI = 2 * currentIndex + 2;

        if (childI < Count)
            return childI;
        else
            throw new IndexOutOfRangeException(
                $"Child index {childI} of current index {currentIndex} is out " +
                $"of range of list with length {Count}."
                );
    }

    /// <summary>
    /// Returns the index of the smallest child of the element at <paramref
    /// name="currentIndex"/>.
    /// </summary>
    /// <param name="currentIndex">Current index of an element.</param>
    /// <returns>The smallest child index, if it exists. Otherwise throws
    /// IndexOutOfRangeException.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the smallest
    /// child of the element at <paramref name="currentIndex"/> does not
    /// exist.</exception>
    private int GetMinChildIndex(int currentIndex)
    {
        int childIR = 2 * currentIndex + 2;
        int childIL = childIR - 1;

        if (childIL < Count && childIR < Count)
        {
            int comparison = data[childIL].priority.CompareTo(
                data[childIR].priority
            );
            return (comparison < 0) ? childIL : childIR;
        }
        else if (childIL < Count)
        {
            return childIL;
        }
        else if (childIR < Count)
        {
            return childIR;
        }
        else
        {
            return -1;
            // throw new IndexOutOfRangeException(
            //     $"Child indexes {childIR} and {childIL} of current index {currentIndex} is out " +
            //     $"of range of list with length {Count}."
            //     );
        }
    }

    /// <summary>
    /// Returns the parent index of the element at <paramref name="childIndex"/>.
    /// </summary>
    /// <param name="childIndex">Current index of an element.</param>
    /// <returns>The parent index, if it exists. Otherwise throws IndexOutOfRangeException.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the parent of the element at 
    /// <paramref name="childIndex"/> does not exist.</exception>
    private int GetParentIndex(int childIndex)
    {
        if (childIndex == 0)
            throw new IndexOutOfRangeException(
                $"Parent index {0} of child index {childIndex} is out " +
                $"of range of list with length {Count}."
                );

        int parentI = Mathf.FloorToInt((childIndex - 1) / 2);

        if (parentI < Count && 0 <= parentI)
            return parentI;
        else
            throw new IndexOutOfRangeException(
                $"Parent index {parentI} of child index {childIndex} is out " +
                $"of range of list with length {Count}."
                );
    }

    /// <summary>
    /// Percolates up the value at index to its valid spot in the heap.
    /// </summary>
    /// <param name="index">Index to start the percolation at.</param>
    private void PercolateUp(int index)
    {
        while (index > 0 && index < Count)
        {
            var parentI = GetParentIndex(index);

            if (parentI == index)
                break;

            if (data[index].priority.CompareTo(data[parentI].priority) < 0)
            {
                // Parent's child has more priority, Swap elements.
                (data[parentI], data[index]) = (data[index], data[parentI]);

                // Update locators.
                var lastK = locator.Keys.Last();
                var parentLocator = locator[data[parentI].value];
                var currentLocator = locator[data[index].value];
                parentLocator.index = parentI;
                currentLocator.index = index;
                locator[data[parentI].value] = parentLocator;
                locator[data[index].value] = currentLocator;
            }

            index = parentI;
        }
    }

    /// <summary>
    /// Percolates down the value at index to its valid spot in the heap.
    /// </summary>
    /// <param name="index">Index to start the percolation at.</param>
    private void PercolateDown(int index)
    {
        while (index >= 0 && index < Count)
        {
            var childI = GetMinChildIndex(index);

            if (childI >= 0 &&
                data[childI].priority.CompareTo(data[index].priority) < 0)
            {
                // Child has more priority. Swap elements.
                (data[childI], data[index]) = (data[index], data[childI]);

                // Update locators.
                var childLocator = locator[data[childI].value];
                var currentLocator = locator[data[index].value];
                childLocator.index = childI;
                currentLocator.index = index;
                locator[data[childI].value] = childLocator;
                locator[data[index].value] = currentLocator;
            }

            index = childI;
        }
    }
    #endregion
}
