using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ModifierRangeAttribute))]
public class ModifierDrawerRangeUIE : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Attribute
        ModifierRangeAttribute attr = (ModifierRangeAttribute)attribute;

        // Define heights
        float lnHeight = EditorGUIUtility.singleLineHeight;
        float halfWidth = position.width / 2f;

        // Calculate rects
        var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, lnHeight);
        var addRect = new Rect(position.x, position.y + EditorExt.SpacedLineHeight, halfWidth, lnHeight);
        var multRect = new Rect(position.x + halfWidth, position.y + EditorExt.SpacedLineHeight, halfWidth, lnHeight);

        // Label
        EditorGUI.LabelField(labelRect, label);

        // Define indent
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        // Draw fields
        EditorGUIUtility.labelWidth = 25;

        GUIStyle italics = new GUIStyle(GUI.skin.textArea);
        italics.fontStyle = FontStyle.Italic;

        if (attr.multiplierMin < attr.multiplierMax)
        {
            EditorGUI.Slider(
                multRect,
                property.FindPropertyRelative(nameof(Modifier.multiplier)),
                attr.multiplierMin,
                attr.multiplierMax,
                new GUIContent("×")
            );
        }
        else
        {
            EditorGUI.LabelField(
                multRect,
                "Multiplier Disabled",
                italics
            );
        }

        if (attr.additionMin < attr.additionMax)
        {
            EditorGUI.Slider(
                addRect,
                property.FindPropertyRelative(nameof(Modifier.addition)),
                attr.additionMin,
                attr.additionMax,
                new GUIContent("+")
            );
        }
        else
        {
            EditorGUI.LabelField(
                addRect,
                "Addition Disabled",
                italics
            );
        }

        EditorGUIUtility.labelWidth = 0;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorExt.SpacedLineHeight * 2;
    }
}