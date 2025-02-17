using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ignores collisions between <see cref="target"/> and <see cref="others"/>.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public class ColliderIgnorer : MonoBehaviour
{
    #region Structs
    [System.Serializable]
    public struct ColliderOption
    {
        public Collider2D collider;

        [Tooltip("If true, then find and ignore children as well.")]
        public bool applyToChildren;
    } 
    #endregion

    #region Variables
    [Tooltip("The target collider to ignore with others.")]
    public ColliderOption target;

    [Tooltip("The colliders to ignore with target.")]
    public ColliderOption[] others;
    #endregion

    #region Methods
    private void Start()
    {
        DoIgnore(true);
    }

    public void DoIgnore(bool ignore)
    {
        if (target.applyToChildren)
        {
            foreach (var child in target.collider.GetComponentsInChildren<Collider2D>())
            {
                var ignorer = child.gameObject.AddComponent<ColliderIgnorer>();
                ignorer.target = new()
                {
                    collider = child,
                    applyToChildren = false
                };
                ignorer.others = others;
            }
        }

        foreach (var other in others)
        {
            if (other.applyToChildren)
            {
                DoIgnore(target.collider,
                    other.collider.GetComponentsInChildren<Collider2D>(),
                    ignore
                );
            }

            Physics2D.IgnoreCollision(target.collider, other.collider, ignore);
        }
    }

    public void DoIgnore(Collider2D target, IEnumerable<Collider2D> others, bool ignore)
    {
        foreach (var c2d in others)
        {
            Physics2D.IgnoreCollision(target, c2d, ignore);
        }
    }
    #endregion
}