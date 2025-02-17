using UnityEngine;

/// <summary>
/// Contains static <see cref="GUIStyle"/>s.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public static class GUIStyleExt
{
    #region Variables
    /// <inheritdoc cref="LeftAlignBox"/>
    private static GUIStyle leftAlignBox = null;

    /// <inheritdoc cref="BoldLabel"/>
    private static GUIStyle boldLabel = null;
    #endregion

    #region Properties
    /// <summary>
    /// A GUI style of a box where all the left is left aligned instead of
    /// center aligned.
    /// </summary>
    public static GUIStyle LeftAlignBox
    {
        get
        {
            if (leftAlignBox == null)
            {
                leftAlignBox = GUI.skin.box;
                leftAlignBox.alignment = TextAnchor.MiddleLeft;
            }

            return leftAlignBox;
        }
    }

    /// <summary>
    /// A GUI style of a label that's bold.
    /// </summary>
    public static GUIStyle BoldLabel
    {
        get
        {
            if (boldLabel == null)
            {
                boldLabel = GUI.skin.label;
                boldLabel.fontStyle = FontStyle.Bold;
            }

            return boldLabel;
        }
    }
    #endregion
}