using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Extensions for <see cref="ISaveLoadable{T}"/> and <see cref="ISaveState"/>.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public static class SaveLoadExt
{
    #region Variables
    private static readonly JsonSerializerSettings defaultJsonSettings = new()
    {
        Converters = {
            new UnityValueConverter(),
            new RoundingConverter(),
            new StringEnumConverter(),
            new IsoDateTimeConverter(),
            new SaveLoadableConverter(),
            new BitArrayConverter(),
        },
        TypeNameHandling = TypeNameHandling.Auto,
        ObjectCreationHandling = ObjectCreationHandling.Replace
    };
    #endregion

    #region Methods
    /// <summary>
    /// Converts the <see cref="ISaveLoadable{T}"/> directly into a JSON string.
    /// </summary>
    /// <param name="saveLoadable">The <see cref="ISaveLoadable{T}"/>.</param>
    /// <param name="prettyPrint">If true, pretty print the JSON string.</param>
    /// <returns>A JSON representation of saveLoadable.</returns>
    public static string ToJsonString<T>(this ISaveLoadable<T> saveLoadable,
        bool prettyPrint = false) where T : ISaveState
    {
        return saveLoadable.SaveState().ToJson(prettyPrint);
    }


    /// <summary>
    /// Returns a JSON representation of this object.
    /// </summary>
    /// <param name="obj">The object to save.</param>
    /// <param name="prettyPrint">If true, pretty print the JSON
    /// representation.</param>
    /// <param name="settings">The settings to use.</param>
    /// <returns>A JSON representation of this object.</returns>
    public static string ToJson<T>(this T obj,
        JsonSerializerSettings settings, bool prettyPrint = false)
    {
        return JsonConvert.SerializeObject(
            obj,
            prettyPrint ? Formatting.Indented : Formatting.None,
            MakeJsonSettings(settings)
        );
    }

    /// <inheritdoc cref="ToJson{T}(T, JsonSerializerSettings, bool)"/>
    public static string ToJson<T>(this T obj, bool prettyPrint = false)
    {
        return JsonConvert.SerializeObject(
            obj,
            prettyPrint ? Formatting.Indented : Formatting.None,
            defaultJsonSettings
        );
    }

    /// <summary>
    /// Converts the object into JSON then into CSV.
    /// </summary>
    public static string ToCSV(this string jsonString)
    {
        // CSVBuilder csv = new();
        CSVArrayBuilder csvArr;

        using (JsonTextReader reader = new(new StringReader(jsonString)))
        {
            csvArr = new(reader);
        }

        return csvArr.ToString();
    }

    /// <summary>
    /// Loads an object from the specified <paramref name="jsonString"/>.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    /// <param name="jsonString">The json representation of this struct.</param>
    /// <param name="settings">The settings to use.</param>
    /// <returns>The object loaded from its jsonString.</returns>
    public static T LoadFromJson<T>(this string jsonString,
        JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject<T>(jsonString,
            MakeJsonSettings(settings));
    }

    /// <inheritdoc cref="LoadFromJson{T}(string, JsonSerializerSettings)"/>
    public static T LoadFromJson<T>(this string jsonString)
    {
        jsonString.ThrowIfNullOrWhiteSpace();

        return JsonConvert.DeserializeObject<T>(
            jsonString,
            defaultJsonSettings
        );
    }

    /// <inheritdoc cref="LoadFromJson{T}(string, JsonSerializerSettings)"/>
    /// <param name="output">The object loaded from its jsonString.</param>
    public static void LoadFromJson<T>(this string jsonString,
        out T output, JsonSerializerSettings settings)
    {
        output = LoadFromJson<T>(jsonString, settings);
    }

    /// <inheritdoc cref="LoadFromJson{T}(string, JsonSerializerSettings)"/>
    /// <param name="output">The object loaded from its jsonString.</param>
    public static void LoadFromJson<T>(this string jsonString,
        out T output)
    {
        output = LoadFromJson<T>(jsonString);
    }

    /// <summary>
    /// Populates an object using the specified <paramref name="jsonString"/>.
    /// </summary>
    /// <param name="target">Object to populate.</param>
    /// <returns></returns>
    /// <inheritdoc cref="FromJson{T}(string, JsonConverter[])"/>
    public static void PopulateFromJson<T>(this string jsonString,
        T target, JsonSerializerSettings settings)
    {
        JsonConvert.PopulateObject(jsonString, target,
            MakeJsonSettings(settings));
    }

    /// <inheritdoc cref="PopulateFromJson{T}(string, T,
    /// JsonSerializerSettings)"/>
    public static void PopulateFromJson<T>(this string jsonString, T target)
    {
        JsonConvert.PopulateObject(jsonString, target, defaultJsonSettings);
    }

    /// <summary>
    /// Loads an object from the specified <paramref name="jsonString"/>.
    /// </summary>
    /// <typeparam name="SL">An <see cref="ISaveLoadable{T}"/>.</typeparam>
    /// <typeparam name="SS">An <see cref="ISaveState"/>.</typeparam>
    /// <param name="target">The <see cref="ISaveLoadable{T}"/> to load.</param>
    /// <param name="jsonString">The json representation of this struct.</param>
    /// <param name="settings">The settings to use.</param>
    /// <returns>The object loaded from its jsonString.</returns>
    public static SL LoadStateFromJson<SL, SS>(this SL target, string jsonString,
        JsonSerializerSettings settings)
        where SL : ISaveLoadable<SS>
        where SS : ISaveState
    {
        SS state = jsonString.LoadFromJson<SS>(settings);

        if (target.CanLoadState(state))
        {
            target.LoadState(state);
            return target;
        }

        throw new ArgumentException($"{target} cannot load {state}.");
    }

    /// <summary>
    /// Returns a modified <see cref="JsonSerializerSettings"/> to include all
    /// the converters in <paramref name="additionalConverters"/>.
    /// </summary>
    /// <param name="additionalConverters">Array of additional
    /// converters.</param>
    /// <returns>The new settings.</returns>
    public static JsonSerializerSettings MakeJsonSettings(
        JsonSerializerSettings settings)
    {
        return new(settings)
        {
            Converters = settings.Converters
                .Intersect(defaultJsonSettings.Converters)
                .ToList(),
            TypeNameHandling = defaultJsonSettings.TypeNameHandling,
        };
    }
    #endregion
}
