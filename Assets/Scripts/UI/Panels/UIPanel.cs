using UnityEngine;

/// <summary>
/// Defines a panel that can be activated and deactivated.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2025)
/// </remarks>
public abstract class UIPanel : MonoBehaviour
{
    private bool active;

    public bool Active 
    {
        get => active;
        set
        {
            active = value;

            if (value) Activate();
            else Deactivate();
        }
    }

    public abstract void Activate();

    public abstract void Deactivate();
}