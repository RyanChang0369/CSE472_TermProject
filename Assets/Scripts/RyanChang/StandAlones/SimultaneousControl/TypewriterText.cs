using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

/// <summary>
/// Allows for fancy typewritten text, when paired with <see
/// cref="TextMeshProUGUI"/>. Clears all text in the <see
/// cref="TextMeshProUGUI"/>, then replaces each character in the text one after
/// the other.
///
/// <br/>
///
/// Authors: Ryan Chang (2023)
/// </summary>
/// <seealso cref="TypewriterController"/>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TypewriterText : SimultaneousControl
{
    #region Variables
    [Tooltip("If true, override the character delay set by " +
        "TypewriterController.")]
    [SerializeField]
    private bool overrideCharacterDelay = false;

    [Tooltip("The overridden character delay.")]
    [ShowIf(nameof(overrideCharacterDelay))]
    [SerializeField]
    private RNGPattern characterDelayOverride = new(0.05f);

    [Tooltip("The TextMeshPro text that will be used. Autoadded.")]
    [ReadOnly]
    public TextMeshProUGUI text;

    [Tooltip("What was the original text contained by text?")]
    [ReadOnly]
    public string originalText;
    #endregion

    #region Method
    public override void Instantiate()
    {
        this.RequireComponent(out text);
        originalText = text.text;
        text.text = "";
    }

    public override void ResetControl()
    {
        text.text = "";
    }

    public override void DisableControl()
    {
        text.text = originalText;
    }

    public override IEnumerator DoAction()
    {
        var delay = overrideCharacterDelay ? characterDelayOverride :
            ((TypewriterController)Controller).characterDelay;

        foreach (var c in originalText)
        {
            text.text += c;

            yield return new WaitForSecondsRealtime(delay.Evaluate());
        }
    }
    #endregion
}