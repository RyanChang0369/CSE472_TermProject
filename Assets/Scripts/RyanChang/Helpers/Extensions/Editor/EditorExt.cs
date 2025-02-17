using System.Collections;
using IOPath = System.IO.Path;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Contains classes pertaining to editor stuff.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public static class EditorExt
{
    #region Constants/Pseudo-Constants
    /// <summary>
    /// Default binding flags for various reflection operations.
    /// </summary>
    private const BindingFlags DEFAULT_BINDING =
        BindingFlags.Public | BindingFlags.NonPublic |
        BindingFlags.Instance | BindingFlags.Static;
    
    /// <summary>
    /// Equal to <see cref="EditorGUIUtility.singleLineHeight"/> +
    /// <see cref="EditorGUIUtility.standardVerticalSpacing"/>.
    /// </summary>
    public static float SpacedLineHeight => EditorGUIUtility.singleLineHeight
        + EditorGUIUtility.standardVerticalSpacing;
    #endregion

    #region Reflection
    #region Get Object
    /// <summary>
    /// Gets the proper object using reflection.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="propertyPath">The custom property path to search for the
    /// object.</param>
    /// <exception cref="NullReferenceException">
    /// If one or more of the properties (except for the last, target property)
    /// in the path is null.
    /// </exception>
    public static object ObjectFromProperty(
        this SerializedProperty property, string propertyPath)
    {
        // Get the target object and type. These will be modified when we are
        // traversing the path.
        object targetObject = property.serializedObject.targetObject;
        Type targetType = targetObject.GetType();

        string fullPath = propertyPath;
        fullPath = fullPath.Replace("Array.data[", "[");

        string[] path = fullPath.Split('.');

        // Path traversal.
        foreach (var name in path)
        {
            if (targetObject == null)
                throw new NullReferenceException(
                    $"Trying to obtain field {name} from null reference. " +
                    $"Full path is {fullPath}. Target type is {targetType}."
                );

            // Check if there's a bracket. If so, that means that we have an
            // array.
            int lBracket = name.IndexOf('[');
            int rBracket = name.IndexOf(']');
            if (lBracket >= 0 && rBracket >= 0)
            {
                lBracket++;
                // rBracket--;
                // In an array
                // This is an array element we are looking for.
                int index = int.Parse(name[lBracket..rBracket]);

                // Iterate through the enumerable, until index is reached.
                foreach (object thing in (IEnumerable)targetObject)
                {
                    if (index.IsNegative(SignBehavior.ZeroIsNegative))
                    {
                        targetObject = thing;
                        targetType = thing.GetType();
                        break;
                    }

                    index--;
                }
            }
            else if (lBracket.Sign() != rBracket.Sign())
            {
                throw new ArgumentException(
                    $"Mismatched brackets: {name}"
                );
            }
            else
            {
                // Go through all inherited classes as well.
                // For each name in the path, get the reflection.
                FieldInfo field = targetType.GetField(
                    name,
                    BindingFlags.NonPublic | BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.FlattenHierarchy
                ) ?? throw new NullReferenceException(
                    $"Field is null for {name}"
                );

                targetType = field.FieldType;
                targetObject = field.GetValue(targetObject);
            }
        }

        return targetObject;
    }

    /// <inheritdoc cref="ObjectFromProperty(SerializedProperty)"/>
    public static object ObjectFromProperty(
        this SerializedProperty property) =>
        ObjectFromProperty(property, property.propertyPath);

    /// <param name="obj">The object to assign the value to.</param>
    /// <inheritdoc cref="ObjectFromProperty(SerializedProperty)"/>
    public static void ObjectFromProperty<T>(
        this SerializedProperty property, out T obj) =>
        obj = (T)property.ObjectFromProperty();

    /// <param name="obj">The object to assign the value to.</param>
    /// <inheritdoc cref="ObjectFromProperty(SerializedProperty)"/>
    public static void ObjectFromProperty<T>(
        this SerializedProperty property, string propertyPath, out T obj) =>
        obj = (T)property.ObjectFromProperty(propertyPath);
    #endregion

    #region Seek to Type
    /// <summary>
    /// Calls <see cref="SerializedProperty.Next(bool)"/> until a child is found
    /// of type <paramref name="type"/>. This is useful if your property is
    /// nested somewhere.
    /// </summary>
    /// <param name="property">The serialized property to search. If it returns
    /// true, then this will be a serialized property of type <paramref
    /// name="type"/>. Otherwise, it is reset.</param>
    /// <param name="type">The type to look for.</param>
    /// <param name="visitChildren">If true, then drops down into children
    /// properties.</param>
    /// <param name="visibleOnly">If true, then calls <see
    /// cref="SerializedProperty.NextVisible(bool)"/> instead.</param>
    /// <return>True on success, false otherwise.</return>
    public static bool SeekToType(this SerializedProperty property,
        Type type, bool visitChildren = true, bool visibleOnly = false)
    {
        do
        {
            if (property.type == type.Name)
                return true;
        }
        while (visibleOnly ?
            property.NextVisible(visitChildren) :
            property.Next(visitChildren));

        property.Reset();
        return false;
    }

    /// <inheritdoc cref="SeekToType(SerializedProperty, Type, bool)"/>
    /// <typeparam name="T">The type.</typeparam>
    public static bool SeekToType<T>(this SerializedProperty property,
         bool visitChildren = true, bool visibleOnly = false) =>
        SeekToType(property, typeof(T), visitChildren, visibleOnly);
    #endregion

    #region Force Get Member Value
    /// <summary>
    /// Gets the value of a member by its name from some <paramref
    /// name="target"/>, using whatever means nessisary. If such a member cannot
    /// be found, then throws a <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="target">The target object to obtain the member
    /// value.</param>
    /// <param name="outputType">The type of the return value.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>The value of the member.</returns>
    public static object ForceGetMemberValue(this object target,
        Type outputType, string memberName,
        BindingFlags bindingFlags = DEFAULT_BINDING)
    {
        Type targetType = target.GetType();

        MemberInfo[] members = targetType.GetMember(
            memberName,
            bindingFlags
        );

        foreach (var member in members)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo field = (FieldInfo)member;

                    if (outputType.IsAssignableFrom(field.FieldType))
                        return field.GetValue(target);

                    break;
                case MemberTypes.Property:
                    PropertyInfo property = (PropertyInfo)member;

                    if (outputType.IsAssignableFrom(property.PropertyType) &&
                        property.CanRead)
                    {
                        return property.GetValue(target);
                    }

                    break;
                case MemberTypes.Method:
                    MethodInfo method = (MethodInfo)member;

                    if (outputType.IsAssignableFrom(method.ReturnType) &&
                        method.GetParameters().Any(p => !p.HasDefaultValue))
                    {
                        return method.Invoke(target, new object[0]);
                    }

                    break;
            }
        }

        throw new ArgumentException(
            $"Cannot find member: {memberName}"
        );
    }

    /// <inheritdoc cref="ForceGetMemberValue(object, Type, string)"/>
    /// <typeparam name="T">The type of the return value.</typeparam>
    public static T ForceGetMemberValue<T>(this object target,
        string memberName)
    {
        return (T)ForceGetMemberValue(target, typeof(T), memberName);
    }
    #endregion
    #endregion

    #region Drawing
    /// <summary>
    /// Draws a bolded title at position.
    /// </summary>
    /// <param name="position">Where to draw this.</param>
    /// <param name="label">Label to use as a title.</param>
    public static void TitleLabelField(ref Rect position, GUIContent label)
    {
        position.height = 24;
        EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
        position.TranslateY(position.height);
    }

    /// <summary>
    /// Draws a label at position.
    /// </summary>
    /// <inheritdoc cref="TitleLabelField(ref Rect, GUIContent)"/>
    public static void LabelField(ref Rect position, GUIContent label)
    {
        position.height = 24;
        EditorGUI.LabelField(position, label);
        position.TranslateY(position.height);
    }

    /// <summary>
    /// Draws the serialized property. Tries to look for custom property
    /// drawers.
    /// </summary>
    /// <param name="property">Property to draw.</param>
    /// <param name="position">Where to draw the property.</param>
    /// <param name="label">Label to use with custom property drawers, if found
    /// </param>
    public static void PropertyField(this SerializedProperty property,
        ref Rect position, GUIContent label)
    {
        // Try to find property drawer, if we can.
        PropertyDrawer drawer = PropertyDrawerFinder.FindDrawer(property);

        if (drawer != null)
        {
            // Found a custom property drawer. Use found drawer.
            position.height = drawer.GetPropertyHeight(property, label);
            EditorGUI.BeginProperty(position, label, property);
            drawer.OnGUI(position, property, label);
            EditorGUI.EndProperty();
        }
        else
        {
            // Did not find a custom property drawer. Use property field.
            position.height = EditorGUI.GetPropertyHeight(property);
            EditorGUI.PropertyField(position, property, true);
        }

        position.TranslateY(position.height);
    }

    /// <summary>
    /// Draws the serialized property. Tries to look for custom property
    /// drawers.
    /// </summary>
    /// <param name="property">Property to draw.</param>
    /// <param name="position">Where to draw the property.</param>
    public static void PropertyField(this SerializedProperty property,
        ref Rect position)
    {
        property.PropertyField(ref position, GUIContent.none);
    }

    /// <summary>
    /// Returns the height of the serialized property, trying to look for any
    /// custom height getters. If not found, returns defaultHeight.
    /// </summary>
    /// <param name="property">Property to get the height from.</param>
    /// <param name="label">The label.</param>
    /// <param name="defaultHeight">The default height
    /// (base.GetPropertyHeight(property, label);).</param>
    /// <returns></returns>
    public static float GetSerializedPropertyHeight(
        this SerializedProperty property, GUIContent label, float defaultHeight)
    {
        // Try to find property drawer, if we can.
        PropertyDrawer drawer = PropertyDrawerFinder.FindDrawer(property);

        if (drawer != null)
        {
            // Found a custom property drawer. Use found drawer.
            return drawer.GetPropertyHeight(property, label);
        }
        else
        {
            // Did not find a custom property drawer. Use default.
            return defaultHeight;
        }
    }

    /// <summary>
    /// Alias for <see cref="EditorGUI.GetPropertyHeight(SerializedProperty)"/>
    /// </summary>
    public static float GetPropertyHeight(this SerializedProperty property,
        GUIContent label = null, bool includeChildren = true) =>
        EditorGUI.GetPropertyHeight(property, label, includeChildren);

    /// <summary>
    /// Retrieves all immediate children of <paramref name="property"/>.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static IEnumerable<SerializedProperty> GetImmediateChildren(
        this SerializedProperty property, bool visibleOnly = true)
    {
        if (property.hasVisibleChildren)
        {
            var end = property.GetEndProperty();
            Next(property, visibleOnly, true);

            while (!SerializedProperty.EqualContents(property, end))
            {
                yield return property;

                Next(property, visibleOnly, false);
            }
        }
    }

    public static void Next(this SerializedProperty property,
        bool visibleOnly, bool enterChildren)
    {
        if (visibleOnly)
            property.NextVisible(enterChildren);
        else
            property.Next(enterChildren);
    }
    #endregion

    #region IO
    /// <summary>
    /// Creates and saves a scriptable object to the disk.
    /// </summary>
    /// <typeparam name="T">Type of scriptable object to create.</typeparam>
    /// <param name="path">Path to save the file to.</param>
    /// <returns>The created scriptable object.</returns>
    public static T CreateAndSaveScriptableObject<T>(
        string path = "Assets/ScriptableObjects") where T : ScriptableObject
    {
        var so = ScriptableObject.CreateInstance<T>();

        Directory.CreateDirectory(path);

        string fn = IOPath.ChangeExtension(typeof(T).ToString(), ".asset");
        AssetDatabase.CreateAsset(so, IOPath.Combine(path, fn));
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = so;
        return so;
    }
    #endregion
}
