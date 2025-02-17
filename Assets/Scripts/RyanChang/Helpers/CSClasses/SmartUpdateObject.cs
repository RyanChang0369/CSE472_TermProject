using UnityEngine;
using System;

[Serializable]
public class SmartUpdateObject<T>
{
    /// <summary>
    /// Current value
    /// </summary>
    [Tooltip("Current value. Can be used to initialize this object with Unity.")]
    [SerializeField]
    private T current;

    /// <summary>
    /// Previous value
    /// </summary>
    private T previous;

    /// <summary>
    /// Gets the current value
    /// </summary>
    public T Value => current;

    /// <summary>
    /// Represents the comparison between current and previous. If this returns true,
    /// then the previous value will be replaced by the current. For example, making
    /// this comparison be current =/= previous only returns true when you know that
    /// current has been changed.
    /// </summary>
    /// <param name="a">Value a</param>
    /// <param name="b">Value b</param>
    /// <returns></returns>
    public delegate bool Comparison(T a, T b);

    /// <summary>
    /// The comparison to make
    /// </summary>
    private Comparison comparison;

    /// <summary>
    /// Default comparison
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>True if a =/= b, false otherwise.</returns>
    private static bool DefaultComparison(T a, T b)
    {
        return !a.Equals(b);
    }

    /// <summary>
    /// Used by Unity to create a default SmartUpdateObject using initial value.
    /// </summary>
    private SmartUpdateObject()
    {
        if (comparison == null)
        {
            comparison = new Comparison(DefaultComparison);
        }
    }

    /// <summary>
    /// Creates a smart update object
    /// </summary>
    /// <param name="value">The value to store</param>
    /// <param name="comparison">The comparison to make</param>
    public SmartUpdateObject(T value, Comparison comparison)
    {
        current = value;
        previous = value;
        this.comparison = comparison;
    }

    /// <summary>
    /// Updates the current value to newValue and checks current and previous with the comparison,
    /// updating previous if not. Returns the return value of comparison.
    /// </summary>
    /// <param name="newValue">The value to compare</param>
    /// <returns>True if an update has occured. Use this boolean to do whatever other updates
    /// you need to do.</returns>
    public bool Update(T newValue)
    {
        current = newValue;
        bool doUpdate = comparison(current, previous);

        if (doUpdate)
        {
            previous = newValue;
        }

        return doUpdate;
    }

    /// <summary>
    /// Updates the previous value with the current value if comparison returns true.
    /// Returns the return value of comparison. ONLY USE THIS METHOD IF CURRENT HAS BEEN
    /// EXPOSED TO UNITY AND IF YOU ARE USING IT AS SUCH. Otherwise, nothing will happen.
    /// </summary>
    /// <returns>True if an update has occured. Use this boolean to do whatever other updates
    /// you need to do.</returns>
    public bool Update()
    {
        return Update(current);
    }
}