using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using System.IO;

/// <summary>
/// Contains methods that extend game objects or other unity objects.
/// </summary>
public static class UnityObjectExt
{
    #region Query
    #region Query Single

    #region In Self
    /// <summary>
    /// Checks if gameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool HasComponent<T>(this GameObject gameObject,
        bool includeInactive = false)
    {
        return !gameObject.GetComponent<T>().IsUnityNull();
    }

    /// <summary>
    /// Checks if self's GameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool HasComponent<T>(this Component self,
        bool includeInactive = false)
    {
        return self.gameObject.HasComponent<T>(includeInactive);
    }

    /// <summary>
    /// Checks if gameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool HasComponent<T>(this GameObject gameObject,
        out T component)
    {
        component = gameObject.GetComponent<T>();
        return !component.IsUnityNull();
    }

    /// <summary>
    /// Checks if gameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool HasComponent<T>(this Component self, out T component)
    {
        component = self.GetComponent<T>();
        return !component.IsUnityNull();
    }
    #endregion

    #region In Parent
    /// <summary>
    /// Checks if gameObject has the specified component in its parent.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// parent.</returns>
    public static bool HasComponentInParent<T>(this GameObject gameObject,
        bool includeInactive = false)
    {
        return !gameObject.GetComponentInParent<T>(includeInactive)
            .IsUnityNull();
    }

    /// <summary>
    /// Checks if gameObject has the specified component in its children.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// children.</returns>
    public static bool HasComponentInParent<T>(this Component self,
        bool includeInactive = false)
    {
        return self.gameObject.HasComponentInParent<T>(includeInactive);
    }

    /// <summary>
    /// Checks if gameObject has the specified component in its parent.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// parent.</returns>
    public static bool HasComponentInParent<T>(this GameObject gameObject,
        out T component, bool includeInactive = false)
    {
        component = gameObject.GetComponentInParent<T>(includeInactive);
        return !component.IsUnityNull();
    }

    /// <summary>
    /// Checks if gameObject has the specified component in its children.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// children.</returns>
    public static bool HasComponentInParent<T>(this Component self,
        out T component, bool includeInactive = false)
    {
        return self.gameObject.HasComponentInParent<T>(out component, includeInactive);
    }

    #region Any Parent
    /// <summary>
    /// Checks if <paramref name="self"/> has the specified component
    /// in any of its parents or itself.
    /// </summary>
    /// <inheritdoc cref="HasComponentInParent{T}(GameObject, out T, bool)"/>
    public static bool HasComponentInAnyParent<T>(this GameObject self,
        out T component, bool includeInactive = false)
        where T : Component
    {
        foreach (var parent in self.transform.Parents())
        {
            if ((includeInactive || parent.gameObject.activeInHierarchy)
                && parent.HasComponent(out component))
            {
                return true;
            }
        }

        component = null;
        return false;
    }

    /// <inheritdoc cref="HasComponentInAnyParent{T}(GameObject, out T)"/>
    public static bool HasComponentInAnyParent<T>(this Component self,
        out T component, bool includeInactive = false)
        where T : Component
    {
        return self.gameObject.HasComponentInAnyParent(out component, includeInactive);
    }

    /// <summary>
    /// Returns the specified component of type <typeparamref name="T"/>,
    /// if it exists within self or any of its parents.
    /// </summary>
    /// <returns>The component, or null if it's not found.</returns>
    /// <inheritdoc cref="HasComponentInParent{T}(GameObject, out T, bool)"/>
    public static T GetComponentInAnyParent<T>(this GameObject self,
        bool includeInactive = false)
        where T : Component
    {
        self.HasComponentInAnyParent(out T component, includeInactive);
        return component;
    }

    /// <inheritdoc cref="GetComponentInAnyParent{T}(GameObject, bool)"/>
    public static T GetComponentInAnyParent<T>(this Component self,
        bool includeInactive = false)
        where T : Component
    {
        return self.gameObject.GetComponentInAnyParent<T>(includeInactive);
    }
    #endregion
    #endregion

    #region In Children
    /// <summary>
    /// Checks if gameObject has the specified component in its children.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// children.</returns>
    public static bool HasComponentInChildren<T>(this GameObject gameObject,
        bool includeInactive = false)
    {
        return !gameObject.GetComponentInChildren<T>(includeInactive)
            .IsUnityNull();
    }

    /// <summary>
    /// Checks if gameObject has the specified component in its children.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// children.</returns>
    public static bool HasComponentInChildren<T>(this Component self,
        bool includeInactive = false)
    {
        return self.gameObject.HasComponentInChildren<T>(includeInactive);
    }

    /// <summary>
    /// Checks if gameObject has the specified component in its children.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// children.</returns>
    public static bool HasComponentInChildren<T>(this GameObject gameObject,
        out T component, bool includeInactive = false)
    {
        component = gameObject.GetComponentInChildren<T>(includeInactive);
        return !component.IsUnityNull();
    }

    /// <summary>
    /// Checks if gameObject has the specified component in its children.
    /// </summary>
    /// <typeparam name="T">Type of the component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component in its
    /// children.</returns>
    public static bool HasComponentInChildren<T>(this Component self,
        out T component, bool includeInactive = false)
    {
        return self.gameObject.HasComponentInChildren<T>(out component,
            includeInactive);
    }
    #endregion

    #region In Scene
    /// <summary>
    /// Attempts to locate a gameobject with the specified <paramref name="tag"/>.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <param name="gameObject">The reference to the gameobject is found, otherwise null.</param>
    /// <returns>True if gameobject was found, false otherwise.</returns>
    public static bool ExistsWithTag(string tag, out GameObject gameObject)
    {
        try
        {
            gameObject = GameObject.FindWithTag(tag);

            return gameObject != null;
        }
        catch (UnityException e)
        {
            Debug.LogError($"Tag {tag} is not defined!");
            throw e;
        }
    }

    /// <summary>
    /// Attempts to locate a gameobject with the specified <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <inheritdoc cref="ExistsWithTag(string, out GameObject)"/>
    public static bool ExistsWithName(string name, out GameObject gameObject)
    {
        gameObject = GameObject.Find(name);
        return gameObject != null;
    }
    #endregion

    #endregion

    #region Query Multiple
    /// <summary>
    /// Returns an array of all objects with a certain type.
    /// </summary>
    /// <typeparam name="T">The component to look for.</typeparam>
    /// <param name="array">Array of GameObjects to look through.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>An array of GameObjects with only the type of components
    /// specified.</returns>
    public static IEnumerable<GameObject> WithComponent<T>(
        this IEnumerable<GameObject> array,
        bool includeInactive = false)
    {
        return array.Where(go => go.HasComponent<T>(includeInactive));
    }

    /// <summary>
    /// Returns an array of all objects with a certain tag.
    /// </summary>
    /// <param name="array">Array of GameObjects to look through.</param>
    /// <param name="tagName">Name of tag to search for.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>An array of GameObjects with only the tags.</returns>
    public static IEnumerable<GameObject> WithTag(
        this IEnumerable<GameObject> array,
        string tagName, bool includeInactive = false)
    {
        return array.Where(go => go.CompareTag(tagName));
    }
    #endregion
    #endregion

    #region Autoadd
    #region Add If Missing
    /// <summary>
    /// Adds a component of type <typeparamref name="T"/> if not found.
    /// </summary>
    /// <typeparam name="T">The type of component to find/add.</typeparam>
    /// <param name="gameObject">The gameobject where we search for and add the
    /// component.</param>
    /// <returns>The found/added component of type <typeparamref
    /// name="T"/>.</returns>
    public static T AddComponentIfMissing<T>(this GameObject gameObject)
        where T : Component
    {
        if (!gameObject.HasComponent(out T component))
        {
            return gameObject.AddComponent<T>();
        }
        else
        {
            return component;
        }
    }

    /// <inheritdoc cref="AddComponentIfMissing{T}(GameObject)"/>
    /// <param name="component">Either the newly added component or an existing
    /// one.</param>
    /// <returns>True if the component was already present, false if the
    /// component had to be added.</returns>
    public static bool AddComponentIfMissing<T>(this GameObject gameObject,
        out T component) where T : Component
    {
        T thing = gameObject.GetComponent<T>();
        if (thing.IsUnityNull())
        {
            component = gameObject.AddComponent<T>();
            return false;
        }
        else
        {
            component = thing;
            return true;
        }
    }

    /// <inheritdoc cref="AddComponentIfMissing{T}(GameObject)"/>
    /// <param name="self">The component whose gameobject is used to search for
    /// and add the component.</param>
    public static T AddComponentIfMissing<T>(this Component self)
        where T : Component =>
        self.gameObject.AddComponentIfMissing<T>();

    /// <inheritdoc cref="AddComponentIfMissing{T}(Component)"/>
    /// <inheritdoc cref="AddComponentIfMissing{T}(GameObject, out T)"/>
    public static bool AddComponentIfMissing<T>(this Component self,
        out T component) where T : Component =>
        self.gameObject.AddComponentIfMissing(out component);
    #endregion

    #region Require Component
    #region In Self
    /// <summary>
    /// Checks if self's GameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of required component.</typeparam>
    /// <param name="self">Component whose GameObject will be used to
    /// search for the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool RequireComponent<T>(
        this Component self,
        out T component,
        bool doError = true)
    {
        return self.RequireComponent(out component, typeof(T).ToString(),
            doError);
    }

    /// <inheritdoc cref="RequireComponent{T}(Component, out T, bool)"/>
    /// <param name="gameObject">GameObject used to search for the
    /// component.</param>
    public static bool RequireComponent<T>(
        this GameObject gameObject,
        out T component,
        bool doError = true)
    {
        return gameObject.RequireComponent(out component, typeof(T).ToString(),
            doError);
    }

    /// <summary>
    /// Checks if self's GameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="name">Name of the component that we are looking
    /// for.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool RequireComponent<T>(
        this Component self,
        out T component,
        string name,
        bool doError = true)
    {
        return self.gameObject.RequireComponent(out component, name, doError);
    }

    /// <summary>
    /// Checks if gameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="name">Name of the component that we are looking
    /// for.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool RequireComponent<T>(
        this GameObject gameObject,
        out T component,
        string name,
        bool doError = true)
    {
        if (gameObject.HasComponent(out component))
        {
            return true;
        }
        else
        {
            string errorMessage =
                $"{gameObject} is missing required component {name}.";

            if (doError)
                throw new MissingComponentException(errorMessage);
            else
                Debug.LogWarning(errorMessage, gameObject);

            return false;
        }
    }

    /// <summary>
    /// Instantiates prefab with a required component, 
    /// Checks if <paramref name="prefab"/> has the required <paramref
    /// name="componentInstance"/>, then instantiates the prefab and sets the
    /// value of <paramref name="componentInstance"/> to the new instance value.
    /// </summary>
    /// <inheritdoc cref="RequireComponent"/>
    /// <param name="prefab">GameObject to search for the component.</param>
    /// <param name="componentInstance">Set to the instance of the component if
    /// found.</param>
    /// <returns></returns>
    public static bool InstantiateWithComponent<T>(
        this GameObject prefab,
        out T componentInstance,
        string name,
        bool doError = true) where T : Component
    {
        bool hasComponent = RequireComponent(
            prefab, out T componentPrefab,
            name, doError
        );

        componentInstance = hasComponent ?
            componentPrefab.InstantiateComponent() :
            null;

        return hasComponent;
    }

    /// <inheritdoc cref="InstantiateWithComponent{T}(GameObject, out T,
    /// string, bool)"/>
    public static bool InstantiateWithComponent<T>(
        this GameObject prefab,
        out T componentInstance,
        bool doError = true) where T : Component
    {
        return prefab.InstantiateWithComponent(
            out componentInstance,
            typeof(T).ToString(),
            doError
        );
    }
    #endregion

    #region In Children
    /// <summary>
    /// Checks if self's GameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="gameObject">Component whose GameObject will be used to
    /// search for the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool RequireComponentInChildren<T>(
        this Component self,
        out T component,
        bool doError = true,
        bool includeInactive = false)
    {
        return self.RequireComponentInChildren(out component,
            typeof(T).ToString(), doError, includeInactive);
    }

    /// <summary>
    /// Checks if self's GameObject has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="gameObject">GameObject used to search for the
    /// component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject has the specified component.</returns>
    public static bool RequireComponentInChildren<T>(
        this GameObject gameObject,
        out T component,
        bool doError = true,
        bool includeInactive = false)
    {
        return gameObject.RequireComponentInChildren(out component,
            typeof(T).ToString(), doError, includeInactive);
    }

    /// <summary>
    /// Checks if self's gameObject or its children has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="name">Name of the component that we are looking
    /// for.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject or its children has the specified
    /// component.</returns>
    public static bool RequireComponentInChildren<T>(
        this Component self,
        out T component,
        string name,
        bool doError = true,
        bool includeInactive = false)
    {
        return self.gameObject.RequireComponentInChildren(out component,
            name, doError, includeInactive);
    }

    /// <summary>
    /// Checks if gameObject or its children has the specified component.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="gameObject">GameObject to search for the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="name">Name of the component that we are looking
    /// for.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <param name="includeInactive">If true, include inactive GameObjects when
    /// searching. Otherwise, do not include them.</param>
    /// <returns>True if gameObject or its children has the specified
    /// component.</returns>
    public static bool RequireComponentInChildren<T>(
        this GameObject gameObject,
        out T component,
        string name,
        bool doError = true,
        bool includeInactive = false)
    {
        if (gameObject.HasComponentInChildren(out component, includeInactive))
        {
            return true;
        }
        else
        {
            string errorMessage =
                $"{gameObject} is missing required component {name} in children.";

            if (doError)
                Debug.LogError(errorMessage, gameObject);
            else
                Debug.LogWarning(errorMessage, gameObject);

            return false;
        }
    }
    #endregion
    #endregion

    #region Add To Manager Index
    /// <summary>
    /// If a MonoBehavior of type T already exists within managerCache, return
    /// that MonoBehavior. Otherwise, attempts to find the specified manager in
    /// self or its children. If found, instantiates a singleton for that
    /// manager. Unlike <see cref="InstantiateSingleton{T}(T, ref T, bool)"/>,
    /// this does not throw errors.
    /// </summary>
    ///
    /// <typeparam name="T">The type of manager. Must be a
    /// MonoBehaviour.</typeparam>
    /// <param name="self">The manager container who is the parent of
    /// manager.</param>
    /// <param name="manager">Will be set to the value of the manager.</param>
    /// <param name="managerCache">The cache to put the manager once it is
    /// instantiated, if dontDestroyOnLoad is true. <see
    /// cref="AddToManagerIndex{T}(MonoBehaviour, out T, ref Dictionary{Type,
    /// MonoBehaviour}, bool)"/> uses this cache to locate existing managers
    /// listed as DontDestroyOnLoad. It is recommended that this cache be static
    /// so it is not deleted on scene load.
    /// </param>
    /// <param name="dontDestroyOnLoad">If true, add manager to cache and set it
    /// to DontDestroyOnLoad.</param>
    public static void AddToManagerIndex<T>(this MonoBehaviour self,
        out T manager, ref Dictionary<Type, MonoBehaviour> managerCache,
        bool dontDestroyOnLoad) where T : MonoBehaviour
    {
        if (!Application.isPlaying)
        {
            throw new InvalidOperationException(
                "Cannot create a manager instance for " +
                $"{typeof(T).Name} in editor mode."
            );
        }

        self.RequireComponentInChildren(out T tempManager);

        if (dontDestroyOnLoad)
        {
            managerCache ??= new();

            if (managerCache.ContainsKey(typeof(T)) &&
                managerCache[typeof(T)] != null)
            {
                // Set manager from the managerCache.
                manager = (T)managerCache[typeof(T)];

                if (tempManager != null)
                {
                    // However, we seem to already have another manager in self.
                    // Delete this manager.
                    Debug.LogWarning($"Found another instance of {typeof(T)}. " +
                        "Deleting...", manager);

                    GameObject.Destroy(tempManager.gameObject);
                }
                return;
            }

            // We have a brand new manager.
            manager = tempManager;

            // Set as DoNotDestroyOnLoad
            manager.transform.Orphan();
            GameObject.DontDestroyOnLoad(manager.gameObject);

            // Now set the value in the cache to the new manager.
            managerCache[typeof(T)] = manager;
        }
        else
        {
            // Managers that are destroyed on load won't cause problems, as they
            // can't reload.
            manager = tempManager;
        }
    }
    #endregion

    #region Autofill
    /// <summary>
    /// Does the same thing as <see cref="RequireComponent"/>, but does not set
    /// the value of <paramref name="component"/> if it is already set to a
    /// valid value.
    /// </summary>
    /// <typeparam name="T">Type of component to get.</typeparam>
    /// <param name="self">Component whose GameObject will be used to search for
    /// the component.</param>
    /// <param name="component">Set to the component if found.</param>
    /// <param name="name">Name of the component that we are looking
    /// for.</param>
    /// <param name="doError">If true, log as an error. Otherwise, log as a
    /// warning.</param>
    /// <returns>True if gameObject has the specified component or if the
    /// component is already specified.</returns>
    public static bool AutofillComponent<T>(
        this Component self,
        ref T component,
        string name,
        bool doError = true
        ) where T : Component
    {
        return component ?
            true :
            self.RequireComponent(out component, name, doError);
    }

    /// <inheritdoc cref="AutofillComponent{T}(Component, ref T, string,
    /// bool)"/>
    /// <param name="gameObject">GameObject which will be used to search for the
    /// component.</param>
    public static bool AutofillComponent<T>(
        this GameObject gameObject,
        ref T component,
        string name,
        bool doError = true
        ) where T : Component
    {
        if (component)
            return true;
        else
        {
            return gameObject.RequireComponent(out component, name, doError);
        }
    }

    /// <inheritdoc cref="AutofillComponent{T}(Component, ref T, string,
    /// bool)"/>
    public static bool AutofillComponent<T>(
        this Component self,
        ref T component,
        bool doError = true
        ) where T : Component
    {
        return self.AutofillComponent(
            ref component,
            typeof(T).ToString(),
            doError
        );
    }

    /// <inheritdoc cref="AutofillComponent{T}(GameObject, ref T, string,
    /// bool)"/>
    public static bool AutofillComponent<T>(
        this GameObject self,
        ref T component,
        bool doError = true
        ) where T : Component
    {
        return self.AutofillComponent(
            ref component,
            typeof(T).ToString(),
            doError
        );
    }

    /// <summary>
    /// Does the same thing as <see cref="RequireComponentInChildren"/>, but
    /// does not set the value of <paramref name="component"/> if it is already
    /// set to a valid value.
    /// </summary>
    /// <inheritdoc cref="AutofillComponent{T}(Component, ref T, string,
    /// bool)"/>
    public static bool AutofillComponentInChildren<T>(
        this Component self,
        ref T component,
        string name,
        bool doError = true
        ) where T : Component
    {
        return component ?
            true :
            self.RequireComponentInChildren(out component, name, doError);
    }

    /// <inheritdoc cref="AutofillComponentInChildren{T}(Component, ref T,
    /// string, bool)"/>
    public static bool AutofillComponentInChildren<T>(
        this Component self,
        ref T component,
        bool doError = true
        ) where T : Component
    {
        return self.AutofillComponentInChildren(
            ref component,
            typeof(T).ToString(),
            doError
        );
    }

    /// <inheritdoc cref="AutofillComponentInChildren{T}(Component, ref T,
    /// string, bool)"/>
    public static bool AutofillComponentInChildren<T>(
        this GameObject self,
        ref T component,
        string name,
        bool doError = true
        ) where T : Component
    {
        return component ?
            true :
            self.RequireComponentInChildren(out component, name, doError);
    }

    /// <inheritdoc cref="AutofillComponentInChildren{T}(GameObject, ref T,
    /// string, bool)"/>
    public static bool AutofillComponentInChildren<T>(
        this GameObject self,
        ref T component,
        bool doError = true
        ) where T : Component
    {
        return self.AutofillComponentInChildren(
            ref component,
            typeof(T).ToString(),
            doError
        );
    }
    #endregion
    #endregion

    #region Instantiation
    /// <summary>
    /// Instantiates the gameobject belonging to reference, then returns the
    /// instantiated reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reference">The prefab or other reference to instantiate.
    /// This will be unchanged.</param>
    /// <returns>The newly instantiated component of type <typeparamref
    /// name="T"/>.</returns>
    public static T InstantiateComponent<T>(this T reference) where T : Component
    {
        return reference.gameObject.InstantiateComponent<T>();
    }

    /// <inheritdoc cref="InstantiateComponent{T}(T)"/>
    /// <inheritdoc cref="UnityEngine.Object.Instantiate(UnityEngine.Object, Transform, bool)"/>
    public static T InstantiateComponent<T>(this T reference, Transform parent,
        bool instantiateInWorldSpace) where T : Component
    {
        return reference.gameObject.InstantiateComponent<T>(
            parent,
            instantiateInWorldSpace
        );
    }

    /// <inheritdoc cref="InstantiateComponent{T}(T)"/>
    /// <inheritdoc cref="UnityEngine.Object.Instantiate(UnityEngine.Object, Vector3, Quaternion)"/>
    public static T InstantiateComponent<T>(this T reference, Vector3 position,
        Quaternion rotation) where T : Component
    {
        return reference.gameObject.InstantiateComponent<T>(
            position,
            rotation
        );
    }

    /// <inheritdoc cref="InstantiateComponent{T}(T)"/>
    /// <inheritdoc cref="UnityEngine.Object.Instantiate(UnityEngine.Object, Vector3, Quaternion, Transform)"/>
    public static T InstantiateComponent<T>(this T reference, Vector3 position,
        Quaternion rotation, Transform parent) where T : Component
    {
        if (!reference) throw new ArgumentNullException(
            nameof(reference),
            "Reference has been destroyed or is null."
        );
        if (!parent) throw new ArgumentNullException(
            nameof(parent),
            "Parent has been destroyed or is null."
        );
        if (!reference.gameObject) throw new NullReferenceException(
            $"GameObject of reference ({reference}) has been destroyed or " +
            $"is null."
        );

        GameObject.Instantiate(
            reference.gameObject,
            position,
            rotation,
            parent
        ).RequireComponent(out T instance);
        return instance;
    }

    /// <inheritdoc cref="InstantiateComponent{T}(T)"/>
    /// <param name="gameobject">The gameobject prefab to instantiate.
    /// This will be unchanged.</param>
    public static T InstantiateComponent<T>(this GameObject gameobject)
        where T : Component
    {
        if (!gameobject) throw new ArgumentNullException(
            nameof(gameobject),
            "GameObject has been destroyed or is null."
        );

        GameObject.Instantiate(gameobject).RequireComponent(out T instance);
        return instance;
    }

    /// <inheritdoc cref="InstantiateComponent{T}(GameObject)"/>
    /// <inheritdoc cref="UnityEngine.Object.Instantiate(UnityEngine.Object,
    /// Transform, bool)"/>
    public static T InstantiateComponent<T>(this GameObject gameobject,
        Transform parent, bool instantiateInWorldSpace) where T : Component
    {
        if (!gameobject) throw new ArgumentNullException(
            nameof(gameobject),
            "GameObject has been destroyed or is null."
        );

        GameObject.Instantiate(
            gameobject,
            parent,
            instantiateInWorldSpace
        ).RequireComponent(out T instance);
        return instance;
    }

    /// <inheritdoc cref="InstantiateComponent{T}(GameObject, Vector3,
    /// Quaternion)"/>
    public static T InstantiateComponent<T>(this GameObject gameobject,
        Vector3 position) where T : Component =>
        gameobject.InstantiateComponent<T>(position, Quaternion.identity);

    /// <inheritdoc cref="InstantiateComponent{T}(GameObject, Vector3,
    /// Quaternion)"/>
    public static T InstantiateComponent<T>(this GameObject gameobject,
        Quaternion rotation) where T : Component =>
        gameobject.InstantiateComponent<T>(Vector3.zero, rotation);


    /// <inheritdoc cref="InstantiateComponent{T}(GameObject)"/>
    /// <inheritdoc cref="UnityEngine.Object.Instantiate(UnityEngine.Object,
    /// Vector3, Quaternion)"/>
    public static T InstantiateComponent<T>(this GameObject gameobject,
        Vector3 position, Quaternion rotation) where T : Component
    {
        if (!gameobject) throw new ArgumentNullException(
            nameof(gameobject),
            "GameObject has been destroyed or is null."
        );

        GameObject.Instantiate(
            gameobject,
            position,
            rotation
        ).RequireComponent(out T instance);
        return instance;
    }

    /// <inheritdoc cref="InstantiateComponent{T}(T)"/>
    /// <inheritdoc cref="UnityEngine.Object.Instantiate(UnityEngine.Object, Vector3, Quaternion, Transform)"/>
    public static T InstantiateComponent<T>(this GameObject gameobject, Vector3 position,
        Quaternion rotation, Transform parent) where T : Component
    {
        if (!gameobject) throw new ArgumentNullException(
            nameof(gameobject),
            "GameObject has been destroyed or is null."
        );

        GameObject.Instantiate(
            gameobject,
            position,
            rotation,
            parent
        ).RequireComponent(out T instance);
        return instance;
    }
    #endregion

    #region Construction
    /// <summary>
    /// Creates a new GameObject named <paramref name="name"/> with one or more
    /// defined component.
    /// </summary>
    /// <typeparam name="T1">The first parameter type.</typeparam>
    /// <typeparam name="T2">The second parameter type.</typeparam>
    /// <typeparam name="T3">The third parameter type.</typeparam>
    /// <typeparam name="T4">The fourth parameter type.</typeparam>
    /// <typeparam name="T5">The fifth parameter type.</typeparam>
    /// <typeparam name="T6">The sixth parameter type.</typeparam>
    /// <param name="name">The name of the new GameObject.</param>
    /// <param name="component1">The first component.</param>
    /// <param name="component2">The second component.</param>
    /// <param name="component3">The third component.</param>
    /// <param name="component4">The fourth component.</param>
    /// <param name="component5">The fifth component.</param>
    /// <param name="component6">The sixth component.</param>
    /// <returns>The new GameObject.</returns>
    public static GameObject Create<T1, T2, T3, T4, T5, T6>(string name,
        out T1 component1, out T2 component2, out T3 component3,
        out T4 component4, out T5 component5, out T6 component6)
        where T1 : Component where T2 : Component where T3 : Component
        where T4 : Component where T5 : Component where T6 : Component
    {
        GameObject obj = new(name,
            typeof(T1), typeof(T2), typeof(T3),
            typeof(T4), typeof(T5), typeof(T6)
        );
        obj.RequireComponent(out component1);
        obj.RequireComponent(out component2);
        obj.RequireComponent(out component3);
        obj.RequireComponent(out component4);
        obj.RequireComponent(out component5);
        obj.RequireComponent(out component6);
        return obj;
    }

    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5, T6}(string, out T1,
    /// out T2, out T3, out T4, out T5, out T6)"/>
    public static GameObject Create<T1, T2, T3, T4, T5>(string name,
        out T1 component1, out T2 component2, out T3 component3,
        out T4 component4, out T5 component5)
        where T1 : Component where T2 : Component where T3 : Component
        where T4 : Component where T5 : Component
    {
        GameObject obj = new(name,
            typeof(T1), typeof(T2), typeof(T3),
            typeof(T4), typeof(T5)
        );
        obj.RequireComponent(out component1);
        obj.RequireComponent(out component2);
        obj.RequireComponent(out component3);
        obj.RequireComponent(out component4);
        obj.RequireComponent(out component5);
        return obj;
    }

    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5, T6}(string, out T1,
    /// out T2, out T3, out T4, out T5, out T6)"/>
    public static GameObject Create<T1, T2, T3, T4>(string name,
        out T1 component1, out T2 component2,
        out T3 component3, out T4 component4)
        where T1 : Component where T2 : Component
        where T3 : Component where T4 : Component
    {
        GameObject obj = new(name,
            typeof(T1), typeof(T2),
            typeof(T3), typeof(T4)
        );
        obj.RequireComponent(out component1);
        obj.RequireComponent(out component2);
        obj.RequireComponent(out component3);
        obj.RequireComponent(out component4);
        return obj;
    }

    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5, T6}(string, out T1,
    /// out T2, out T3, out T4, out T5, out T6)"/>
    public static GameObject Create<T1, T2, T3>(string name,
        out T1 component1, out T2 component2, out T3 component3)
        where T1 : Component where T2 : Component where T3 : Component
    {
        GameObject obj = new(name,
            typeof(T1), typeof(T2), typeof(T3)
        );
        obj.RequireComponent(out component1);
        obj.RequireComponent(out component2);
        obj.RequireComponent(out component3);
        return obj;
    }

    /// <inheritdoc cref="Create{T1, T2, T3, T4, T5, T6}(string, out T1,
    /// out T2, out T3, out T4, out T5, out T6)"/>
    public static GameObject Create<T1, T2>(string name,
        out T1 component1, out T2 component2)
        where T1 : Component where T2 : Component
    {
        GameObject obj = new(name,
            typeof(T1), typeof(T2)
        );
        obj.RequireComponent(out component1);
        obj.RequireComponent(out component2);
        return obj;
    }

    /// <inheritdoc cref="Create{T1, T2}(string, out T1, out T2)"/>
    public static GameObject Create<T1>(string name, out T1 component1)
    {
        GameObject obj = new(name, typeof(T1));
        obj.RequireComponent(out component1);
        return obj;
    }
    #endregion

    #region Asset Manipulation
    /// <summary>
    /// Creates and saves an unity object as an asset.
    /// </summary>
    /// <param name="asset">The object to make into an asset.</param>
    /// <param name="filePath">Path of the asset.</param>
    public static void SaveAsset(UnityEngine.Object asset, string filePath)
    {
        string folderPath = Path.GetDirectoryName(filePath);
        Directory.CreateDirectory(folderPath);

        AssetDatabase.CreateAsset(asset, filePath);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Loads or creates a "scriptable asset". If <paramref name="filePath"/>
    /// points to an asset, loads that asset as a scriptable object. Otherwise,
    /// creates the asset and saves it to <paramref name="filePath"/>.
    /// </summary>
    /// <param name="scriptable">The scriptable object to assign to.</param>
    /// <param name="filePath">Path of the asset.</param>
    /// <typeparam name="T">The type of scriptable object to create.</typeparam>
    /// <returns>The created scriptable asset.</returns>
    public static void ScriptableAsset<T>(ref T scriptable, string filePath)
        where T : ScriptableObject
    {
        if (File.Exists(filePath))
        {
            if (scriptable)
                return;

            // File exists. Load it.
            scriptable = AssetDatabase.LoadAssetAtPath<T>(filePath);
        }
        else
        {
            // Create the asset.
            scriptable = ScriptableObject.CreateInstance<T>();
            SaveAsset(scriptable, filePath);
        }
    }
    #endregion

    #region Checks
    /// <summary>
    /// Checks if an object either 
    /// <list type="bullet">
    /// <item>Is null, if it is not a <see cref="UnityEngine.Object"/>.</item>
    /// <item>Is destroyed, not assigned, or newly created, if it is.</item>
    /// </list>
    ///
    /// Unity overloads the == operator for <see cref="UnityEngine.Object"/>,
    /// and returns true for a == null both if a is null, or if it doesn't exist
    /// in the c++ engine. This method is for checking for either of those being
    /// the case for objects that are not necessarily UnityEngine.Objects. This
    /// is useful when you're using interfaces, since == is a static method, so
    /// if you check if a member of an interface == null, it will hit the
    /// default C# == check instead of the overridden Unity check.
    /// </summary>
    /// <remarks>
    /// Source: <see
    /// href="https://forum.unity.com/threads/when-a-rigid-body-is-not-attached-component-getcomponent-rigidbody-returns-null-as-a-string.521633/#post-3423328"/>
    /// Summary from source.
    /// </remarks>
    /// <param name="obj">Object to check</param>
    /// <returns>True if the object is null, or if it's a <see
    /// cref="UnityEngine.Object"/> that has been destroyed</returns>
    public static bool IsUnityNull(this object obj) =>
        obj is UnityEngine.Object uObj ? !uObj : obj == null;

    /// <summary>
    /// A usable pseudo null conditional operator (see <see
    /// href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-"/>)
    /// that supports <see cref="UnityEngine.Object"/>. Supports both member and
    /// element access.
    /// </summary>
    /// <typeparam name="U">The <see cref="UnityEngine.Object"/>
    /// type.</typeparam>
    /// <typeparam name="M">The member type of <typeparamref
    /// name="U"/>.</typeparam>
    /// <param name="obj">The <see cref="UnityEngine.Object"/>.</param>
    /// <param name="member">The member type of <paramref name="obj"/></param>
    /// <returns></returns>
    public static M NullCond<U, M>(this U obj,
        Func<U, M> member)
        where U : UnityEngine.Object
        where M : class =>
            obj ? member(obj) : null;

    /// <summary>
    /// Determines if <paramref name="unityObject"/> exists (is not null) and is
    /// <see cref="GameObject.activeInHierarchy"/>.
    /// </summary>
    /// <param name="unityObject">The object to check.</param>
    /// <returns></returns>
    public static bool ExistsAndActive(this GameObject unityObject) =>
        unityObject && unityObject.activeInHierarchy;

    /// <summary>
    /// Determines if <paramref name="unityBehaviour"/> exists (is not null) and
    /// is <see cref="Behaviour.enabled"/>.
    /// </summary>
    /// <param name="unityBehaviour">The object to check.</param>
    /// <returns></returns>
    public static bool ExistsAndEnabled(this Behaviour unityBehaviour) =>
        unityBehaviour && unityBehaviour.enabled;

    /// <summary>
    /// Returns true if <paramref name="unityObject"/> is a prefab, not an
    /// instance (ie it does not exist within any scene).
    /// </summary>
    /// <param name="unityObject">The object to check.</param>
    public static bool IsPrefab(this GameObject unityObject) =>
        unityObject.scene.name == null;

    /// <summary>
    /// Returns true if the gameobject containing <paramref name="behaviour"/>
    /// is a prefab, not an instance (ie it does not exist within any scene).
    /// </summary>
    /// <param name="behaviour">The monobehaviour  to check.</param>
    public static bool IsPrefab(this MonoBehaviour behaviour) =>
        behaviour.gameObject.IsPrefab();
    #endregion

    #region Copy
    /// <summary>
    /// Copies a component to <paramref name="target"/>. Adapted from
    /// http://answers.unity.com/answers/1118416/view.html
    /// </summary>
    /// <typeparam name="T">A component.</typeparam>
    /// <param name="original">Reference to the component to copy.</param>
    public static void CopyComponentTo<T>(this T original, T target)
        where T : Component
    {
        Type type = original.GetType();
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic)
                continue;

            field.SetValue(target, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name")
                continue;

            try
            {
                prop.SetValue(target, prop.GetValue(original, null), null);
            }
            catch (Exception)
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Copies a component and adds it to destination. Adapted from
    /// http://answers.unity.com/answers/1118416/view.html
    /// </summary>
    /// <typeparam name="T">A component.</typeparam>
    /// <param name="original">Reference to the component to copy.</param>
    /// <param name="destination">Where to add the component.</param>
    /// <returns></returns>
    public static T CopyComponent<T>(this T original, GameObject destination)
        where T : Component
    {
        Type type = original.GetType();
        var dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic)
                continue;

            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name")
                continue;

            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst;
    }

    /// <summary>
    /// Copies <paramref name="component"/> into a new child gameobject of its
    /// current gameobject.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component">The component to copy.</param>
    /// <param name="deleteOld">If true, remove the old component.</param>
    /// <param name="name">The name to give to the new child gameobject.</param>
    /// <returns>The new child gameobject.</returns>
    public static GameObject CopyIntoNewChild<T>(this T component, bool deleteOld,
        string name) where T : Component
    {
        GameObject parent = component.gameObject;
        GameObject child = new(name);

        child.transform.SetParent(parent.transform, false);
        child.transform.localPosition = Vector3.zero;

        component.CopyComponent(child);

        if (deleteOld)
        {
            GameObject.Destroy(component);
        }

        return child;
    }

    /// <inheritdoc cref="CopyIntoNewChild{T}(T, bool, string)"/>
    public static GameObject CopyIntoNewChild<T>(this T component,
        bool deleteOld) where T : Component
    {
        return CopyIntoNewChild(component, deleteOld, component.GetType().Name);
    }
    #endregion

    #region Singleton
    /// <inheritdoc cref="InstantiateSingleton{T}(T, ref T, bool, bool)"/>
    public static void InstantiateSingleton<T>(this T self, ref T singleton,
        bool duplicatesOK = true)
        where T : ScriptableObject
    {
        if (singleton)
        {
            if (self == singleton && duplicatesOK)
                return;

            throw new ArgumentException($"Multiple instances of {typeof(T)}.");
        }
        else
        {
            singleton = self;
        }
    }

    /// <summary>
    /// Instantiates a singleton (aka an instance). Also checks if singleton is
    /// already set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self">The monobehavior to instantiate the singleton
    /// on.</param>
    /// <param name="singleton">The static singleton to set.</param>
    /// <param name="dontDestroyOnLoad">If true, then call DontDestroyOnLoad on
    /// the gameobject. Also orphans the gameobject.</param>
    /// <param name="duplicatesOK">If true, don't display error if singleton
    /// is already set to <paramref name="self"/>.</param>
    public static void InstantiateSingleton<T>(this T self, ref T singleton,
        bool dontDestroyOnLoad = true, bool duplicatesOK = true)
        where T : MonoBehaviour
    {
        if (singleton)
        {
            if (self == singleton && duplicatesOK)
                return;

            GameObject.Destroy(self.gameObject);
            Debug.LogError($"Multiple instances of {typeof(T)}.");
        }
        else
        {
            singleton = self;

            if (dontDestroyOnLoad)
            {
                self.transform.Orphan();
                GameObject.DontDestroyOnLoad(self.gameObject);
            }
        }
    }
    #endregion

    #region Destroy
    /// <summary>
    /// Destroys the unity object, whether or not the application is playing or
    /// not. Fails for prefabs.
    /// </summary>
    /// <param name="unityObject">Object to destroy.</param>
    /// <returns>True if object is successfully destroyed. False
    /// otherwise.</returns>
    public static bool AutoDestroy(this GameObject unityObject)
    {
        if (Application.isPlaying)
        {
            GameObject.Destroy(unityObject);
            return true;
        }
        else if (!unityObject.IsPrefab())
        {
            GameObject.DestroyImmediate(unityObject);
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="AutoDestroy(GameObject)"/>
    public static bool AutoDestroy(this Component unityObject) =>
        unityObject.gameObject.AutoDestroy();

    /// <summary>
    /// Destroys <paramref name="unityObject"/> if <paramref name="condition"/>
    /// evaluates to false.
    /// </summary>
    /// <param name="unityObject">The object to conditionally destroy.</param>
    /// <param name="condition">Whether or not to destroy the object.</param>
    /// <param name="t">Time before destruction of the object.</param>
    public static bool DestroyIf(this UnityEngine.Object unityObject,
        bool condition, float t = 0)
    {
        if (condition) UnityEngine.Object.Destroy(unityObject, t);
        return condition;
    }

    /// <inheritdoc cref="DestroyIf"/>.
    public static bool DestroyIf(this UnityEngine.Object unityObject,
        Func<bool> condition, float t = 0) =>
        unityObject.DestroyIf(condition(), t);
    #endregion
}
