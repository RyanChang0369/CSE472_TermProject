using System;
using Newtonsoft.Json;

/// <summary>
/// A json converter for float, double, and decimals.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public class RoundingConverter : JsonConverter
{
    #region Variables / Properties
    /// <summary>
    /// Digits to round to.
    /// </summary>
    private readonly int digits;

    public override bool CanRead => false;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a rounding converter.
    /// </summary>
    /// <param name="digits">Digits to round to.</param>
    public RoundingConverter(int digits = 3)
    {
        this.digits = digits;
    }
    #endregion

    #region Methods
    public override bool CanConvert(Type objectType) =>
        objectType == typeof(float) ||
        objectType == typeof(double) ||
        objectType == typeof(decimal);

    public override object ReadJson(JsonReader reader, Type objectType,
        object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value,
        JsonSerializer serializer)
    {
        if (value is float flt)
            writer.WriteValue(flt.Round(digits));
        else if (value is double dbl)
            writer.WriteValue(Math.Round(dbl, digits));
        else if (value is decimal dec)
            writer.WriteValue(Math.Round(dec, digits));
        else
            throw new NotImplementedException();
    }
    #endregion
}