using System;

[Flags]
public enum UnityDictionaryErrorCode
{
    /// <summary>
    /// <see cref="keyValuePairs"/> has been successfully validated.
    /// </summary>
    None = 0,
    /// <summary>
    /// <see cref="keyValuePairs"/> has at least 2 duplicate keys.
    /// </summary>
    DuplicateKeys = 1,
    /// <summary>
    /// <see cref="keyValuePairs"/> has at least 1 null key.
    /// </summary>
    NullKeys = 2
}