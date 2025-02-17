using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixedList<>), true)]
public class FixedListPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.isExpanded = EditorGUI.Foldout(position,
            property.isExpanded, label.text, EditorStyles.foldoutHeader);

        if (property.isExpanded)
        {
            // Enter child, which is internalList.
            property.Next(true);
            int arrLen = property.arraySize;

            // Collapsable section
            EditorGUI.indentLevel++;

            position.xMin += EditorGUIUtility.singleLineHeight;  // Some padding
            position.Translate(new(
                0,
                EditorExt.SpacedLineHeight
            ));

            for (int i = 0; i < arrLen; i++)
            {
                // Draw each of the elements in internalList.
                var elem = property.GetArrayElementAtIndex(i);
                elem.PropertyField(ref position, new($"Element {i}"));
            }
            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            float height = base.GetPropertyHeight(property, label);

            // Enter child
            property.Next(true);
            int arrLen = property.arraySize;

            for (int i = 0; i < arrLen; i++)
            {
                var elem = property.GetArrayElementAtIndex(i);
                height += EditorGUI.GetPropertyHeight(elem);
            }

            return height + EditorGUIUtility.standardVerticalSpacing;
        }

        return EditorGUIUtility.singleLineHeight;
    }
}