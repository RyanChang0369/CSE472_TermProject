using System;
using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine.Assertions;

/// <summary>
/// <see cref="JsonConverter"/> for <see
/// cref="StaticKeyedDictionary{TKey, TValue}"/>.
/// </summary>
///
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public class StaticKeyedDictionaryConverter : UnityDictionaryConverter
{
    protected override Type DictionaryType => typeof(StaticKeyedDictionary<,>);

    public override bool CanConvert(Type objectType)
    {
        return base.CanConvert(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType,
        object existingValue, JsonSerializer serializer)
    {
        Type dictType = typeof(IDictionary);

        object dict = serializer.Deserialize(reader, dictType);
        var constructorInfo = objectType.GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            Type.DefaultBinder,
            new Type[] { },
            new ParameterModifier[] { }
        );

        Assert.IsNotNull(
            dict,
            "Cannot serialize to IDictionary"
        );

        Assert.IsNotNull(
            constructorInfo,
            "Constructor not found for " + objectType.Name
        );

        object value = constructorInfo.Invoke(
            new object[] { }
        );

        return value;
    }
}
