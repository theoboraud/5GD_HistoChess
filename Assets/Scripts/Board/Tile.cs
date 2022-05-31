using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The Tile class is used to represent a tile on the board, on which a unit can be placed and move to/from
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private int _x;                                             // X coordinate of the tile
    [SerializeField] private int _y;                                             // Y coordinate of the tile
    private Unit _unit = null;                                                   // Unit on the tile, if any

    // Pathfinding variables
    [System.NonSerialized] public Tile cameFromTile = null;
    [System.NonSerialized] public int tileCost = 9999;

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
    ///     Select the tile
    /// </summary>
    public void OnMouseDown()
    {
        Board.instance.SelectTile(this);
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
}
