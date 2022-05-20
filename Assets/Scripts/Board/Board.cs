using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The Board class is used to represent the game board on which the unit are placed in the Planning Phase to fight during the Battle Phase
/// </summary>
public class Board : MonoBehaviour
{
    public static Board instance;                                                       // Board static reference
    public GameObject tilesGO;                                                          // Contains all tiles references in Unity Editor
    public List<Unit> _playerUnits = new List<Unit>();                                  // Player units on the board
    private List<Unit> _enemyUnits = new List<Unit>();                                  // Enemy units on the board
    public int xSize;
    public int ySize;
    public Dictionary<(int, int), Tile> _tiles = new Dictionary<(int, int), Tile>();    // Contains every tile on the board, with their (x,y) coordinate as key

    // Public get/set
    public List<Unit> playerUnits { get => _playerUnits; set => _playerUnits = value; }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init instance
    /// </summary>
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }

        instance = this;
        Init();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init class variables
    /// </summary>
    public void Init()
    {
        int x = 0;
        int y = 0;
        foreach(Transform childLine in tilesGO.transform)
        {
            foreach(Transform childTile in childLine)
            {
                Tile tile = childTile.gameObject.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;
                _tiles.Add((x,y), tile);
                x++;
            }
            x = 0;
            y++;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns a Tile given its (x,y) position
    /// </summary>
    /// <param name="x"> X coordinate of the tile to return </param>
    /// <param name="y"> Y coordinate of the tile to return </param>
    /// <returns> Tile located at (x,y) </returns>
    public Tile GetTile(int x, int y)
    {
        // If x or y is not in the array, returns null
        if (x >= xSize || x < 0)
        {
            //Debug.Log("Board.GetUnit(" + x + ", " + y + "): " + x + " is not contained in the array _tiles[" + _tiles.GetLength(0) + "," + _tiles.GetLength(1) + "]");
            return null;
        }
        if (y >= ySize || y < 0)
        {
            //Debug.Log("Board.GetUnit(" + x + ", " + y + "): " + y + " is not contained in the array _tiles[" + _tiles.GetLength(0) + "," + _tiles.GetLength(1) + "]");
            return null;
        }
        return _tiles[(x,y)];
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns the Unit on a Tile given its position
    ///     If the Tile does not contain any Unit, returns null
    /// </summary>
    /// <param name="x"> X coordinate of the unit to return, if any </param>
    /// <param name="y"> Y coordinate of the unit to return, if any </param>
    /// <returns> Unit located at (x,y), if any </returns>
    public Unit GetUnit(int x, int y)
    {
        return GetTile(x, y).unit;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns the Unit on a given Tile
    ///     If the Tile does not contain any Unit, returns null
    /// </summary>
    /// <param name="tile"> Tile of the unit to return, if any </param>
    /// <returns> Unit located on tile, if any </returns>
    public Unit GetUnit(Tile tile)
    {
        return tile.unit;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Move a given Unit to a given Tile
    /// </summary>
    /// <param name="tile"> Tile of the unit to return, if any </param>
    /// <returns> Unit located on tile, if any </returns>
    public void MoveUnit(Unit unit, Tile tile)
    {
        if (unit.tile != tile)
        {
            // Reset previous tile's unit reference
            unit.tile.ResetUnit();

            // Move unit to tile
            unit.Move(tile);

            // Set new tile's unit reference
            tile.unit = unit;
        }
    }
}
