#if USING_ADDRESSABLES
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    
    /// <summary>
    /// Extensions for <see cref="Addressables"/>
    /// </summary>
    /// 
    /// <remarks>
    /// Authors: Ryan Chang (2024)
    /// </remarks>
    public static class AddressableExt
    {
        private static void DefaultFailure(string handleName, string keyName,
            Exception exception)
        {
            throw new InvalidOperationException(
                $"Handle {handleName} failed to resolve key: {keyName}",
                exception
            );
        }
    
        #region Load One
        /// <summary>
        /// Loads one addressable asset. The loaded asset is handled by <paramref
        /// name="onSuccess"/>.
        /// </summary>
        /// <param name="key">Key to load the addressable asset.</param>
        /// <param name="onSuccess">Action to perform when the asset is loaded. Use
        /// this to handle the loaded asset.</param>
        /// <param name="onFailure">Action to perform when the asset  or resource
        /// location fails to load. This action uses the debug name of the handle,
        /// then the exception thrown by the loading attempt, as parameters.</param>
        /// <typeparam name="T">Any.</typeparam>
        public static void LoadOne<T>(object key,
            Action<T> onSuccess,
            Action<string, Exception> onFailure)
        {
            LoadResource(key, onSuccess, onFailure);
        }
    
        /// <inheritdoc cref="LoadOne{T}(object, Action{T}, Action{string,
        /// Exception})"/>
        public static void LoadOne<T>(object key, Action<T> onSuccess)
        {
            LoadOne(
                key,
                onSuccess,
                (name, exception) => DefaultFailure(
                    name,
                    key.ToString(),
                    exception
                )
            );
        }
        #endregion
    
        #region Load Many
        /// <summary>
        /// Loads many addressable assets. The loaded asset is handled by <paramref
        /// name="onSuccess"/>.
        /// </summary>
        /// <param name="keys">Keys to use to load assets.</param>
        /// <param name="onOneLoaded">Action performed as soon as one of the assets
        /// are loaded. Uses the loaded asset as the parameter.</param>
        /// <param name="onAllLoaded">Action performed when all assets have been
        /// loaded. Uses the loaded assets as the parameter.</param>
        /// <inheritdoc cref="LoadOne{T}(object, Action{T}, Action{string,
        /// Exception})"/>
        public static void LoadMany<T>(IList<object> keys,
            Action<T> onOneLoaded,
            Action<IList<T>> onAllLoaded,
            Action<string, Exception> onFailure)
        {
            LoadResources(
                keys,
                onOneLoaded,
                onAllLoaded,
                onFailure
            );
        }
    
        /// <summary>
        /// Loads many addressable assets using only one <paramref name="key"/>. The
        /// loaded asset is handled by <paramref name="onSuccess"/>.
        /// </summary>
        /// <param name="key">Key used to load the assets.</param>
        /// <inheritdoc cref="LoadMany{T}(IList{object}, Action{T},
        /// Action{IList{T}}, Action{string, Exception})"/>
        public static void LoadMany<T>(object key,
            Action<T> onOneLoaded,
            Action<IList<T>> onAllLoaded,
            Action<string, Exception> onFailure)
        {
            LoadMany(
                new List<object>() { key },
                onOneLoaded,
                onAllLoaded,
                onFailure
            );
        }
    
        public static void LoadMany<T>(object key,
            Action<T> onOneLoaded,
            Action<IList<T>> onAllLoaded)
        {
            LoadMany(
                key,
                onOneLoaded,
                onAllLoaded,
                (name, exception) => DefaultFailure(
                    name,
                    key.ToString(),
                    exception
                )
            );
        }
        #endregion
    
        #region Helpers
        private static void LoadResources<T>(IList<object> keys,
            Action<T> onOneLoaded,
            Action<IList<T>> onAllLoaded,
            Action<string, Exception> onFailure)
        {
            DebugExt.Log(
                DebugExt.DebugGroup.Addressable,
                $"Loading many resources (see below)\n{keys.PrintElements()}"
            );
            var handle = Addressables.LoadAssetsAsync(
                (IEnumerable)keys,
                onOneLoaded,
                Addressables.MergeMode.Union
            );
            handle.Completed += (h) =>
            {
                switch (h.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        onAllLoaded?.Invoke(h.Result);
                        break;
                    default:
                        onFailure?.Invoke(h.DebugName, h.OperationException);
                        break;
                }
    
                Addressables.Release(h);
            };
        }
    
        private static void LoadResource<T>(object key,
            Action<T> onSuccess,
            Action<string, Exception> onFailure)
        {
            DebugExt.Log(
                DebugExt.DebugGroup.Addressable,
                $"Loading resource {key}"
            );
            var handle = Addressables.LoadAssetAsync<T>(key);
            handle.Completed += (h) =>
            {
                switch (h.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        onSuccess?.Invoke(h.Result);
                        break;
                    default:
                        onFailure?.Invoke(h.DebugName, h.OperationException);
                        break;
                }
    
                Addressables.Release(h);
            };
        }
        #endregion
    }
    
#endif