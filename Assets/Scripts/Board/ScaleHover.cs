using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Increase the scale of the tile/unit when hovered by the mouse, and restablish it to default when not
/// </summary>
public class ScaleHover : MonoBehaviour
{
    [SerializeField] private float _hoverAmount = 1.1f;

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Increase the scale when hovered
    /// </summary>
    public void OnMouseEnter()
    {
        transform.localScale *= _hoverAmount;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the scale when hovering stopped
    /// </summary>
    public void OnMouseExit()
    {
        transform.localScale /= _hoverAmount;
    }
}
