using System.Collections.Generic;

/// <summary>
/// Defines a chain of <see cref="Modifier"/>s. Allows one float to be modified
/// by an arbitrary number of modifiers.
///
/// <br/>
///
/// USAGE: To add to the chain, use <see cref="Add(PriorityKey, Modifier)"/>.
/// <see cref="PriorityKey"/> determines the order the <see cref="Modifier"/>s
/// are executed, with lower values of <see cref="PriorityKey.priority"/>
/// denoting higher priority.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
/// <seealso cref="PriorityKey"/>
[System.Serializable]
public class ModifierChain
{
    #region Variables
    /// <summary>
    /// The internal modifier chain used in modifying some value.
    /// </summary>
    private SortedDictionary<PriorityKey<int>, Modifier> chain = new();

    /// <summary>
    /// The cached value.
    /// </summary>
    private float cache;

    /// <summary>
    /// True if the cache is dirty (cache may be incorrect), false otherwise
    /// (cache is correct).
    /// </summary>
    private bool cacheDirty = true;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new modifier chain with an empty chain.
    /// </summary>
    public ModifierChain()
    {
        chain = new();
        cacheDirty = true;
    }
    #endregion

    /// <summary>
    /// Adds a modifier to the chain.
    /// </summary>
    /// <param name="key">Key to add.</param>
    /// <param name="modifier">Modifier to add.</param>
    /// <returns>True if key successfully added, false otherwise.</returns>
    public bool Add(PriorityKey<int> key, Modifier modifier)
    {
        if (chain.ContainsKey(key))
            return false;

        chain.Add(key, modifier);
        cacheDirty = true;

        return true;
    }

    /// <summary>
    /// Removes a modifier from the chain.
    /// </summary>
    /// <param name="key">Key to remove.</param>
    /// <returns>True on successful removal, false otherwise.</returns>
    public bool Remove(PriorityKey<int> key)
    {
        if (chain.Remove(key))
        {
            cacheDirty = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Clears the list of modifiers.
    /// </summary>
    public void Clear()
    {
        chain.Clear();
        cacheDirty = true;
    }

    /// <summary>
    /// Modifies the input using the chain.
    /// </summary>
    /// <param name="input">The float to be modified.</param>
    /// <returns></returns>
    public float Modify(float input)
    {
        if (cacheDirty)
        {
            cache = input;

            foreach (var pair in chain)
            {
                cache = pair.Value.Modify(cache);
            }

            cacheDirty = false;

            return cache;
        }
        else
        {
            return cache;
        }
    }
}
