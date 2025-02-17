using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Drawer for <see cref="RNGPattern"/>
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
[CustomPropertyDrawer(typeof(RNGPattern))]
public class RNGPatternDrawer : PropertyDrawer
{
    [SerializeField]
    private int patternTypeIndex;

    [SerializeField]
    private bool foldout;

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        // Set up where everything needs to be drawn.
        EditorGUI.indentLevel = property.depth;
        position = EditorGUI.IndentedRect(position);

        foldout = EditorGUI.BeginFoldoutHeaderGroup(
            new Rect(position)
            {
                height = EditorExt.SpacedLineHeight
            },
            foldout,
            label
        );

        position.TranslateY(EditorExt.SpacedLineHeight);

        if (foldout)
        {
            EditorGUI.indentLevel++;

            // Get object
            property.ObjectFromProperty(out RNGPattern cRNG);
            SerializedProperty patternProperty = null;

            // Get list of type options
            List<Type> types = new(typeof(IRNGModel).FindInherited());

            List<GUIContent> typeNameList = new() { new("------") };
            typeNameList.AddRange(
                types.Select(t => new GUIContent(t.Name))
            );

            // If pattern already set, then update the index to match.
            if (cRNG.model != null)
            {
                patternTypeIndex = types.IndexOf(cRNG.model.GetType()) + 1;

                patternProperty = property.
                    FindPropertyRelative(nameof(RNGPattern.model));
            }

            // Show popup
            int index = EditorGUI.Popup(
                new Rect(position)
                {
                    height = EditorExt.SpacedLineHeight
                },
                new GUIContent("Type"),
                patternTypeIndex,
                typeNameList.ToArray()
            );

            position.TranslateY(EditorExt.SpacedLineHeight);

            if (index != patternTypeIndex)
            {
                // Index has been changed
                patternTypeIndex = index;

                if (patternTypeIndex > 0)
                {
                    // Determine + assign selection. No need if no change
                    // happened.
                    var selected = types[patternTypeIndex - 1];
                    cRNG.model = (IRNGModel)Activator.
                        CreateInstance(selected);
                }
            }

            if (patternProperty != null)
            {
                EditorGUI.PropertyField(
                    position, patternProperty
                );
                position.TranslateY(
                    EditorGUI.GetPropertyHeight(patternProperty)
                );
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndFoldoutHeaderGroup();
    }

    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        if (foldout)
        {
            var patternProperty = property.
                FindPropertyRelative(nameof(RNGPattern.model));
            return EditorExt.SpacedLineHeight * 2 +
                EditorGUI.GetPropertyHeight(patternProperty);
        }
        else
        {
            return EditorExt.SpacedLineHeight;
        }
    }
}