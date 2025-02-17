using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Extensions for loading stuff off of the internet.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public static class WebClientExt
{
    #region Variables
    public delegate void HandleDocumentResponse(string serverResponse);

    public delegate void HandleTextureResponse(Texture2D serverResponse);

    public delegate void HandleErrorResponse(string errorMessage);

    public static HandleErrorResponse DefaultErrorResponse
    {
        get
        {
            return errorMessage => throw new InvalidOperationException(
                "Web Request Error! " + errorMessage
            );
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Retrieves a document from the web. On error, displays an notification
    /// dialog.
    /// </summary>
    /// <param name="uri">The uri of the document.</param>
    /// <param name="callOnDone">The delegate that is called when the operation
    /// is finished.</param>
    public static void GetDocument(string uri, HandleDocumentResponse callOnDone)
    {
        GetDocument(uri, callOnDone, DefaultErrorResponse);
    }

    /// <summary>
    /// Retrieves a document from the web with customizable error handling.
    /// </summary>
    /// <param name="callOnError">The delegate that is called when the operation
    /// returns an error.</param>
    /// <inheritdoc cref="GetDocument(string, HandleDocumentResponse)"/>
    public static void GetDocument(string uri,
        HandleDocumentResponse callOnDone, HandleErrorResponse callOnError)
    {
        GameObjectHandle.Instance.StartCoroutine(
            WaitForDocument_CR(uri, callOnDone, callOnError)
        );
    }

    /// <summary>
    /// Retrieves a texture from the web. On error, displays an notification
    /// dialog.
    /// </summary>
    /// <inheritdoc cref="GetDocument(string, HandleDocumentResponse)"/>
    public static void GetTexture(string uri, HandleTextureResponse callOnDone)
    {
        GetTexture(uri, callOnDone, DefaultErrorResponse);
    }

    /// <summary>
    /// Retrieves a texture from the web with customizable error handling.
    /// </summary>
    /// <inheritdoc cref="GetDocument(string, HandleDocumentResponse,
    /// HandleErrorResponse)"/>
    public static void GetTexture(string uri,
        HandleTextureResponse callOnDone, HandleErrorResponse callOnError)
    {
        GameObjectHandle.Instance.StartCoroutine(
            WaitForTexture_CR(uri, callOnDone, callOnError)
        );
    }
    #endregion

    #region Private Methods
    private static IEnumerator WaitForDocument_CR(string uri,
        HandleDocumentResponse callOnDone, HandleErrorResponse callOnError)
    {
        using UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callOnDone(request.downloadHandler.text);
        }
        else
        {
            callOnError(request.error);
        }
    }

    private static IEnumerator WaitForTexture_CR(string uri,
        HandleTextureResponse callOnDone, HandleErrorResponse callOnError)
    {
        // Usage of UnityWebRequestTexture and DownloadHandlerTexture adapted
        // from
        // https://stackoverflow.com/questions/31765518/how-to-load-an-image-from-url-with-unity.
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            if (texture)
            {
                callOnDone(
                    texture
                );
            }
            else
            {
                callOnError("Downloaded file is not an image.");
            }
        }
        else
        {
            callOnError(request.error);
        }
    }
    #endregion
}