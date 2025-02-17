using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a gameobject that is guaranteed to exist at all times, through the
/// use of <see cref="Instance"/>. Can be used as an always-available instance
/// of an arbitrary game object.
/// </summary>
///
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
[DefaultExecutionOrder(-42069)]     // < !IMPORTANT! funni number
[ExecuteAlways]
public class GameObjectHandle : MonoBehaviour
{
    #region Instance
    /// <summary>
    /// The static reference to a(n) 
    /// <see cref="GameObjectHandle"/> instance.
    /// </summary>
    private static GameObjectHandle instance;

    /// <inheritdoc cref="instance"/>
    /// <remarks>
    /// The instance is automatically generated upon request (because properties
    /// are awesome, see <see
    /// href="https://learn.microsoft.com/en-us/dotnet/csharp/properties"/>).
    /// Old instances of the <see cref="GameObjectHandle"/> are pruned by <see
    /// cref="AutoPurge(bool)"/>, which is called basically whenever this script
    /// can receive an update.
    /// </remarks>
    public static GameObjectHandle Instance
    {
        get
        {
            if (!instance)
            {
                GameObject temp = new(RNGExt.RandomHashString(4));
                instance = temp.AddComponent<GameObjectHandle>();
            }

            return instance;
        }
    }
    #endregion

    #region AutoPurge
    private void OnEnable() => AutoPurge(true);
    private void OnDisable() => AutoPurge(true);

    /// <summary>
    /// Logic for whether or not this handle should be destroyed (ie when the
    /// instance no longer refers to this object).
    /// </summary>
    private void AutoPurge(bool playing)
    {
        if (instance != this && Application.IsPlaying(this) == playing)
        {
            if (playing)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }
    #endregion

    #region Editor Coroutine
    private Queue<IEnumerator> editorCoroutines = new();

    /// <summary>
    /// Registers the coroutine as an editor coroutine. Editor coroutines can
    /// execute in the editor.
    /// </summary>
    /// <param name="coroutine"></param>
    public void AddEditorCoroutine(IEnumerator coroutine) =>
        editorCoroutines.Enqueue(coroutine);

    private void ProcessEditorCoroutine()
    {
        if (editorCoroutines.TryDequeue(out IEnumerator cr) && cr != null)
        {
            cr.MoveNext();
        }
    }
    #endregion

    #region Update Loop
    private void Update()
    {
        AutoPurge(false);

        if (!this.IsUnityNull() && !Application.IsPlaying(this))
        {
            ProcessEditorCoroutine();
        }
    }
    #endregion
}