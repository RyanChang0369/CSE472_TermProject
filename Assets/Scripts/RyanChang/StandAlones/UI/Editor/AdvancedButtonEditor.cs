using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(AdvancedButton), true)]
public class AdvancedButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.UpdateIfRequiredOrScript();

        // Show the new Unity events
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AdvancedButton.onPointerEnter)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AdvancedButton.onPointerExit)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AdvancedButton.onPointerDown)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AdvancedButton.onPointerUp)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AdvancedButton.onSelect)));
        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AdvancedButton.onDeselect)));

        // Finalize.
        serializedObject.ApplyModifiedProperties();
    }
}