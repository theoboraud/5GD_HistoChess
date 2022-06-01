using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using Enums;

/// <summary>
///     The Board class is used to represent the game board on which the unit are placed in the Planning Phase to fight during the Battle Phase
/// </summary>
public class Board : MonoBehaviour
{
    // Variables
    private const int MAX_COMMAND_POINTS = 14;
    [SerializeField] private int _xSize = 6;
    [SerializeField] private int _ySize = 6;
    private int _playerCommandPoints;
    private int _enemyCommandPoints;

    // References
    public static Board instance;                                                       // Board static reference
    public GameObject tilesGO;                                                          // Contains all tiles references in Unity Editor
    [SerializeField] private List<Unit> _playerUnits = new List<Unit>();                // Player units on the board
    [SerializeField] private List<Unit> _enemyUnits = new List<Unit>();                 // Enemy units on the board
    public Dictionary<(int, int), Tile> _tiles = new Dictionary<(int, int), Tile>();    // Contains every tile on the board, with their (x,y) coordinate as key
    private Unit _selectedUnit;                                                         // Current selected unit, if any
    [SerializeField] private Transform _playerUnitsParent;
    [SerializeField] private Transform _enemyUnitsParent;
    [SerializeField] private TMP_Text _playerCommandPointsValue;
    [SerializeField] private TMP_Text _enemyCommandPointsValue;

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
        _playerCommandPoints = MAX_COMMAND_POINTS;
        _enemyCommandPoints = MAX_COMMAND_POINTS;
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
    ///     Move a given Unit to a given Tile
    /// </summary>
    /// <param name="unit"> Unit to move </param>
    /// <param name="tile"> Tile to which the unit will move </param>
    public void MoveUnitTowards(Unit unit, Tile tile)
    {
        if (unit.tile != tile)
        {
            if (GetPath(unit.tile, tile).Count > 0)
            {
                Debug.Log($"Unit should move from {unit.tile} to {GetPath(unit.tile, tile)[0]}");
                MoveUnit(unit, GetPath(unit.tile, tile)[0]);
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Calculates the distance between two units
    /// </summary>
    /// <param name="unit1"> First unit from which the distance is calculated </param>
    /// <param name="unit2"> Second unit from which the distance is calculated </param>
    /// <returns> Returns distance between the two units </returns>
    public int GetDistance(Unit unit1, Unit unit2)
    {
        return Mathf.Abs(unit1.tile.x - unit2.tile.x) + Mathf.Abs(unit1.tile.y - unit2.tile.y);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Calculates the distance between two tiles
    /// </summary>
    /// <param name="tile1"> First tile from which the distance is calculated </param>
    /// <param name="tile2"> Second tile from which the distance is calculated </param>
    /// <returns> Returns distance between the two tiles </returns>
    public int GetDistance(Tile tile1, Tile tile2)
    {
        return Mathf.Abs(tile1.x - tile2.x) + Mathf.Abs(tile1.y - tile2.y);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return units ordered by highest initiative
    /// </summary>
    /// <param name="units"> Units to sort by distance and initiative </param>
    /// <returns> Units ordered by distance and initiative </returns>
    public List<Unit> OrderUnitsByInitiative(List<Unit> units)
    {
        List<Unit> orderedUnits = units.OrderByDescending(unit => unit.initiative).ToList();
        return orderedUnits;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return units ordered by closest distance from a given tile and highest initiative
    /// </summary>
    /// <param name="tile"> Tile from which the distance is calculated </param>
    /// <param name="units"> Units to sort by distance and initiative </param>
    /// <returns> Units ordered by distance and initiative </returns>
    public List<Unit> OrderUnitsByDistanceAndInitiative(Tile tile, List<Unit> units)
    {
        List<Unit> orderedUnits = new List<Unit>();
        // If there are units in the given unit list
        if (units.Count > 0)
        {
            orderedUnits = units.OrderBy(unit => GetPath(unit.tile, tile).Count * 10 + (10 - unit.initiative)).ToList();
        }

        return orderedUnits;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return tile path between two given tiles
    /// </summary>
    /// <param name="startTile"> First tile to calculate the path from </param>
    /// <param name="endTile"> Second tile to calculate the path from </param>
    /// <returns> Return tile path between the two given tiles </returns>
    public List<Tile> GetPath(Tile startTile, Tile endTile)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        startTile.tileCost = 0;
        openList.Add(startTile);

        Tile currentTile;
        while (openList.Count > 0)
        {
            currentTile = LowestCostTile(openList);

            if (currentTile == endTile)
            {
                //Debug.Log("Found end tile");
                return MakePath(endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (Tile neighbourTile in GetNeighbourTiles(currentTile))
            {
                if (closedList.Contains(neighbourTile) || (!neighbourTile.FreeForMovement() && neighbourTile != endTile))
                {
                    continue;
                }

                int cost = currentTile.tileCost + 10;

                if (neighbourTile.tileCost > cost)
                {
                    neighbourTile.cameFromTile = currentTile;
                    neighbourTile.tileCost = cost;

                    if (!openList.Contains(neighbourTile))
                    {
                        //Debug.Log($"Added NeighbourTile {neighbourTile}");
                        openList.Add(neighbourTile);
                    }
                }
            }
        }
        //Debug.Log("No end tile");
        return null;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return all neighbours tiles
    /// </summary>
    /// <param name="tile"> Tile from which to get all neighbour tiles </param>
    /// <returns> Return list of neighbour tiles </returns>
    public List<Tile> GetNeighbourTiles(Tile tile)
    {
        List<Tile> neighbourTiles = new List<Tile>();

        if (GetTile(tile.x + 1, tile.y) != null)
        {
            neighbourTiles.Add(GetTile(tile.x + 1, tile.y));
        }
        if (GetTile(tile.x - 1, tile.y) != null)
        {
            neighbourTiles.Add(GetTile(tile.x - 1, tile.y));
        }
        if (GetTile(tile.x, tile.y + 1) != null)
        {
            neighbourTiles.Add(GetTile(tile.x, tile.y + 1));
        }
        if (GetTile(tile.x, tile.y - 1) != null)
        {
            neighbourTiles.Add(GetTile(tile.x, tile.y - 1));
        }

        return neighbourTiles;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return lowest cost tile from a list of tiles
    /// </summary>
    /// <param name="tiles"> Tiles to select the lowest cost tile from </param>
    /// <returns> Return lowest cost tile </returns>
    public Tile LowestCostTile(List<Tile> tiles)
    {
        return tiles.OrderBy(tile => tile.tileCost).ToList()[0];
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return the path given the cameFromTile in each tile
    /// </summary>
    /// <param name="tile"> Tiles to return the path from </param>
    /// <returns> Return optimal path </returns>
    public List<Tile> MakePath(Tile tile)
    {
        //Debug.Log("Path made");
        List<Tile> path = new List<Tile>();
        path.Add(tile);
        while (tile.cameFromTile != null)
        {
            tile = tile.cameFromTile;
            path.Add(tile);
        }
        // Remove end tile
        if (path.Count > 0)
        {
            path.Remove(path[0]);
        }

        // Reverse the order, to start from the starting tile
        path.Reverse();
        // Remove starting tile
        if (path.Count > 0)
        {
            path.Remove(path[0]);
        }

        ResetTiles();
        return path;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return the path given the cameFromTile in each tile
    /// </summary>
    /// <param name="tile"> Tiles to return the path from </param>
    /// <returns> Return optimal path </returns>
    public void ResetTiles()
    {
        foreach (Tile tile in _tiles.Values)
        {
            tile.cameFromTile = null;
            tile.tileCost = 9999;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Whether or not a given unit can attack a given target unit
    /// </summary>
    /// <param name="attackingUnit"> Attacking unit </param>
    /// <param name="targetUnit"> Target unit </param>
    /// <returns> Return true if targetUnit is in range of attackingUnit </returns>
    public bool CanAttack(Unit attackingUnit, Unit targetUnit)
    {
        if (attackingUnit != null && targetUnit != null)
        {
            return GetDistance(attackingUnit.tile, targetUnit.tile) <= attackingUnit.range;
        }
        return false;
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
            // If the corresponding player's command points are sufficient to place down the unit
            if ((_selectedUnit.faction == Faction.Friendly && _playerCommandPoints >= _selectedUnit.commandPoints) || (_selectedUnit.faction == Faction.Enemy && _enemyCommandPoints >= _selectedUnit.commandPoints))
            {
                // If the unit was in the reserve, remove it
                if (Reserve.instance.IsInReserve(_selectedUnit))
                {
                    Reserve.instance.RemoveUnit(_selectedUnit);
                }
                if (EnemyReserve.instance.IsInReserve(_selectedUnit))
                {
                    EnemyReserve.instance.RemoveUnit(_selectedUnit);
                }
                // If the unit is not yet on the board, add it to the corresponding list
                if (!GetAllUnits().Contains(_selectedUnit))
                {
                    if (_selectedUnit.faction == Faction.Friendly)
                    {
                        _playerUnits.Add(_selectedUnit);
                        _selectedUnit.transform.parent = _playerUnitsParent;
                    }
                    else if (_selectedUnit.faction == Faction.Enemy)
                    {
                        _enemyUnits.Add(_selectedUnit);
                        _selectedUnit.transform.parent = _enemyUnitsParent;
                    }
                }
                UpdateCommandPoints();
                MoveUnit(_selectedUnit, tile);
                _selectedUnit.Unselect();
                _selectedUnit = null;
            }
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
        UpdateCommandPoints();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns a list of all units
    /// </summary>
    /// </returns> List of all units
    public List<Unit> GetAllUnits()
    {
        List<Unit> allUnits = new List<Unit>();

        foreach (Unit playerUnit in _playerUnits)
        {
            allUnits.Add(playerUnit);
        }

        foreach (Unit enemyUnit in _enemyUnits)
        {
            allUnits.Add(enemyUnit);
        }

        // Randomly shuffles the list
        int n = allUnits.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Unit unit = allUnits[k];
            allUnits[k] = allUnits[n];
            allUnits[n] = unit;
        }

        return allUnits;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Update command points totals
    /// </summary>
    public void UpdateCommandPoints()
    {
        // Update player command points
        _playerCommandPoints = MAX_COMMAND_POINTS;
        for (int i = 0; i < _playerUnits.Count; i++)
        {
            _playerCommandPoints -= _playerUnits[i].commandPoints;
        }
        _playerCommandPointsValue.text = $"{_playerCommandPoints}/{MAX_COMMAND_POINTS}";

        // Update enemy command points
        _enemyCommandPoints = MAX_COMMAND_POINTS;
        for (int i = 0; i < _enemyUnits.Count; i++)
        {
            _enemyCommandPoints -= _enemyUnits[i].commandPoints;
        }
        _enemyCommandPointsValue.text = $"{_enemyCommandPoints}/{MAX_COMMAND_POINTS}";
    }
}
