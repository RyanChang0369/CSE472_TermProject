using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Extensions for coroutines
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public static class CoroutineExt
{
    #region Exclusive
    /// <summary>
    /// Starts <paramref name="coroutine"/>, stopping it first if it is already
    /// running.
    /// </summary>
    /// <param name="enumerator">The coroutine method.</param>
    /// <inheritdoc cref="StopExclusiveCoroutine(MonoBehaviour, ref
    /// Coroutine)"/>
    public static void StartExclusiveCoroutine(this MonoBehaviour behaviour,
        IEnumerator enumerator, ref Coroutine coroutine, bool doError = false)
    {
        if (doError)
        {
            coroutine = coroutine == null
                ? behaviour.StartCoroutine(enumerator)
                : throw new InvalidOperationException(
                    $"Coroutine {coroutine} in behaviour {behaviour} " +
                    "already running."
                );
        }
        else
        {
            StopExclusiveCoroutine(behaviour, ref coroutine, doError);
            coroutine = behaviour.StartCoroutine(enumerator);
        }
    }

    /// <summary>
    /// If <paramref name="coroutine"/> is running, stop it.
    /// </summary>
    /// <param name="behaviour">The behaviour <paramref name="coroutine"/> is
    /// running on.</param>
    /// <param name="coroutine">The coroutine.</param>
    public static void StopExclusiveCoroutine(this MonoBehaviour behaviour,
        ref Coroutine coroutine, bool doError = false)
    {
        if (coroutine != null)
        {
            behaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
        else if (doError)
        {
            throw new InvalidOperationException(
                $"Coroutine {coroutine} in behaviour {behaviour} " +
                "is not running."
            );
        }
    }
    #endregion

    #region Callbacks
    #region Predicate
    /// <summary>
    /// Creates a callback coroutine that will perform <paramref
    /// name="callback"/> once <paramref name="predicate"/> is satisfied.
    /// </summary>
    /// <param name="behaviour">The behavior to attach the coroutine to.</param>
    /// <param name="predicate">Determines when <paramref name="callback"/>
    /// should be called.</param>
    /// <param name="callback">The action to perform once <paramref
    /// name="predicate"/> is satisfied.</param>
    public static void AfterPredicate(this MonoBehaviour behaviour,
        Func<bool> predicate, Action callback)
    {
        static IEnumerator WaitForPredicate(Func<bool> predicate,
            Action callback)
        {
            yield return new WaitUntil(predicate);
            callback();
        }

        var enumerator = WaitForPredicate(predicate, callback);

        if (!Application.isPlaying)
        {
            // If executing in editor, require the enumerator to be registered.
            GameObjectHandle.Instance.AddEditorCoroutine(enumerator);
        }

        behaviour.StartCoroutine(enumerator);
    }

    /// <inheritdoc cref="AfterPredicate(MonoBehaviour, Func{bool}, Action)"/>
    /// <remarks>Uses the <see cref="GameObjectHandle.Instance"/>.</remarks>
    public static void AfterPredicate(Func<bool> predicate, Action callback) =>
        GameObjectHandle.Instance.AfterPredicate(predicate, callback);
    #endregion

    #region Time
    /// <summary>
    /// Creates a callback coroutine that will perform <paramref
    /// name="callback"/> once <paramref name="seconds"/> have passed.
    /// </summary>
    /// <inheritdoc cref="AfterPredicate(MonoBehaviour, Func{bool}, Action)"/>
    /// <param name="seconds">Number of seconds before <paramref
    /// name="callback"/> is called.</param>
    /// <param name="realTime">Whether or not the coroutine should run in
    /// realtime (if true) or scaled time (if false).</param>
    public static void AfterSeconds(this MonoBehaviour behaviour, float seconds,
        bool realTime, Action callback)
    {
        static IEnumerator WaitForTime(float seconds, bool realtime,
            Action callback)
        {
            if (realtime)
                yield return new WaitForSecondsRealtime(seconds);
            else
                yield return new WaitForSeconds(seconds);
            callback();
        }

        var enumerator = WaitForTime(seconds, realTime, callback);

        if (!Application.isPlaying)
        {
            // If executing in editor, require the enumerator to be registered.
            GameObjectHandle.Instance.AddEditorCoroutine(enumerator);
        }

        behaviour.StartCoroutine(enumerator);
    }

    /// <inheritdoc cref="AfterSeconds(MonoBehaviour, float, bool, Action)"/>
    /// <remarks>Uses the <see cref="GameObjectHandle.Instance"/>.</remarks>
    public static void AfterSeconds(float seconds, bool realTime,
        Action callback) =>
        GameObjectHandle.Instance.AfterSeconds(seconds, realTime, callback);
    #endregion

    #region Fixed Update
    /// <summary>
    /// Creates a callback coroutine that will perform <paramref
    /// name="callback"/> after one fixed update.
    /// </summary>
    /// <inheritdoc cref="AfterPredicate(MonoBehaviour, Func{bool}, Action)"/>
    public static void AfterFixedUpdate(this MonoBehaviour behaviour,
        Action callback)
    {
        static IEnumerator WaitForFixedUpdate(Action callback)
        {
            yield return new WaitForFixedUpdate();
            callback();
        }

        if (!Application.isPlaying)
        {
            throw new InvalidOperationException(
                "FixedUpdate not updated in edit mode."
            );
        }

        var enumerator = WaitForFixedUpdate(callback);
        behaviour.StartCoroutine(enumerator);
    }

    /// <inheritdoc cref="AfterFixedUpdate(Action)(MonoBehaviour, float, bool,
    /// Action)"/>
    /// <remarks>Uses the <see cref="GameObjectHandle.Instance"/>.</remarks>
    public static void AfterFixedUpdate(Action callback) =>
        GameObjectHandle.Instance.AfterFixedUpdate(callback);
    #endregion

    #region Update
    /// <summary>
    /// Creates a callback coroutine that will perform <paramref
    /// name="callback"/> after one update.
    /// </summary>
    /// <inheritdoc cref="AfterPredicate(MonoBehaviour, Func{bool}, Action)"/>
    public static void AfterEndOfFrame(this MonoBehaviour behaviour,
        Action callback)
    {
        static IEnumerator WaitForUpdate(Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback();
        }

        if (!Application.isPlaying)
        {
            throw new InvalidOperationException(
                "FixedUpdate not updated in edit mode."
            );
        }

        var enumerator = WaitForUpdate(callback);
        behaviour.StartCoroutine(enumerator);
    }

    /// <inheritdoc cref="AfterEndOfFrame(Action)(MonoBehaviour, float, bool,
    /// Action)"/>
    /// <remarks>Uses the <see cref="GameObjectHandle.Instance"/>.</remarks>
    public static void AfterEndOfFrame(Action callback) =>
        GameObjectHandle.Instance.AfterEndOfFrame(callback);
    #endregion
    #endregion
}