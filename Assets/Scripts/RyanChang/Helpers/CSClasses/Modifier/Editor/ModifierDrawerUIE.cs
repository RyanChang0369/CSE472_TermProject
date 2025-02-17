using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Modifier))]
public class ModifierDrawerUIE : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Define heights
        float lnHeight = EditorGUIUtility.singleLineHeight;
        float tHeight = GetPropertyHeight(property, label);
        float thirdWidth = position.width / 3;

        // Calculate rects
        var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, lnHeight);
        var addRect = new Rect(position.x + thirdWidth, position.y, thirdWidth, lnHeight);
        var multRect = new Rect(position.x + 2 * thirdWidth, position.y, thirdWidth, lnHeight);

        // Label
        EditorGUI.LabelField(labelRect, label);

        // Define indent
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        // Draw fields
        EditorGUIUtility.labelWidth = 25;
        EditorGUI.PropertyField(
            multRect,
            property.FindPropertyRelative(nameof(Modifier.multiplier)),
            new GUIContent("×")
        );
        EditorGUI.PropertyField(
            addRect,
            property.FindPropertyRelative(nameof(Modifier.addition)),
            new GUIContent("+")
        );
        EditorGUIUtility.labelWidth = 0;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }
}