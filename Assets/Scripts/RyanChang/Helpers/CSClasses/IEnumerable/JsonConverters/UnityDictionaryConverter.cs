using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

/// <summary>
/// <see cref="JsonConverter"/> for <see cref="UnityDictionary{TKey, TValue}"/>.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public class UnityDictionaryConverter : JsonConverter
{
    protected virtual Type DictionaryType => typeof(UnityDictionary<,>);

    public override bool CanConvert(Type objectType)
    {
        if (objectType.IsGenericType)
        {
            var generics = objectType.GenericTypeArguments;

            if (generics.Length == 2)
            {
                var unityDictType = DictionaryType.
                    MakeGenericType(generics);
                return objectType.IsAssignableFrom(unityDictType);
            }
        }

        return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType,
        object existingValue, JsonSerializer serializer)
    {
        Type[] generics = GetBaseGenericTypeArguments(objectType);

        Type dictType = typeof(IDictionary<,>).
            MakeGenericType(generics);
        Type unityDictType = DictionaryType.
            MakeGenericType(generics);

        object value = unityDictType.GetConstructor(
            new Type[] { dictType }
        ).Invoke(
            new object[] { serializer.Deserialize(reader, dictType) }
        );

        return value;
    }

    public override void WriteJson(JsonWriter writer, object value,
        JsonSerializer serializer)
    {
        var generics = GetBaseGenericTypeArguments(value.GetType());

        var dictType = typeof(Dictionary<,>).
            MakeGenericType(generics);
        var iDictType = typeof(IDictionary<,>).
            MakeGenericType(generics);

        object dictionary = dictType.GetConstructor(
            new Type[] { iDictType }
        ).Invoke(
            new object[] { value }
        );

        serializer.Serialize(writer, dictionary);
    }

    /// <summary>
    /// Gets an array of generic types the UnityDictionary type has.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected Type[] GetBaseGenericTypeArguments(Type type)
    {
        Type[] interfaces = type.FindInterfaces(
            new TypeFilter(TypeNameFilter),
            "IDictionary`2"
        );

        if (interfaces.Length == 0)
            throw new ArgumentException(
                type.Name + " does not inherit from IDictionary`2"
            );
        
        return interfaces[0].GenericTypeArguments;
    }

    private bool TypeNameFilter(Type filter, object criteria) =>
        filter.Name == (string)criteria;
}
