using UnityEngine;

/// <summary>
/// Contains methods that pertain to Unity layers.
/// </summary>
public static class LayerExt
{
    #region Layer Modification

    /// <summary>
    /// Moves all children of <paramref name="root"/> into <paramref
    /// name="layer"/>.
    /// </summary>
    /// <param name="root">The root transform.</param>
    /// <param name="layer">The new layer.</param>
    public static void MoveAllToLayer(this Transform root, int layer)
    {
        root.gameObject.layer = layer;

        foreach (Transform child in root.transform)
        {
            MoveAllToLayer(child, layer);
        }
    }
    #endregion

    #region Masks
    /// <summary>
    /// Determines if the layer mask is empty, logging a warning if it is.
    /// </summary>
    /// <param name="mask">The layer mask.</param>
    /// <param name="context">The optional context (gameobject the mask is
    /// attached to).</param>
    /// <returns>True if the mask is empty, false otherwise.</returns>
    public static bool WarnIfMaskEmpty(this LayerMask mask, GameObject context = null)
    {
        return WarnIfMaskEmpty((int)mask, context);
    }

    /// <inheritdoc cref="WarnIfMaskEmpty(LayerMask, GameObject)"/>
    public static bool WarnIfMaskEmpty(this int mask, GameObject context = null)
    {
        if (mask == 0)
        {
            Debug.LogWarning("Mask is not set to any value.", context);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns true if layer is in mask.
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool ContainsLayer(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    /// <summary>
    /// Returns true if the layer is in mask.
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool ContainsLayer(this int mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    /// <summary>
    /// Returns a layer mask from <paramref name="layers"/>.
    /// </summary>
    /// <param name="layers">The layers to exclude from the mask.</param>
    /// <returns></returns>
    public static LayerMask CreateMask(params int[] layers)
    {
        int mask = 0;

        foreach (var layer in layers)
        {
            mask |= 1 << layer;
        }

        return mask;
    }
    #endregion
}