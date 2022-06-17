using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The Tile class is used to represent a tile on the board, on which a unit can be placed and move to/from
/// </summary>
public class Tile : MonoBehaviour, ISelectableEntity
{
    [SerializeField] private int _x;                                             // X coordinate of the tile
    [SerializeField] private int _y;                                             // Y coordinate of the tile
    private Unit _unit = null;                                                   // Unit on the tile, if any

    // Pathfinding variables
    [System.NonSerialized] public Tile cameFromTile = null;
    [System.NonSerialized] public int tileCost = 9999;

    // Color variables
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _searchingColor;
    [SerializeField] private Color _darkColor;

    // Public get/set
    public int x { get => _x; set => _x = value; }
    public int y { get => _y; set => _y = value; }
    public Unit unit { get => _unit; set => _unit = value; }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init class variables
    /// </summary>
    public void Init()
    {
        _unit = null;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Reset the unit reference
    /// </summary>
    public void ResetUnit()
    {
        _unit = null;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Whether or not an unit can move on this tile
    /// </summary>
    /// <returns> True if an unit can move on the tile, false otherwise </param>
    public bool FreeForMovement()
    {
        return _unit == null;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when the tile has been clicked on
    /// </summary>
    public void OnMouseDown()
    {
        Select();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the tile
    /// </summary>
    public void Select()
    {
        Board.instance.SelectTile(this);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Unselect the tile
    /// </summary>
    public void Unselect()
    {
        // TODO: Unselect behaviour
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Change tile color to the default color
    /// </summary>
    public void FeedbackDefault()
    {
        gameObject.GetComponent<SpriteRenderer>().color = _defaultColor;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Change tile color to the searching color
    /// </summary>
    public void FeedbackSearching()
    {
        gameObject.GetComponent<SpriteRenderer>().color = _searchingColor;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Change tile color to the searched color
    /// </summary>
    public void FeedbackDark()
    {
        gameObject.GetComponent<SpriteRenderer>().color = _darkColor;
    }
}
