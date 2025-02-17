using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A dictionary of durations, all of which must be done in order for the
/// multi duration to complete.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
public class MultiDuration : MonoBehaviour
{
    #region Variables
    private Dictionary<string, Duration> durations = new();
    #endregion

    #region Properties
    /// <summary>
    /// True if all internal durations have completed.
    /// </summary>
    public bool AllDone => durations.Count <= 0;
    #endregion

    #region Methods
    /// <summary>
    /// Inserts a duration into the internal durations with the specified key.
    /// </summary>
    /// <param name="key">Key of the duration.</param>
    /// <param name="duration">Duration to add.</param>
    public void InsertDuration(string key, Duration duration)
    {
        durations[key] = duration;
    }

    /// <summary>
    /// Inserts a new duration that's <paramref name="time"/> seconds long with
    /// the specified key.
    /// </summary>
    /// <param name="key">Key of the duration.</param>
    /// <param name="time">MaxTime for the duration.</param>
    public void InsertDelay(string key, float time)
    {
        InsertDuration(key, new(time));
    }

    /// <summary>
    /// Determines if a specific duration with the specified key is done.
    /// </summary>
    /// <param name="key">Key of the duration.</param>
    /// <returns></returns>
    public bool IsDone(string key)
    {
        return !durations.ContainsKey(key) || durations[key].IsDone;
    }

    /// <summary>
    /// Increments <see cref="elapsed"/> by the specified <paramref
    /// name="deltaTime"/>.
    /// </summary>
    /// <param name="deltaTime">How much to increment <see cref="elapsed"/>
    /// by.</param>
    /// <returns>The new value of <see cref="AllDone"/>.</returns>
    public bool Increment(float deltaTime)
    {
        foreach (var duration in durations)
        {
            duration.Value.Increment(deltaTime, false);
        }

        Cull();

        return AllDone;
    }

    /// <inheritdoc cref="Duration.IncrementUpdate(bool)"/>
    public bool IncrementUpdate()
    {
        return Increment(Time.deltaTime);
    }

    /// <inheritdoc cref="Duration.IncrementFixedUpdate(bool)"/>
    public bool IncrementFixedUpdate()
    {
        return Increment(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Clears all internal durations.
    /// </summary>
    public void Reset()
    {
        foreach (var duration in durations)
        {
            duration.Value.Reset();
        }

        durations.Clear();
    }
    #endregion

    #region Helpers
    private void Cull()
    {
        durations = durations
            .Where(d => !d.Value.IsDone)
            .ToDictionary(d => d.Key, d => d.Value);
    }
    #endregion
}