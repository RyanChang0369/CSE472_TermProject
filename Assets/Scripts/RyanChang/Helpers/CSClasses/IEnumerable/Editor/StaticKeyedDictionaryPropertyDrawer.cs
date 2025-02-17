using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

[CustomPropertyDrawer(typeof(StaticKeyedDictionary<,>), true)]
public class StaticKeyedDictionaryPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        try
        {
            // Fix the enum dictionary if needed.
            property.ObjectFromProperty(out IStaticKeyedDictionary dict);

            // Check if the base property is expanded or not. If not, don't draw
            // anything.
            property.isExpanded = EditorGUI.Foldout(
                position,
                property.isExpanded,
                label.text,
                EditorStyles.foldoutHeader);

            if (property.isExpanded)
            {
                // Inspector key value pairs changed checking boolean.
                bool ikvpChanged = false;
                // Collapsable section
                EditorGUI.indentLevel++;

                // First drop into the first child, which will be editorDict,
                // then drop into the second child, which will be keyValuePairs.
                property = property.FindPropertyRelative(
                    "editorDict.keyValuePairs"
                );
                property.AssertNotNull(
                    "Could not locate editorDict.keyValuePairs"
                );

                foreach (SerializedProperty child in property)
                {
                    // Drop into the Key field and get the static key value.
                    var key = child.FindPropertyRelative("key");
                    // child.Next(true);
                    var keyVal = key.ObjectFromProperty();
                    string keyName = dict.LabelFromKey(keyVal);

                    // Drop into the Value field and draw that property.
                    if (key.Next(false))
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(
                            key,
                            new GUIContent(keyName),
                            GUILayout.ExpandHeight(true),
                            GUILayout.ExpandWidth(true)
                        );
                        ikvpChanged |= EditorGUI.EndChangeCheck();
                    }
                    else
                    {
                        Debug.LogWarning("Could not serialize value! " +
                            "Make sure the value is marked Serializable.");
                        EditorGUILayout.LabelField(new GUIContent(keyName));
                    }
                }

                if (ikvpChanged)
                {
                    // Thing has changed.
                    dict.ResetInspectorKVPs();
                }

                // Exit collapsible section.
                EditorGUI.indentLevel--;
            }
        }
        catch (ArgumentException e)
        {
            EditorGUI.LabelField(
                position,
                new GUIContent(
                    "<color=red><b>Cannot display dictionary:</b></color> " +
                    $"<color=white>{e.Message}</color>"),
                new GUIStyle()
                {
                    richText = true
                }
            );
            return;
        }
    }
}
