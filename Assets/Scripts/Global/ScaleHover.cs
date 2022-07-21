using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Increase the scale of the tile/unit when hovered by the mouse, and restablish it to default when not
/// </summary>
public class ScaleHover : MonoBehaviour
{
    [SerializeField] private float _hoverAmount = 1.1f;
    private bool _hasScaledUp = false;

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Increase the scale when hovered
    /// </summary>
    public void OnMouseEnter()
    {
        ScaleUp();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the scale when hovering stopped
    /// </summary>
    public void OnMouseExit()
    {
        ScaleDown();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the scale when hovering stopped
    /// </summary>
    public void ScaleUp()
    {
        if (!_hasScaledUp)
        {
            _hasScaledUp = true;
            transform.localScale *= _hoverAmount;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the scale when hovering stopped
    /// </summary>
    public void ScaleDown()
    {
        if (_hasScaledUp)
        {
            _hasScaledUp = false;
            transform.localScale /= _hoverAmount;
        }
    }
}
