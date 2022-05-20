using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The Tile class is used to represent a tile on the board, on which a unit can be placed and move to/from
/// </summary>
public class Tile : MonoBehaviour
{
    private int _x;                                             // X coordinate of the tile
    private int _y;                                             // Y coordinate of the tile
    private Unit _unit;                                         // Unit on the tile, if any

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
}
