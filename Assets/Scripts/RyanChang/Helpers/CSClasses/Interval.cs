using System;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A customizable, randomizable stopwatch that increments with calls to <see
/// cref="UpdateInterval(float)"/> and related functions. Automatically resets
/// when the time reaches a set time (determined by <see cref="pattern"/>).
/// Designed to be attached to a monobehaviour, to allow for a quick
/// implementation of a timer that is designed to be updated with the <see
/// cref="MonoBehaviour.Update"/> and <see cref="MonoBehaviour.FixedUpdate"/>
/// functions.
/// </summary>
///
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class Interval
{
    #region Variables
    /// <summary>
    /// The RNG pattern used.
    /// </summary>
    [Tooltip("The RNG pattern used.")]
    [FormerlySerializedAs("range")]
    [JsonProperty]
    public RNGPattern pattern;

    /// <summary>
    /// The timer used for the interval.
    /// </summary>
    [Tooltip("The timer used for the interval.")]
    [ReadOnly]
    public float timer;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates an interval with the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    public Interval(RNGPattern pattern)
    {
        this.pattern = pattern;
    }

    /// <summary>
    /// Creates an interval with a single range.
    /// </summary>
    /// <param name="value">The value to set to.</param>
    public Interval(float value) : this(new RNGPattern(value))
    { }

    /// <summary>
    /// Creates a new interval with a linear range with max and min.
    /// </summary>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximal value</param>
    public Interval(float min, float max) : this(new RNGPattern(min, max))
    { }
    #endregion

    #region Methods
    /// <summary>
    /// Updates the interval.
    /// </summary>
    /// <param name="deltaTime">Amount of time that has passed since last
    /// update.</param>
    /// <returns>True if the interval has reached its internal timer. Most
    /// likely Time.deltaTime.</returns>
    public bool UpdateInterval(float deltaTime)
    {
        if (timer >= pattern.Select())
        {
            pattern.Reset();
            timer = 0;
            return true;
        }
        else
        {
            timer += deltaTime;
            return false;
        }
    }

    /// <summary>
    /// Ensure that the next call to UpdateInterval returns true by setting the
    /// timer to its maximum selected value.
    /// </summary>
    public void WindToEnd()
    {
        timer = pattern.Select() + 1;
    }

    /// <summary>
    /// Reset timer to zero without resetting the timer's time selection.
    /// </summary>
    public void WindToStart()
    {
        timer = 0;
    }

    /// <summary>
    /// Resets the timer and selects a new maximum time.
    /// </summary>
    public void Restart()
    {
        WindToStart();
        pattern.Reset();
    }
    #endregion
}
