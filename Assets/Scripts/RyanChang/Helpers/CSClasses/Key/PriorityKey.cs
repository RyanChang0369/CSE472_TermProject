using System;
using Newtonsoft.Json;

/// <summary>
/// Can be used in a sorted list, dictionary, or set in order to define priority
/// for its values.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
[Serializable]
public class PriorityKey<TPriority> : IComparable<PriorityKey<TPriority>>
    where TPriority : IComparable<TPriority>
{
    #region Variables
    /// <summary>
    /// Based on convention, a lower value for <see cref="priority"/> usually
    /// denotes a higher priority, so lower values of <see cref="priority"/>
    /// means that the key will be selected ahead of keys with higher values of
    /// <see cref="priority"/>.
    /// </summary>
    [JsonProperty]
    public TPriority priority;

    /// <summary>
    /// The ID of the key, used to distinguish between different keys. This is
    /// used as a first-level tie breaker if two keys have the same priority. It
    /// is intended that this be set to the InstanceID of a Unity GameObject,
    /// but it can be set to any unique value. See the constructors of this
    /// class for more information.
    /// </summary>
    [JsonProperty]
    public int id;

    /// <summary>
    /// Optional tag to distinguish between keys with the same ID and
    /// priorities. This is used as a second-level tie breaker id two keys have
    /// the same priority and ID. You could, for example, use it to denote which
    /// method this key is used in.
    /// </summary>
    [JsonProperty]
    public string tag;
    #endregion

    #region Constructor
    /// <summary>
    /// Copies a key.
    /// </summary>
    /// <param name="key">The priority key to copy from.</param>
    public PriorityKey(PriorityKey<TPriority> key) :
        this(key.priority, key.id, key.tag)
    { }

    /// <summary>
    /// Creates a new priority key.
    /// </summary>
    /// <param name="priority">Priority of the key. A lower value is a higher
    /// priority.</param>
    /// <param name="id">ID of the key. See <see cref="id"/>.</param>
    /// <param name="tag">Optional tag to distinguish between different keys
    /// with the same ID. See <see cref="tag"/>. If not provided or null,
    /// generates a random hash string to be used as the tag.</param>
    [JsonConstructor]
    public PriorityKey(TPriority priority, int id, string tag = null)
    {
        this.priority = priority;
        this.id = id;
        this.tag = tag ?? RNGExt.RandomHashString(
            16,
            RNGExt.HashStringOptions.WithDashes
        );
    }

    /// <summary>
    /// Creates a new priority key, using a Unity GameObject to generate the ID.
    /// </summary>
    /// <param name="unityObject">Object used for ID. See <see
    /// cref="id"/>.</param>
    /// <inheritdoc cref="PriorityKey{TPriority}(TPriority, int, string)"/>
    public PriorityKey(TPriority priority, UnityEngine.Object unityObject,
        string tag = null) :
        this(priority, unityObject.GetInstanceID(), tag)
    { }

    /// <summary>
    /// Creates a new priority key, using a Unity GameObject to generate the ID,
    /// with the default priority.
    /// </summary>
    /// <inheritdoc cref="PriorityKey(TPriority, UnityEngine.Object, string)"/>
    public PriorityKey(UnityEngine.Object unityObject, string tag = null) :
        this(default, unityObject.GetInstanceID(), tag)
    { }
    #endregion

    #region Methods
    public int CompareTo(PriorityKey<TPriority> other)
    {
        // Lower value == higher priority.
        int cmp = priority.CompareTo(other.priority);

        return cmp == 0 ?
        (
            (id == other.id) ?
                tag.CompareTo(other.tag) :
                id.CompareTo(other.id)
        ) :
        cmp;
    }

    public override string ToString() =>
        $"PriorityKey [{priority}] ({id}:{tag})";

    public override int GetHashCode() =>
        HashCode.Combine(priority, id, tag);
    #endregion
}