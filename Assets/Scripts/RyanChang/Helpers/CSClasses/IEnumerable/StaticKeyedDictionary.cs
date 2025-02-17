using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Provides a base class to allow for easy creation of new dictionaries (with
/// program-defined keys) that are visible to the unity editor; aka a read-only
/// dictionary for unity. Utilizes the <see cref="UnityDictionary{TKey,
/// TValue}"/>
/// </summary>
///
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
[JsonObject(MemberSerialization.OptIn)]
[JsonConverter(typeof(StaticKeyedDictionaryConverter))]
[Serializable]
public abstract class StaticKeyedDictionary<TKey, TValue> :
    IStaticKeyedDictionary<TKey, TValue>
{
    #region Variables
    /// <summary>
    /// The dictionary explicitly used by the editor.
    /// </summary>
    [JsonProperty, SerializeField]
    protected UnityDictionary<TKey, TValue> editorDict = new();

    /// <summary>
    /// The loaded json keys, if any. This can be null if there are no json
    /// keys, or non-null if there are loaded json keys.
    /// </summary>
    private Dictionary<string, TValue> jsonValues = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="StaticKeyedDictionary{TKey, TValue}"/> from the
    /// provided <paramref name="dictionary"/>.
    /// </summary>
    /// <remarks>
    /// This constructor is marked protected as it should only be used by the
    /// JSON converter, and this constructor should not be used normally. See
    /// the documentation for <see cref="ISaveLoadable{T}"/> for more
    /// information on JSON.
    /// </remarks>
    /// <param name="dictionary">The provided dictionary.</param>
    protected StaticKeyedDictionary(IDictionary<TKey, TValue> dictionary)
    {
        editorDict = new(dictionary);
    }

    /// <summary>
    /// Pre-loads any json keys into this <see
    /// cref="StaticKeyedDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <param name="jsonValues">The keys.</param>
    /// <inheritdoc cref="StaticKeyedDictionary{TKey, TValue}(IDictionary{TKey,
    /// TValue})"/>
    protected StaticKeyedDictionary(Dictionary<string, TValue> jsonValues)
    {
        this.jsonValues = jsonValues;
    }
    #endregion

    #region Properties
    #region IReadOnlyDictionary Implementation
    public IEnumerable<TKey> Keys => editorDict.Keys;

    public IEnumerable<TValue> Values => editorDict.Values;

    public int Count => editorDict.Count;

    public bool IsFixedSize => true;

    public bool IsReadOnly => true;

    ICollection IDictionary.Keys => editorDict.Keys;

    ICollection IDictionary.Values => editorDict.Values;

    public bool IsSynchronized => false;

    public object SyncRoot => null;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => editorDict.Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => editorDict.Values;

    object IDictionary.this[object key]
    {
        get => editorDict[key];
        set => editorDict[key] = value;
    }
    #endregion
    #endregion

    #region IStaticKeyedDictionary Implementation
    public void GenerateStaticKeys(UnityEngine.Object targetObject)
    {
        GenerateStaticKeys(targetObject, jsonValues);
        jsonValues = null;
    }

    /// <summary>
    /// Generates static keys while allowing the import of json keys (if
    /// loaded).
    /// </summary>
    /// <param name="jsonValues">The loaded json keys, if any. This can be
    /// null if there are no json keys, or non-null if there are loaded json
    /// keys.</param>
    /// <inheritdoc cref="GenerateStaticKeys(UnityEngine.Object)"/>
    protected abstract void GenerateStaticKeys(UnityEngine.Object targetObject,
        Dictionary<string, TValue> jsonValues);

    /// <inheritdoc cref="LabelFromKey(object)"/>
    public abstract string LabelFromKey(TKey key);

    public string LabelFromKey(object key)
    {
        if (key is TKey genericKey)
        {
            return LabelFromKey(genericKey);
        }

        throw new ArgumentException($"{key} not of correct type ({typeof(TKey)}).");
    }

    protected void LoadJsonValues(IEnumerable<TKey> keys, Func<TKey, string> keyNameFunc)
    {
        foreach (var key in keys)
        {
            if (!editorDict.ContainsKey(key))
            {
                if (jsonValues == null)
                {
                    editorDict[key] = TypeExt.
                        CallParameterlessConstructor<TValue>();
                }
                else
                {
                    editorDict[key] = jsonValues.
                        GetValueOrDefault(
                            keyNameFunc(key),
                            TypeExt.CallParameterlessConstructor<TValue>()
                        );
                }
            }
        }
    }

    public UnityDictionaryErrorCode GetErrorCode() =>
        editorDict.GetErrorCode();

    public void ResetInternalDict(bool force = false) =>
        editorDict.ResetInternalDict(force);

    public void ResetInspectorKVPs(bool force = false) =>
        editorDict.ResetInspectorKVPs(force);
    #endregion

    #region IDictionary Implementation
    public TValue this[TKey key]
    {
        get => editorDict[key];
        set => editorDict[key] = value;
    }

    public bool ContainsKey(TKey key) => editorDict.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value) =>
        editorDict.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
        editorDict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => editorDict.GetEnumerator();

    private const string NOT_SUPPORTED_MSG =
        "StaticKeyedDictionary has a fixed size and is read only.";

    void IDictionary.Add(object key, object value) =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);

    void IDictionary.Clear() =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);
    bool IDictionary.Contains(object key) => ContainsKey((TKey)key);

    IDictionaryEnumerator IDictionary.GetEnumerator() =>
        (IDictionaryEnumerator)GetEnumerator();

    void IDictionary.Remove(object key) =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);

    void ICollection.CopyTo(Array array, int index) =>
        editorDict.CopyTo(array, index);

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);

    bool IDictionary<TKey, TValue>.Remove(TKey key) =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);

    void ICollection<KeyValuePair<TKey, TValue>>.Add(
        KeyValuePair<TKey, TValue> item) =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);

    void ICollection<KeyValuePair<TKey, TValue>>.Clear() =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(
        KeyValuePair<TKey, TValue> item) =>
        editorDict.Contains(item);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        editorDict.CopyTo(array, arrayIndex);

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(
        KeyValuePair<TKey, TValue> item) =>
        throw new NotSupportedException(NOT_SUPPORTED_MSG);
    #endregion

    #region Other Methods
    public override string ToString() => editorDict.ToString();
    #endregion
}
