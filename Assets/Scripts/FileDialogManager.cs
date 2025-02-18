using UnityEngine;
using System.Collections;
using System.IO;

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
    #region Constants
    private const string ERROR_PLATFORM_MISMATCH =
        "Cannot open file dialog. Platform mismatch. " +
        "Supported platforms are WebGL and Editor.";
    #endregion

    #region Enums
    public enum FileMode
    {
        Bytes,
        Text,
    }
    #endregion

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
        this.InstantiateSingleton(ref instance);
        gameObject.name = "FileDialogManager";

#if UNITY_WEBGL && !UNITY_EDITOR
            // Call the external method.
            AddClickListenerForFileDialog();
#endif
    }

    #region Extern
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
        private static extern void WriteTextToDisk_JS(string uri, string content);

        [DllImport("__Internal")]
        private static extern void WriteBytesToDisk_JS(string uri, byte[] content);
    
        [DllImport("__Internal")]
        private static extern void DownloadTextDialog_JS(string defaultFileName, string content);

        [DllImport("__Internal")]
        private static extern void DownloadBytesDialog_JS(string defaultFileName, byte[] content);
#endif
    #endregion

    #region Delegates
    /// <summary>
    /// Called when the file path is retrieved from the open file dialog.
    /// </summary>
    /// <param name="valid">While any file was selected. If false, <paramref
    /// name="uri"/> is an empty string.</param>
    /// <param name="uri">The URI. If <paramref name="valid"/> is false, then
    /// this is still filled out.</param>
    public delegate void PathRetrieved(bool valid, string uri);

    /// <summary>
    /// Called when the file contents are retrieved.
    /// </summary>
    /// <param name="valid">Whether the file could be read or not.</param>
    /// <param name="text">The text contents of the file. If <paramref
    /// name="valid"/> is false, this value is an empty string.</param>
    public delegate void FileTextRetrieved(bool valid, string text);

    /// <inheritdoc cref="FileTextRetrieved"/>
    /// <param name="bytes">The bytes contents of the file.</param>
    public delegate void FileBytesRetrieved(bool valid, byte[] bytes);

    /// <summary>
    /// Called when file contents have been written.
    /// </summary>
    /// <param name="success">Whether the write was successful.</param>
    public delegate void FileContentsWritten(bool success);
    #endregion

    #region Variables
    private PathRetrieved cachedPathRetrieved;

    private FileTextRetrieved cachedTextRetrieved;
    private FileBytesRetrieved cachedBytesRetrieved;
    #endregion

    #region File Dialog
    /// <summary>
    /// Opens a file dialog.
    /// </summary>
    /// <param name="callback">Callback on retrieval.</param>
    /// <param name="extensions">Comma-separated list of acceptable file
    /// extensions.</param>
    public void OpenFileDialog(PathRetrieved callback, string extensions)
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel(
            "File Dialog", "", ""
        );
        callback(File.Exists(path), path);
#elif UNITY_WEBGL
        cachedPathRetrieved = callback;
        FocusFileUploader(extensions);
#else
        throw new InvalidOperationException(ERROR_PLATFORM_MISMATCH);
#endif
    }

    /// <summary>
    /// Coroutine version of <see cref="OpenFileDialog"/>.
    /// </summary>
    /// <inheritdoc cref="OpenFileDialog"/>
    public IEnumerator OpenFileDialog_CR(PathRetrieved callback,
        string extensions)
    {
        bool retrieved = false;

        OpenFileDialog((valid, path) =>
        {
            retrieved = true;
            callback.Invoke(valid, path);
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

    #region Writing
    /// <summary>
    /// Writes <paramref name="contents"/> to the file identified by <paramref
    /// name="uri"/>.
    /// </summary>
    /// <param name="uri">Either a file path (for Editor) or a file blob (for
    /// WebGL).</param>
    /// <param name="contents">The contents to write to the file.</param>
    public void WriteText(string uri, string contents)
    {
#if UNITY_EDITOR
        File.WriteAllText(uri, contents);
#elif UNITY_WEBGL
        WriteTextToDisk_JS(uri, contents);
#else
        throw new InvalidOperationException(ERROR_PLATFORM_MISMATCH);
#endif
    }

    /// <summary>
    /// Writes <paramref name="contents"/> to the file identified by <paramref
    /// name="uri"/>.
    /// <inheritdoc cref="WriteText"/>
    public void WriteBytes(string uri, byte[] contents)
    {
#if UNITY_EDITOR
        File.WriteAllBytes(uri, contents);
#elif UNITY_WEBGL
        WriteBytesToDisk_JS(uri, contents);
#else
        throw new InvalidOperationException(ERROR_PLATFORM_MISMATCH);
#endif
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
        WriteText(path, contents);
#elif UNITY_WEBGL
        DownloadTextDialog_JS(defaultFileName, contents);
#else
        throw new InvalidOperationException(ERROR_PLATFORM_MISMATCH);
#endif
    }

    public void SaveAsFileDialog(string defaultFileName, byte[] contents)
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.SaveFilePanel(
            "File Dialog", "", defaultFileName, ".json"
        );
        WriteBytes(path, contents);
#elif UNITY_WEBGL
        DownloadBytesDialog_JS(defaultFileName, contents);
#else
        throw new InvalidOperationException(ERROR_PLATFORM_MISMATCH);
#endif
    }
    #endregion
}