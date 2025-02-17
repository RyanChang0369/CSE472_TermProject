using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a duration of time, bundling both maximum time and elapsed time
/// for an event in one class.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class Duration
{
    #region Enums
    [Flags]
    public enum Options
    {
        None = 0,
        Repeat = 1,
        UnscaledTime = 2,
    }
    #endregion

    #region Variables
    [Tooltip("How long is this duration?")]
    public float maxTime = 1f;

    [HideInInspector]
    public float elapsed = 0f;

    [HideInInspector]
    private Coroutine callbackCR;

    [HideInInspector]
    private MonoBehaviour callbackMB;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new duration that will last for <paramref name="duration"/>
    /// seconds long.
    /// </summary>
    /// <param name="duration">Amount of time, in seconds, to set the duration
    /// to.</param>
    public Duration(float duration)
    {
        maxTime = duration;
    }
    #endregion

    #region Properties
    /// <summary>
    /// True if the elapsed time is greater than the maximal time.
    /// </summary>
    public bool IsDone => elapsed >= maxTime;

    /// <summary>
    /// What percentage is this duration done by?
    /// </summary>
    public float Percent => IsDone ? 1 : elapsed / maxTime;

    /// <summary>
    /// Checks if a callback is currently running.
    /// </summary>
    public bool CallbackActive => callbackCR != null;
    #endregion

    /// <summary>
    /// Increments <see cref="elapsed"/> by the specified <paramref
    /// name="deltaTime"/>.
    /// </summary>
    /// <param name="deltaTime">How much to increment <see cref="elapsed"/>
    /// by.</param>
    /// <param name="resetIfDone">If true, resets the duration</param>
    /// <returns>The new value of <see cref="IsDone"/>.</returns>
    public bool Increment(float deltaTime, bool resetIfDone)
    {
        elapsed += deltaTime;
        elapsed = Mathf.Clamp(elapsed, 0, maxTime);

        if (IsDone && resetIfDone)
        {
            ResetIfDone();
            return true;
        }

        return IsDone;
    }

    /// <summary>
    /// Increments <see cref="elapsed"/> by <see cref="Time.deltaTime"/>. Use
    /// this in Update() functions.
    /// </summary>
    /// <inheritdoc cref="Increment(float, bool)"/>
    public bool IncrementUpdate(bool resetIfDone)
    {
        return Increment(Time.deltaTime, resetIfDone);
    }

    /// <summary>
    /// Increments <see cref="elapsed"/> by <see cref="Time.fixedDeltaTime"/>.
    /// Use this in FixedUpdate() functions.
    /// </summary>
    /// <inheritdoc cref="Increment(float, bool)"/>
    public bool IncrementFixedUpdate(bool resetIfDone)
    {
        return Increment(Time.fixedDeltaTime, resetIfDone);
    }

    /// <summary>
    /// Resets the value of <see cref="elapsed"/>, if the duration is done. This
    /// doesn't do anything if the timer is not done, however.
    /// </summary>
    /// <returns>True if <see cref="IsDone"/> was true when the function was
    /// ran. False otherwise.</returns>
    public bool ResetIfDone()
    {
        if (IsDone)
        {
            Reset();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the value of <see cref="elapsed"/> to zero, regardless of whether
    /// or not the timer is done.
    /// </summary>
    public void Reset() => elapsed = 0;

    /// <summary>
    /// Sets the value of <see cref="elapsed"/> to <see cref="maxTime"/>, if the
    /// duration is not done.
    /// </summary>
    /// <returns>False if <see cref="IsDone"/> is already true, true
    /// otherwise.</returns>
    public bool Finish()
    {
        if (!IsDone)
        {
            elapsed = maxTime;
            return true;
        }

        return false;
    }

    #region Callback Methods
    /// <summary>
    /// Creates a coroutine on <paramref name="host"/> that calls <paramref
    /// name="callback"/> once <see cref="maxTime"/> has passed. This does not
    /// affect any values within this <see cref="Duration"/>. Only one callback
    /// per instance of <see cref="Duration"/> is allowed to be running at a
    /// time.
    /// </summary>
    /// <param name="host">The behavior to attach the coroutine to.</param>
    /// <param name="callback">The callback to perform.</param>
    /// <param name="unscaledTime">Whether or not to use unscaled time.</param>
    /// <param name="options">The options to include with the callback.</param>
    /// <returns>False if another callback is running. True otherwise.</returns>
    public bool CreateCallback(MonoBehaviour host, Action callback,
        Options options = Options.None)
    {
        if (CallbackActive)
            return false;

        callbackMB = host;
        callbackCR = host.StartCoroutine(WaitUntilDone_CR(
            callback, options
        ));

        return true;
    }

    /// <summary>
    /// A static callback create method that binds a callback to a <paramref
    /// name="host"/>. Unlike <see cref="CreateCallback(MonoBehaviour, Action,
    /// Options)"/>, there is no limit to the amount of created callbacks
    /// running at the same time (as this method does not operate on instances
    /// of <see cref="Duration"/>).
    /// </summary>
    /// <remarks>
    /// The callback will be called once a fixed update is available (using <see
    /// cref="WaitForFixedUpdate"/>).
    /// </remarks>
    /// <inheritdoc cref="CreateCallback(MonoBehaviour, Action, Options)"/>
    public static void CreateFixedUpdateCallback(MonoBehaviour host,
        Action callback)
    {
        host.StartCoroutine(WaitForFixedUpdate_CR(callback));
    }

    /// <inheritdoc cref="CreateFixedUpdateCallback(MonoBehaviour, Action)"/>
    /// <remarks>
    /// The callback will be called once the current frame has ended (using <see
    /// cref="WaitForEndOfFrame"/>).
    /// </remarks>
    public static void CreateEndOfFrameCallback(MonoBehaviour host,
        Action callback)
    {
        host.StartCoroutine(WaitForEndOfFrame_CR(callback));
    }

    /// <summary>
    /// Clears the callback.
    /// </summary>
    /// <returns>True if the callback existed and was removed, false
    /// otherwise.</returns>
    public bool ClearCallback()
    {
        if (CallbackActive)
        {
            callbackMB.StopCoroutine(callbackCR);
            callbackCR = null;
            callbackMB = null;
            return true;
        }

        return false;
    }

    #region Coroutines (Members)
    private IEnumerator WaitUntilDone_CR(Action callback, Options options)
    {
        do
        {
            if (options.HasFlag(Options.UnscaledTime))
                yield return new WaitForSecondsRealtime(maxTime);
            else
                yield return new WaitForSeconds(maxTime);

            if (callback == null)
                yield break;

            callback();
        } while (options.HasFlag(Options.Repeat));

        callbackCR = null;
    }

    private static IEnumerator WaitForFixedUpdate_CR(Action callback)
    {
        yield return new WaitForFixedUpdate();
        callback();
    }

    private static IEnumerator WaitForEndOfFrame_CR(Action callback)
    {
        yield return new WaitForEndOfFrame();
        callback();
    }
    #endregion
    #endregion
}
