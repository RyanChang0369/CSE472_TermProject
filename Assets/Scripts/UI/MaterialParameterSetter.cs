using System;
using System.ComponentModel;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets a material parameter.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class MaterialParameterSetter : MonoBehaviour
{
    #region Enums
    [Serializable]
    public enum ValueType
    {
        Float,
        Integer,
        Color,
        Texture
    }
    #endregion

    #region Variables
    #region Settings
    /// <summary>
    /// Name of the parameter.
    /// </summary>
    [Tooltip("Name of the parameter.")]
    [SerializeField]
    private string parameterName;

    /// <summary>
    /// The type of value this is.
    /// </summary>
    [Tooltip("The type of value this is.")]
    [SerializeField]
    private ValueType type;

    /// <summary>
    /// The values to be set to.
    /// </summary>
    [Tooltip("The values to be set to.")]
    [SerializeField]
    [ShowIf(nameof(type), ValueType.Float)]
    private float floatValue;

    /// <summary>
    /// The values to be set to.
    /// </summary>
    [Tooltip("The values to be set to.")]
    [SerializeField]
    [ShowIf(nameof(type), ValueType.Integer)]
    private int intValue;

    /// <summary>
    /// The values to be set to.
    /// </summary>
    [Tooltip("The values to be set to.")]
    [SerializeField]
    [ShowIf(nameof(type), ValueType.Color)]
    private Color colorValue;

    /// <summary>
    /// The values to be set to.
    /// </summary>
    [Tooltip("The values to be set to.")]
    [SerializeField]
    [ShowIf(nameof(type), ValueType.Texture)]
    private Texture textureValue;
    #endregion
    #endregion

    #region Methods
    #region Set Parameter
    /// <summary>
    /// Sets a parameter using all the default parameters in <see cref="this"/>.
    /// </summary>
    public void SetParameter() => SetParameter(parameterName);

    /// <summary>
    /// Sets a parameter with the name <paramref name="parameterName"/> to the
    /// value stored in this class.
    /// </summary>
    /// <param name="parameterName"></param>
    public void SetParameter(string parameterName)
    {
        DemoCanvasManager dcm = DemoCanvasManager.Instance;
        switch (type)
        {
            case ValueType.Float:
                dcm.SetMaterialParameter(parameterName, floatValue);
                break;
            case ValueType.Integer:
                dcm.SetMaterialParameter(parameterName, intValue);
                break;
            case ValueType.Color:
                dcm.SetMaterialParameter(parameterName, colorValue);
                break;
            case ValueType.Texture:
                dcm.SetMaterialParameter(parameterName, textureValue);
                break;
        }
    }

    /// <inheritdoc cref="SetParameter(string, object)"/>
    public void SetParameter(object value) => SetParameter(parameterName, value);

    /// <summary>
    /// Detect the type of <paramref name="value"/> and set the appropriate
    /// material parameter types.
    /// </summary>
    /// <param name="parameterName">Name of the material parameter.</param>
    /// <param name="value">Value to set to.</param>
    public void SetParameter(string parameterName, object value)
    {
        DemoCanvasManager dcm = DemoCanvasManager.Instance;
        if (value.IsInteger())
        {
            dcm.SetMaterialParameter(parameterName, (int)value);
        }
        else if (value.IsFloatingPoint())
        {
            dcm.SetMaterialParameter(parameterName, (float)value);
        }
        else if (value is Color color)
        {
            dcm.SetMaterialParameter(parameterName, color);
        }
        else if (value is Color32 color32)
        {
            dcm.SetMaterialParameter(parameterName, color32);
        }
        else if (value is Vector4 vector4)
        {
            dcm.SetMaterialParameter(parameterName, vector4);
        }
        else if (value is Texture texture)
        {
            dcm.SetMaterialParameter(parameterName, texture);
        }
    }

    /// <summary>
    /// Sets either a float or an integer material parameter, depending on the
    /// value of <see cref="type"/>.
    /// </summary>
    public void SetParameterWithRounding(float value)
    {
        switch (type)
        {
            case ValueType.Float:
                DemoCanvasManager.Instance.
                    SetMaterialParameter(parameterName, value);
                break;
            case ValueType.Integer:
                DemoCanvasManager.Instance.
                    SetMaterialParameter(parameterName, value.RoundToInt());
                break;
            default:
                throw new ArgumentException(
                    $"Cannot use float to alter a {type} parameter"
                );
        }
    }
    #endregion

    #region Get Parameter
    public object GetParameter(string parameterName)
    {
        DemoCanvasManager dcm = DemoCanvasManager.Instance;
        return type switch
        {
            ValueType.Float => dcm.GetMaterialFloat(parameterName),
            ValueType.Integer => dcm.GetMaterialInteger(parameterName),
            ValueType.Color => dcm.GetMaterialColor(parameterName),
            ValueType.Texture => dcm.GetMaterialTexture(parameterName),
            _ => throw new InvalidEnumArgumentException(
                    nameof(type),
                    (int)type,
                    typeof(ValueType)),
        };
    }

    public object GetParameter() => GetParameter(parameterName);
    #endregion
    #endregion
}