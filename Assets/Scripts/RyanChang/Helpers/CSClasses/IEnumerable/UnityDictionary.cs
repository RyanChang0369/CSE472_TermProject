using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Used to display a dictionary in the unity inspector. Use if you want to
/// allow the user to modify a dictionary that won't be changed anywhere else.
/// </summary>
/// <typeparam name="TKey">A unity-serializable value.</typeparam>
/// <typeparam name="TValue">A unity-serializable value.</typeparam>
[JsonObject(MemberSerialization.OptIn)]
[JsonConverter(typeof(UnityDictionaryConverter))]
[Serializable]
public class UnityDictionary<TKey, TValue> : IUnityDictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    #region Structs
    [Serializable]
    public struct InspectorKeyValuePair
    {
        #region Variables
        /// <summary>
        /// The key associated with this <see cref="InspectorKeyValuePair"/>.
        /// </summary>
        [SerializeField]
        private TKey key;

        /// <summary>
        /// The value associated with this <see cref="InspectorKeyValuePair"/>.
        /// </summary>
        [SerializeField]
        private TValue value;
        #endregion

        #region Properties
        /// <inheritdoc cref="key"/>
        public readonly TKey Key => key;

        /// <inheritdoc cref="value"/>
        public readonly TValue Value => value;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="InspectorKeyValuePair"/> from the provided
        /// <paramref name="key"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The provided key.</param>
        /// <param name="value">The provided value.</param>
        public InspectorKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Creates a new <see cref="InspectorKeyValuePair"/> from <paramref
        /// name="keyValuePair"/>.
        /// </summary>
        /// <param name="keyValuePair">The <see cref="KeyValuePair{TKey,
        /// TValue}"/> to use.</param>
        public InspectorKeyValuePair(KeyValuePair<TKey, TValue> keyValuePair)
        {
            this.key = keyValuePair.Key;
            this.value = keyValuePair.Value;
        }
        #endregion

        #region Converters
        /// <summary>
        /// Automatically converts from a <see cref="KeyValuePair{TKey,
        /// TValue}"/> to a <see cref="InspectorKeyValuePair"/>.
        /// </summary>
        /// <param name="kvp">The value being converted.</param>
        public static implicit operator KeyValuePair<TKey, TValue>(
            InspectorKeyValuePair kvp) =>
            new(kvp.Key, kvp.Value);

        /// <summary>
        /// Allows conversion from a <see cref="InspectorKeyValuePair"/> to a
        /// <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        /// <param name="kvp">The value being converted.</param>
        public static explicit operator InspectorKeyValuePair(
            KeyValuePair<TKey, TValue> kvp) =>
            new(kvp.Key, kvp.Value);
        #endregion
    }
    #endregion

    #region Variables
    /// <summary>
    /// The list of <see cref="InspectorKeyValuePair"/> used by the unity editor
    /// inspector.
    /// </summary>
    [SerializeField]
    [OnValueChanged(nameof(ResetInternalDict))]
    private List<InspectorKeyValuePair> keyValuePairs;

    /// <summary>
    /// The "real" dictionary. If a change is made to the internal dictionary,
    /// it should be reflected with the keyValuePairs.
    /// </summary>
    [JsonProperty("dictionary")]
#pragma warning disable IDE0044 // Add readonly modifier
    private Dictionary<TKey, TValue> internalDict;
#pragma warning restore IDE0044 // Add readonly modifier
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new empty UnityDictionary.
    /// </summary>
    public UnityDictionary()
    {
        keyValuePairs = new();
        internalDict = new();
    }

    /// <summary>
    /// Creates a new <see cref="UnityDictionary{TKey, TValue}"/> from the
    /// collection of key value pairs.
    /// </summary>
    /// <param name="collection">The aforementioned collection.</param>
    public UnityDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
        if (collection.IsNullOrEmpty())
        {
            // No items in collection
            keyValuePairs = new();
            internalDict = new();
        }
        else
        {
            // There are items in the collection.
            internalDict = new(collection);
            ResetInspectorKVPs(true);
        }
    }

    /// <summary>
    /// Creates a new <see cref="UnityDictionary{TKey, TValue}"/> from the
    /// provided <paramref name="dictionary"/>.
    /// </summary>
    /// <param name="dictionary">The provided dictionary.</param>
    [JsonConstructor]
    public UnityDictionary(IDictionary<TKey, TValue> dictionary)
    {
        internalDict = new(dictionary);
        ResetInspectorKVPs(true);
    }
    #endregion

    #region ISerializationCallbackReceiver Implementation
    /// <summary>
    /// Called right before this object is saved (serialized) in Unity. This
    /// "resets" the <see cref="keyValuePairs"/> after they are changed in the
    /// editor, which validates them and ensures that they remain consistent
    /// with the <see cref="internalDict"/>.
    /// </summary>
    /// <remarks>
    /// Note that this is not the same as JSON serialization. Unity uses its own
    /// serialization thing internally to load prefabs and stuff. See
    /// https://docs.unity3d.com/Manual/script-Serialization.html for what
    /// things are serialized, and see
    /// https://docs.unity3d.com/6000.0/Documentation/Manual/ScriptSerializationHowUnityUses.html
    /// for when things are serialized.
    /// </remarks>
    public void OnBeforeSerialize()
    {
        // Need to save the KVPs.
        ResetInspectorKVPs();
    }

    /// <summary>
    /// Called right after this object is loaded (deserialized). This "resets"
    /// the <see cref="internalDict"/>, forcing them to match with the newly
    /// loaded <see cref="keyValuePairs"/>.
    /// </summary>
    /// <inheritdoc cref="OnBeforeSerialize"/>
    public void OnAfterDeserialize()
    {
        // Set the dictionary based on the KVPs.
        ResetInternalDict();
    }
    #endregion

    #region IUnityDictionary Implementation
    public void ResetInspectorKVPs(bool force = false)
    {
        // Local function (see
        // https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic?view=netstandard-2.1).
        // Good idea to use these if you have redundant code that is specific to
        // one method/constructor/etc. See
        // https://www.reddit.com/r/csharp/comments/wi1ifu/comment/ij96ua2/ for
        // additional reasoning.
        void MakeKVPs()
        {
            keyValuePairs = new(
                internalDict.Select(static kvp => (InspectorKeyValuePair)kvp)
            );
        }

        if (!force && keyValuePairs != null && keyValuePairs.Count >= 2)
        {
            var last0 = keyValuePairs[^1];
            var last1 = keyValuePairs[^2];

            MakeKVPs();

            if (last0.Key.Equals(last1.Key))
            {
                // Keep last duplicate (may be added recently).
                keyValuePairs.Add(last0);
            }
        }
        else
        {
            MakeKVPs();
        }
    }

    public void ResetInternalDict(bool force = false)
    {
        internalDict.Clear();

        foreach (var kvp in keyValuePairs)
        {
            internalDict[kvp.Key] = kvp.Value;
        }
    }
    #endregion

    #region IDictionary Implementation
    #region Properties
    public Dictionary<TKey,TValue>.KeyCollection Keys => internalDict.Keys;

    public Dictionary<TKey,TValue>.ValueCollection Values => internalDict.Values;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

    public int Count => internalDict.Count;

    public bool IsReadOnly => false;

    public bool IsFixedSize => false;

    ICollection IDictionary.Keys => Keys;

    ICollection IDictionary.Values => Values;

    public bool IsSynchronized => false;

    public object SyncRoot => null;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

    public object this[object key]
    {
        get => this[(TKey)key];
        set => this[(TKey)key] = (TValue)value;
    }

    public TValue this[TKey key]
    {
        get => internalDict[key];
        set => internalDict[key] = value;
    }
    #endregion

    #region Methods
    public void Add(TKey key, TValue value) => internalDict.Add(key, value);

    public bool ContainsKey(TKey key) => internalDict.ContainsKey(key);

    public bool Remove(TKey key) => internalDict.Remove(key);

    public bool TryGetValue(TKey key, out TValue value) =>
        internalDict.TryGetValue(key, out value);

    public void Add(KeyValuePair<TKey, TValue> item) =>
        internalDict.Add(item.Key, item.Value);

    public void Clear() => internalDict.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return internalDict.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        ((IDictionary<TKey, TValue>)internalDict).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<TKey, TValue> item) =>
        internalDict.Remove(item.Key);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
        internalDict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => internalDict.GetEnumerator();

    public void Add(object key, object value) => Add((TKey)key, (TValue)value);

    public bool Contains(object key) => Contains((TKey)key);

    IDictionaryEnumerator IDictionary.GetEnumerator() =>
        (IDictionaryEnumerator)GetEnumerator();

    public void Remove(object key) => Remove((TKey)key);

    public void CopyTo(Array array, int index) =>
        CopyTo((KeyValuePair<TKey, TValue>[])array, index);
    #endregion
    #endregion

    #region Other Methods
    /// <summary>
    /// Checks if the <see cref="keyValuePairs"/> are valid and reports an error
    /// code if they are not.
    /// </summary>
    public UnityDictionaryErrorCode GetErrorCode()
    {
        UnityDictionaryErrorCode code = UnityDictionaryErrorCode.None;

        if (keyValuePairs == null)
        {
            return code;
        }

        if (keyValuePairs.Count != internalDict.Count)
        {
            code |= UnityDictionaryErrorCode.DuplicateKeys;
        }

        if (typeof(TKey).IsClass)
        {
            if (keyValuePairs.Any(kvp => kvp.Key == null))
            {
                code |= UnityDictionaryErrorCode.NullKeys;
            }
        }

        return code;
    }

    public override string ToString()
    {
        var kvpStr = this.Select(d => $"({d.Key}, {d.Value})");
        return $"<{string.Join(", ", kvpStr)}>";
    }

    public void TestValidation()
    {
        ResetInspectorKVPs();

        Assert.AreEqual(
            internalDict.Count, keyValuePairs.Count,
            $"InDict ({internalDict.Count}) and " +
            $"KVPs ({keyValuePairs.Count}) different lengths!"
        );

        foreach (var kvp in keyValuePairs)
        {
            Assert.IsTrue(
                internalDict.ContainsKey(kvp.Key),
                $"{kvp.Key} not in InDict"
            );
            Assert.AreEqual(
                internalDict[kvp.Key], kvp.Value,
                "InDict/KVP values mismatch " +
                $"({internalDict[kvp.Key]} vs {kvp.Value})"
            );
        }

        Debug.Log("Tests passed");
    }
    #endregion
}

