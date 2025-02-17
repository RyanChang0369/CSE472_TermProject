using UnityEngine;
using UnityEditor;

/// <summary>
/// Drawer for <see cref="ToggleActiveAttribute"/>.
/// </summary>
///
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
[CustomPropertyDrawer(typeof(ToggleActiveAttribute), true)]
public class ToggleActiveAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        string targetName = ((ToggleActiveAttribute)attribute).ToggledName;
        SerializedProperty targetProperty = property.serializedObject.FindProperty(
            property.propertyPath.SpliceWords(0..^1, '.') + '.' + targetName
        );

        float h = EditorExt.SpacedLineHeight;

        Rect togglePosition = new(
            position.x, position.y,
            h * 2, position.height
        );
        // h *= 2;
        Rect targetPosition = new(
            position.x + h, position.y,
            position.width - h, position.height
        );

        property.boolValue = EditorGUI.ToggleLeft(
            togglePosition,
            "",
            property.boolValue
        );
        EditorGUI.BeginDisabledGroup(!property.boolValue);
        EditorGUI.PropertyField(targetPosition, targetProperty, label);
        EditorGUI.EndDisabledGroup();
    }
}