using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// JSON converter for the <see cref="BitArray"/>.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public class BitArrayConverter : JsonConverter<BitArray>
{
    public override BitArray ReadJson(JsonReader reader,
        Type objectType, BitArray existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        Debug.Log("BitArray: ReadJson");
        int[] data = JsonConvert.DeserializeObject<int[]>(reader.Value.ToString());
        return new BitArray(data);
    }

    public override void WriteJson(JsonWriter writer, BitArray value,
        JsonSerializer serializer)
    {
        Debug.Log("BitArray: WriteJson");
        int[] data = new int[Mathf.CeilToInt(value.Length / 32f)];
        value.CopyTo(data, 0);
        writer.WriteValue(data.ToJson());
    }
}
