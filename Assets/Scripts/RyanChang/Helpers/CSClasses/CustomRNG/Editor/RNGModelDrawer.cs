using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(IRNGModel), true)]
public class RNGModelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(
            position,
            label,
            property
        );

        foreach (var child in property.GetImmediateChildren())
        {
            float childHeight = child.GetPropertyHeight();

            EditorGUI.PropertyField(
                new Rect(position)
                {
                    height = childHeight
                },
                property
            );

            position.TranslateY(childHeight);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label) =>
            (float)property.GetImmediateChildren().
            Sum(p => p.GetPropertyHeight());
}