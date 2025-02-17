using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a dictionary of enums, where each enum is mapped to a value.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
[Serializable]
public class EnumDictionary<TEnum, TValue> :
    StaticKeyedDictionary<TEnum, TValue>
    where TEnum : struct, Enum
{
    #region Variables
    /// <summary>
    /// The default enum dictionary for this <see cref="TEnum"/> type.
    /// </summary>
    private static IDictionary<TEnum, TValue> DefaultEnumDict =>
        DefaultEnumValues.
        ToDictionary<TEnum, TEnum, TValue>(d => d, d => default);

    /// <summary>
    /// The default enum values for this <see cref="TEnum"/> type.
    /// </summary>
    public static IEnumerable<TEnum> DefaultEnumValues =>
        EnumExt.GetValues<TEnum>();
    #endregion

    #region Constructors
    public EnumDictionary() : base(DefaultEnumDict)
    { }

    protected EnumDictionary(IDictionary<TEnum, TValue> dict) : base(dict)
    { }

    #endregion

    #region StackedKeyDictionary Implementation
    protected override void GenerateStaticKeys(UnityEngine.Object targetObject,
        Dictionary<string, TValue> jsonValues)
    {
        LoadJsonValues(
            EnumExt.GetValues<TEnum>(),
            e => e.GetName()
        );
    }

    public override string LabelFromKey(TEnum key)
    {
        return Enum.GetName(typeof(TEnum), key);
    }
    #endregion
}
