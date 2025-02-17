using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Contains functions that extend types and reflection.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public static class TypeExt
{
    #region Query
    #region Type
    /// <summary>
    /// Gets the first name in the <see cref="CurrentAssembly"/> with the <see
    /// cref="Type.Name"/> defined by <paramref name="name"/>.
    /// </summary>
    /// <param name="name">Name of the type.</param>
    public static Type GetTypeByName(string name)
    {
        return CurrentAssembly.DefinedTypes.
            Select(
                (typeInfo) => typeInfo.AsType()
            ).First(
            (t) => t.Name == name
        );
    }
    #endregion

    #region Inherited Types
    /// <summary>
    /// Returns all types that can be inherited from <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The derivative type.</param>
    /// <returns>The derived types.</returns>
    public static IEnumerable<Type> FindInherited(this Type type)
    {
        return type.Assembly
            .GetTypes()
            .Where(t =>
                t != type &&
                type.IsAssignableFrom(t)
            );
    }

    /// <summary>
    /// Returns all types that can be derived from <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The derivative type.</typeparam>
    /// <returns>The derived types.</returns>
    public static IEnumerable<Type> FindInherited<T>()
        => FindInherited(typeof(T));

    /// <summary>
    /// Returns all concrete types (not abstract or interfaces) that can be
    /// derived from <paramref name="type"/>.
    /// </summary>
    /// <inheritdoc cref="FindInherited(Type)"/>
    public static IEnumerable<Type> FindInheritedConcrete(this Type type) =>
        FindInherited(type).Where(t => !t.IsAbstract && !t.IsInterface);

    /// <inheritdoc cref="FindInheritedConcrete(Type)"/>
    /// <inheritdoc cref="FindInherited{T}()"/>
    public static IEnumerable<Type> FindInheritedConcrete<T>() =>
        FindInheritedConcrete(typeof(T));
    #endregion

    #region Field Value
    /// <summary>
    /// Finds and returns the field value for <paramref name="obj"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The instance of the object.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="flags">Any flags.</param>
    /// <returns></returns>
    public static T GetFieldValue<T>(this object obj, string fieldName,
        BindingFlags flags = BindingFlags.Default)
    {
        var field = obj.GetType().GetField(fieldName, flags);

        return (T)field.GetValue(obj);
    }

    /// <summary>
    /// Tries to find the field value for <paramref name="obj"/>.
    /// </summary>
    /// <param name="fieldValue">If found, the field value will be placed here.
    /// Otherwise, null will be placed here.</param>
    /// <returns>True if field value was found.</returns>
    /// <inheritdoc cref="GetFieldValue{T}(object, string, BindingFlags)"/>
    public static bool TryGetFieldValue<T>(this object obj, string fieldName,
        BindingFlags flags, out T fieldValue)
    {
        var field = obj.GetType().GetField(fieldName, flags);

        if (field == null)
        {
            fieldValue = default;
            return false;
        }

        try
        {
            fieldValue = (T)field.GetValue(obj);
            return true;
        }
        catch (InvalidCastException)
        {
            fieldValue = default;
            return false;
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Attempt to obtain a parameterless constructor for the specified type.
    /// </summary>
    /// <param name="type">The specified type.</param>
    /// <param name="constructor">The constructor, or null if not found.</param>
    /// <param name="bindingFlags">The binding flags.</param>
    /// <returns>True if found, false otherwise.</returns>
    public static bool TryGetParameterlessConstructor(this Type type,
        out ConstructorInfo constructor,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        constructor = type.GetConstructor(
            bindingFlags,
            Type.DefaultBinder,
            Type.EmptyTypes,
            new ParameterModifier[] { }
        );
        return constructor != null;
    }

    /// <typeparam name="T">The specified type.</typeparam>
    /// <inheritdoc cref="TryGetParameterlessConstructor(Type, out
    /// ConstructorInfo, BindingFlags)"/>
    public static bool TryGetParameterlessConstructor<T>(
        out ConstructorInfo constructor,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) =>
        TryGetParameterlessConstructor(
            typeof(T),
            out constructor,
            bindingFlags
        );

    /// <summary>
    /// Attempt to call a parameterless constructor for the specified type.
    /// </summary>
    /// <param name="instance">The instance to assign to.</param>
    /// <inheritdoc cref="TryGetParameterlessConstructor(Type, out
    /// ConstructorInfo, BindingFlags)"/>
    public static bool CallParameterlessConstructor(this Type type,
        out object instance,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        bool hasConstr = type.TryGetParameterlessConstructor(
            out ConstructorInfo constructor,
            bindingFlags
        );

        if (hasConstr)
        {
            instance = constructor.Invoke(new object[] { });
            return true;
        }
        else
        {
            instance = null;
            return false;
        }
    }

    /// <inheritdoc cref="CallParameterlessConstructor(Type, out object,
    /// BindingFlags)"/>
    /// <inheritdoc cref="TryGetParameterlessConstructor{T}(out ConstructorInfo,
    /// BindingFlags)"/>
    public static bool CallParameterlessConstructor<T>(
        out T instance,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        bool hasConstr = CallParameterlessConstructor(
            typeof(T),
            out object nonGenericInstance,
            bindingFlags
        );
        instance = (T)nonGenericInstance;

        return hasConstr;
    }

    /// <returns>The newly created object of the specified type, or null if no
    /// parameterless constructor can be found.</returns>
    /// <inheritdoc cref="CallParameterlessConstructor(Type, out object,
    /// BindingFlags)"/>
    public static object CallParameterlessConstructor(this Type type,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        bool hasConstr = type.TryGetParameterlessConstructor(
            out ConstructorInfo constructor,
            bindingFlags
        );

        if (hasConstr)
        {
            return constructor.Invoke(new object[] { });
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc cref="CallParameterlessConstructor(Type, BindingFlags)"/>
    public static T CallParameterlessConstructor<T>(
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        return (T)CallParameterlessConstructor(typeof(T), bindingFlags);
    }
    #endregion
    #endregion

    #region Type Testing
    /// <summary>
    /// Determines if <paramref name="type"/> is a struct.
    /// </summary>
    public static bool IsStruct(this Type type) =>
        type.IsValueType && !type.IsEnum;

    /// <summary>
    /// Determines if <paramref name="type"/> is a class or struct.
    /// </summary>
    public static bool IsClassOrStruct(this Type type) =>
        type.IsClass || type.IsStruct();
    #endregion

    #region Assembly
    /// <summary>
    /// The current executing assembly.
    /// </summary>
    public static Assembly CurrentAssembly => Assembly.GetExecutingAssembly();
    #endregion
}
