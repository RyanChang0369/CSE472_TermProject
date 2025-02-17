using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// Contains methods pertaining to Newtonsoft's <see cref="JsonWriter"/> and
/// <see cref="JsonReader"/>.
/// </summary>
public static class JsonExt
{
    #region JSON Writer
    /// <summary>
    /// Writes the property name followed by the value of the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer">The JsonWriter.</param>
    /// <param name="name">Name of the property used to serialize it.</param>
    /// <param name="property">The property to write.</param>
    public static void WriteProperty<T>(this JsonWriter writer, string name,
        T property)
    {
        writer.WritePropertyName(name);
        writer.WriteValue(property);
    }

    /// <summary>
    /// Writes a raw value with the correct indentation. Adapted from <see
    /// href="https://stackoverflow.com/a/67360891"/>.
    /// </summary>
    /// <param name="writer">The JsonWriter.</param>
    /// <param name="jsonString">The json string.</param>
    /// <param name="dateParseHandling">How to handle dates? It's recommended
    /// that it be set to none so that dates aren't confused with
    /// numbers.</param>
    /// <param name="floatParseHandling">How to handle floats?</param>
    public static void WriteFormattedRawValue(this JsonWriter writer,
        string jsonString,
        DateParseHandling dateParseHandling = DateParseHandling.None,
        FloatParseHandling floatParseHandling = default)
    {
        if (jsonString == null)
        {
            writer.WriteRawValue(jsonString);
        }
        else
        {
            using var reader = new JsonTextReader(new StringReader(jsonString))
            {
                DateParseHandling = dateParseHandling,
                FloatParseHandling = floatParseHandling
            };
            writer.WriteToken(reader);
        }
    }

    /// <summary>
    /// Writes an array to JSON.
    /// </summary>
    /// <typeparam name="T">Must be writable via <see
    /// cref="JsonWriter"/>.</typeparam>
    /// <param name="writer">The JsonWriter.</param>
    /// <param name="values">Values to write.</param>
    public static void WriteArray<T>(this JsonWriter writer, params T[] values)
    {
        writer.WriteEnumerable(values);
    }

    /// <inheritdoc cref="WriteArray{T}(JsonWriter, T[])"/>
    public static void WriteEnumerable<T>(this JsonWriter writer,
        IEnumerable<T> values)
    {
        writer.WriteStartArray();

        foreach (var val in values)
        {
            writer.WriteRawValue(val.ToJson());
        }

        writer.WriteEndArray();
    }
    #endregion

    #region Json Reader
    /// <summary>
    /// Reads JSON until the predicate <paramref name="seekTo"/> is satisfied,
    /// then returns true. Returns false if the predicate is never satisfied
    /// before all the JSON is read.
    /// </summary>
    /// <param name="reader">The Json reader to use.</param>
    /// <param name="seekTo">The predicate to seek until.</param>
    /// <returns>True if predicate satisfied, false otherwise.</returns>
    public static bool Seek(this JsonReader reader, Predicate<JsonReader> seekTo)
    {
        while (reader.Read())
        {
            if (seekTo(reader))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Reads <paramref name="reader"/> until we reach a property with the
    /// specified property name.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    /// <param name="reader">The Json reader to use.</param>
    /// <param name="propertyName">The name of the property to read
    /// from.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T ReadProperty<T>(this JsonReader reader, string propertyName)
    {
        // Check for the following:
        if (!reader.Seek(
            // Find token with a property name.
            t => t.TokenType == JsonToken.PropertyName
            // Convert value to a string.
            && t.Value is string name
            // Check if the name matches the propertyName.
            && name == propertyName))
        {
            // If not, throw error.
            throw new ArgumentException($"Property {propertyName} not found " +
                "in json reader");
        }

        reader.Read();

        return (T)reader.Value;
    }

    /// <inheritdoc cref="ReadProperty{T}(JsonReader, string)"/>
    /// <param name="value">If the property is found, its value will be placed
    /// here.</param>
    public static void ReadProperty<T>(this JsonReader reader,
        string propertyName, out T value)
    {
        value = ReadProperty<T>(reader, propertyName);
    }

    /// <summary>
    /// Reads a JSON array with member types <typeparamref name="T"/>.
    /// </summary>
    /// <inheritdoc cref="ReadProperty{T}(JsonReader, string)"/>
    public static T[] ReadArray<T>(this JsonReader reader)
    {
        return DoArrayRead<T>(reader).ToArray();
    }

    /// <summary>
    /// Helper method that preforms an array read.
    /// </summary>
    /// <inheritdoc cref="ReadArray{T}(JsonReader)"/>
    private static IEnumerable<T> DoArrayRead<T>(JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonToken.StartArray:
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    yield return JsonConvert.DeserializeObject<T>(
                        reader.Value.ToString()
                    );
                }
                break;
            case JsonToken.Null:
                break;
            default:
                throw new JsonException(
                    "Invalid token type " + reader.TokenType
                );
        }
    }
    #endregion
}
