using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using System;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

/// <summary>
/// manager to enable file dialogs in the WebGL build.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public class FileDialogManager : MonoBehaviour
{
    #region Instance
    /// <summary>
    /// The static reference to a(n) 
    /// <see cref="FileDialogManager"/> instance.
    /// </summary>
    private static FileDialogManager instance;

    /// <inheritdoc cref="instance"/>
    public static FileDialogManager Instance => instance;
    #endregion

    private void Awake()
    {
        this.InstantiateComponent();

#if UNITY_WEBGL && !UNITY_EDITOR
            // Call the external method.
            AddClickListenerForFileDialog();
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    /// <summary>
    /// This is the external method that allows for a file dialog to be opened
    /// in WebGL. See the comments on JSFileDialog.jslib for more information.
    /// </summary>
    [DllImport("__Internal")]
    private static extern void AddClickListenerForFileDialog();

    [DllImport("__Internal")]
    private static extern void FocusFileUploader(string accept);

    [DllImport("__Internal")]
    private static extern void WriteToDisk_JS(string uri, string content);

    [DllImport("__Internal")]
    private static extern void DownloadFileDialog_JS(string defaultFileName, string content);
#endif


    #region Delegates
    /// <summary>
    /// Called when the file path is retrieved from the open file dialog.
    /// </summary>
    /// <param name="valid">If false, <paramref name="uri"/> is an empty
    /// string.</param>
    /// <param name="uri">The URI. If <paramref name="valid"/> is false, then
    /// this is still filled out.</param>
    public delegate void OnPathRetrieved(bool valid, string uri);

    /// <summary>
    /// Called when the file contents are retrieved.
    /// </summary>
    /// <param name="valid">Whether the file could be read or not.</param>
    /// <param name="contents">The contents of the file. If <paramref
    /// name="valid"/> is false, this value is an empty string.</param>
    public delegate void FileContentsRetrieved(bool valid, string contents);

    public delegate void FileContentsWritten(bool success);
    #endregion

    #region Variables
    private OnPathRetrieved cachedPathRetrieved;

    private FileContentsRetrieved cachedContentsRetrieved;

    private FileContentsWritten cachedContentsWritten;
    #endregion

    #region Checks
    public void FileExists(string uri, Action<bool> callOnChecked)
    {
        StartCoroutine(FileExists_CR(uri, callOnChecked));
    }

    public IEnumerator FileExists_CR(string uri, Action<bool> callOnChecked)
    {
        if (Application.isEditor)
        {
            callOnChecked(File.Exists(uri));
            yield return null;
        }
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            yield return webRequest.SendWebRequest();

            callOnChecked(webRequest.result == UnityWebRequest.Result.Success);
        }
        else
        {
            throw new InvalidOperationException("Cannot check file. " +
            "Platform mismatch. Supported platforms are WebGL and Editor.");
        }
    }
    #endregion

    #region File Dialog
    /// <summary>
    /// Opens a file dialog.
    /// </summary>
    /// <param name="callOnRetrieved">Callback on retrieval.</param>
    /// <param name="extensions">Comma-separated list of acceptable file
    /// extensions.</param>
    public void OpenFileDialog(OnPathRetrieved callOnRetrieved,
        string extensions)
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel(
                "File Dialog", "", extensions
            );
        callOnRetrieved(File.Exists(path), path);
#elif UNITY_WEBGL
        cachedPathRetrieved = callOnRetrieved;
        FocusFileUploader(extensions);
#else
        throw new System.InvalidOperationException("Cannot open file dialog. " +
        "Platform mismatch. Supported platforms are WebGL and Editor.");
#endif
    }

    /// <summary>
    /// Coroutine version of <see cref="OpenFileDialog(OnPathRetrieved)"/>.
    /// </summary>
    /// <inheritdoc cref="OpenFileDialog"/>
    public IEnumerator OpenFileDialog_CR(OnPathRetrieved callOnRetrieved,
        string extensions)
    {
        bool retrieved = false;

        OpenFileDialog((valid, path) =>
        {
            retrieved = true;
            callOnRetrieved.Invoke(valid, path);
        }, extensions);

        yield return new WaitUntil(() => retrieved);
    }

    /// <summary>
    /// File dialog result, called from the JS library.
    /// </summary>
    /// <param name="fileUrl">The url of the file. This will be a file blob
    /// instead of a traditional path.</param>
    public void FileDialogResult(string fileUrl)
    {
        cachedPathRetrieved(!string.IsNullOrWhiteSpace(fileUrl), fileUrl);
    }
    #endregion

    #region Reading
    /// <summary>
    /// Read contents from a file identified by <see cref="uri"/>.
    /// </summary>
    /// <param name="uri">Either a file path (for Editor) or a file blob (for
    /// WebGL).</param>
    /// <param name="callOnRetrieved">The callback to invoke when the call is
    /// done.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ReadFromFile(string uri, FileContentsRetrieved callOnRetrieved)
    {
#if UNITY_EDITOR
        try
        {
            string contents = File.ReadAllText(uri);
            callOnRetrieved(true, contents);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            callOnRetrieved(false, null);
        }
#elif UNITY_WEBGL
        cachedContentsRetrieved = callOnRetrieved;
        StartCoroutine(LoadBlob(uri));
#else
        throw new System.InvalidOperationException("Cannot read file. " +
            "Platform mismatch. Supported platforms are WebGL and Editor.");
#endif
    }

    /// <summary>
    /// Load the file contents of the blob.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private IEnumerator LoadBlob(string uri)
    {
        // Apparently, you can use web request to get stuff on the local
        // machine.
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            cachedContentsRetrieved(true, webRequest.downloadHandler.text);
        }
        else
        {
            cachedContentsRetrieved(false, null);
        }
    }
    #endregion

    #region Writing
    /// <summary>
    /// Writes <paramref name="contents"/> to the file identified by <paramref
    /// name="uri"/>.
    /// </summary>
    /// <param name="uri">Either a file path (for Editor) or a file blob (for
    /// WebGL).</param>
    /// <param name="contents">The contents to write to the file.</param>
    public void WriteToFile(string uri, string contents)
    {
#if UNITY_EDITOR
        File.WriteAllText(uri, contents);
#elif UNITY_WEBGL
        WriteToDisk_JS(uri, contents);
#else
        throw new System.InvalidOperationException("Cannot write to file. " +
            "Platform mismatch. Supported platforms are WebGL and Editor.");
#endif
    }

    private IEnumerator WriteToFile_CR(string uri, string contents)
    {
        UnityWebRequest webRequest = UnityWebRequest.Put(uri, contents);
        yield return webRequest.SendWebRequest();
    }
    #endregion

    #region Save As
    /// <summary>
    /// Opens a save as file dialog, where users can create new files.
    /// </summary>
    /// <param name="defaultFileName">Default name of the file, including the
    /// extension.</param>
    /// <param name="contents">Contents of the file.</param>
    public void SaveAsFileDialog(string defaultFileName, string contents)
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.SaveFilePanel(
            "File Dialog", "", defaultFileName, ".json"
        );
        WriteToFile(path, contents);
#elif UNITY_WEBGL
        DownloadFileDialog_JS(defaultFileName, contents);
#else
        throw new System.InvalidOperationException("Cannot open file dialog. " +
            "Platform mismatch. Supported platforms are WebGL and Editor.");
#endif
    }
    #endregion
}