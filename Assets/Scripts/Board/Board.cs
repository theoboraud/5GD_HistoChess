using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     The Board class is used to represent the game board on which the unit are placed in the Planning Phase to fight during the Battle Phase
/// </summary>
public class Board : MonoBehaviour
{
    public static Board instance;                                                       // Board static reference
    public GameObject tilesGO;                                                          // Contains all tiles references in Unity Editor
    [SerializeField]private List<Unit> _playerUnits = new List<Unit>();                 // Player units on the board
    [SerializeField]private List<Unit> _enemyUnits = new List<Unit>();                  // Enemy units on the board
    [SerializeField] private int _xSize = 6;
    [SerializeField] private int _ySize = 6;
    public Dictionary<(int, int), Tile> _tiles = new Dictionary<(int, int), Tile>();    // Contains every tile on the board, with their (x,y) coordinate as key
    private Unit _selectedUnit;

    // Public get/set
    public List<Unit> playerUnits { get => _playerUnits; set => _playerUnits = value; }
    public List<Unit> enemyUnits { get => _enemyUnits; set => _enemyUnits = value; }

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

        foreach (Transform childTile in tilesGO.transform)
        {

            Tile tile = childTile.gameObject.GetComponent<Tile>();
            tile.x = x;
            tile.y = y;
            _tiles.Add((x,y), tile);
            x++;

            if (x >= _xSize)
            {
                x = 0;
                y++;
            }
        }

        foreach (Unit unit in _playerUnits)
        {
            unit.Init();
        }

        foreach (Unit unit in _enemyUnits)
        {
            unit.Init();
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
        if (x >= _xSize || x < 0)
        {
            //Debug.Log("Board.GetUnit(" + x + ", " + y + "): " + x + " is not contained in the array _tiles[" + _tiles.GetLength(0) + "," + _tiles.GetLength(1) + "]");
            return null;
        }
        if (y >= _ySize || y < 0)
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
    /// <param name="unit"> Unit to move </param>
    /// <param name="tile"> Tile to which the unit will move </param>
    public void MoveUnit(Unit unit, Tile tile)
    {
        if (unit.tile != tile)
        {
            // Reset previous tile's unit reference
            if (unit.tile != null)
            {
                unit.tile.ResetUnit();
            }

            // Move unit to tile
            unit.Move(tile);

            // Set new tile's unit reference
            tile.unit = unit;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns all tiles in strict range of a given unit
    /// </summary>
    /// <param name="unit"> Unit searching for every tile in its range </param>
    /// <param name="range"> Range value the unit is searching tiles in </param>
    /// <returns> List of tiles in range of the given unit </returns>
    public List<Tile> GetTilesInRange(Unit unit, int range)
    {
        List<Tile> tilesInRange = new List<Tile>();

        if (range == 1)
        {
            tilesInRange.Add(GetTile(unit.tile.x + 1, unit.tile.y));
            tilesInRange.Add(GetTile(unit.tile.x - 1, unit.tile.y));
            tilesInRange.Add(GetTile(unit.tile.x, unit.tile.y + 1));
            tilesInRange.Add(GetTile(unit.tile.x, unit.tile.y - 1));
        }

        if (range == 2)
        {
            tilesInRange.Add(GetTile(unit.tile.x + 2, unit.tile.y));
            tilesInRange.Add(GetTile(unit.tile.x - 2, unit.tile.y));
            tilesInRange.Add(GetTile(unit.tile.x, unit.tile.y + 2));
            tilesInRange.Add(GetTile(unit.tile.x, unit.tile.y - 2));

            tilesInRange.Add(GetTile(unit.tile.x + 1, unit.tile.y + 1));
            tilesInRange.Add(GetTile(unit.tile.x + 1, unit.tile.y - 1));
            tilesInRange.Add(GetTile(unit.tile.x - 1, unit.tile.y + 1));
            tilesInRange.Add(GetTile(unit.tile.x - 1, unit.tile.y - 1));
        }

        if (range == 3)
        {
            tilesInRange.Add(GetTile(unit.tile.x + 3, unit.tile.y));
            tilesInRange.Add(GetTile(unit.tile.x - 3, unit.tile.y));
            tilesInRange.Add(GetTile(unit.tile.x, unit.tile.y + 3));
            tilesInRange.Add(GetTile(unit.tile.x, unit.tile.y - 3));

            tilesInRange.Add(GetTile(unit.tile.x + 2, unit.tile.y + 1));
            tilesInRange.Add(GetTile(unit.tile.x + 2, unit.tile.y - 1));
            tilesInRange.Add(GetTile(unit.tile.x - 2, unit.tile.y + 1));
            tilesInRange.Add(GetTile(unit.tile.x - 2, unit.tile.y - 1));

            tilesInRange.Add(GetTile(unit.tile.x + 1, unit.tile.y + 2));
            tilesInRange.Add(GetTile(unit.tile.x + 1, unit.tile.y - 2));
            tilesInRange.Add(GetTile(unit.tile.x - 1, unit.tile.y + 2));
            tilesInRange.Add(GetTile(unit.tile.x - 1, unit.tile.y - 2));
        }

        for (int i = 0; i < tilesInRange.Count; i++)
        {
            if (tilesInRange[i] == null)
            {
                tilesInRange.Remove(tilesInRange[i]);
            }
        }

        return tilesInRange;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return the highest priority taret, depending on its initiative
    /// </summary>
    /// <param name="unit"> Unit searching for priority target </param>
    /// <param name="range"> Max range value the unit is searching tiles in </param>
    /// <returns> Unit located on tile, if any </returns>
    public Unit GetPriorityTarget(Unit unit, int range)
    {
        List<Unit> priorityTargets = new List<Unit>();
        int initiative = 0;

        // Search for all closest enemy units
        for (int i = 1; i <= range; i++)
        {
            List<Tile> tilesInRange = GetTilesInRange(unit, i);

            foreach (Tile tile in tilesInRange)
            {
                if (tile.unit != null)
                {
                    // If the tile contains an unit of the opposite faction
                    if (unit.faction != tile.unit.faction)
                    {
                        // If unit has initiative higher or equal to current highest initiative, add unit to the list
                        if (tile.unit.initiative >= initiative)
                        {
                            // If the unit initiative is higher than the current highest initiative, we clear the list and update the highest initiative
                            if (tile.unit.initiative > initiative)
                            {
                                initiative = tile.unit.initiative;
                                priorityTargets = new List<Unit>();
                            }
                            priorityTargets.Add(tile.unit);
                        }

                    }
                }
            }

            // If the initiative list contains at least one unit, the target search is stopped
            if (priorityTargets.Count > 0)
            {
                i = range;
            }
        }

        // Select a random target among registered priority targets, if any
        if (priorityTargets.Count > 0)
        {
            return priorityTargets[Random.Range(0, priorityTargets.Count)];
        }

        // If no priority targets selected, return null
        return null;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select a unit
    /// </summary>
    /// <param name="unit"> Unit to select </param>
    public void SelectUnit(Unit unit)
    {
        _selectedUnit = unit;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the tile
    ///     If a unit is selected, move selected unit to this tile
    /// </summary>
    /// <param name="tile"> Tile to move the selected unit to </param>
    public void SelectTile(Tile tile)
    {
        if (_selectedUnit != null)
        {
            MoveUnit(_selectedUnit, tile);
            _selectedUnit = null;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Kill the given unit
    /// </summary>
    /// <param name="unit"> Unit to kill </param>
    public void KillUnit(Unit unit)
    {
        unit.tile.ResetUnit();
        _playerUnits.Remove(unit);
        _enemyUnits.Remove(unit);
        Destroy(unit.gameObject);
    }
}
