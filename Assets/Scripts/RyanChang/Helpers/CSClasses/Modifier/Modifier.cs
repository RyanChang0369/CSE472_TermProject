using System;
using UnityEngine;

[Serializable]
public class Modifier
{
    [Tooltip("What is added to the base value. " +
    "Final modification calculated by addition + multiplier * input.")]
    public float addition = 0;

    [Tooltip("What the input is multiplied by. " + 
    "Final modification calculated by addition + multiplier * input.")]
    public float multiplier = 1;

    /// <summary>
    /// Creates a new Modifier.
    /// </summary>
    /// <param name="addition">What is added to the base value.
    /// Final modification calculated by addition + multiplier * input.</param>
    /// <param name="multiplier">What the input is multiplied by. 
    /// Final modification calculated by addition + multiplier * input.</param>
    public Modifier(float addition = 0, float multiplier = 1)
    {
        this.addition = addition;
        this.multiplier = multiplier;
    }

    /// <summary>
    /// Calculate the modification by addition + multiplier * input.
    /// </summary>
    /// <param name="input">Input value to modify.</param>
    /// <returns>The modification.</returns>
    public float Modify(float input)
    {
        return input * multiplier + addition;
    }

    public override int GetHashCode()
    {
        return Tuple.Create(addition, multiplier).GetHashCode();
    }
}

