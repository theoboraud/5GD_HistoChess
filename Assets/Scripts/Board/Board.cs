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
    public int maxCommandPoints = 8;
    [SerializeField] private int _xSize = 8;
    [SerializeField] private int _ySize = 8;
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
    [SerializeField] private GameObject _fog;

    // Public get/set
    public List<Unit> playerUnits { get => _playerUnits; set => _playerUnits = value; }
    public List<Unit> enemyUnits { get => _enemyUnits; set => _enemyUnits = value; }
    public Unit selectedUnit { get => _selectedUnit; set => _selectedUnit = value; }
    public Transform enemyUnitsParent { get => _enemyUnitsParent; set => _enemyUnitsParent = value; }
    public int xSize { get => _xSize; }
    public int ySize { get => _ySize; }

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
        _playerCommandPoints = 0;
        _enemyCommandPoints = 0;
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

        UpdateCommandPoints();

        Board.instance.DarkEnemyTiles(true);
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
        // Set new tile's unit reference
        tile.unit = unit;

        if (unit.tile != tile)
        {
            // Reset previous tile's unit reference
            if (unit.tile != null)
            {
                unit.tile.ResetUnit();
            }

            // Move unit to tile
            unit.Move(tile);
            if (GameManager.instance.gameMode == GameMode.Battle)
            {
                tile.FeedbackShadow(true);
            }

            if (_playerUnits.Contains(unit) || _enemyUnits.Contains(unit))
            {
                if (unit.HasTrait(Trait.Charge))
                {
                    SoundManager.instance.UnitMoveCAV(unit);
                }
                else
                {
                    SoundManager.instance.UnitMoveCAC_DIST(unit);
                }
            }


        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Move a given unit towards a given target unit
    /// </summary>
    /// <param name="unit"> Unit to move </param>
    /// <param name="targetTile"> Tile to which the unit will move </param>
    public void MoveUnitTowards(Unit unit, Tile targetTile)
    {
        if (unit.tile != targetTile)
        {
            List<Tile> path = GetPath(unit.tile, targetTile);
            // If both tiles are separated by at least one tile (path[0] is unit.tile)
            if (path.Count > 1)
            {
                Tile newTile = path[1];
                //Debug.Log($"Unit should move from {unit.tile} to {GetPath(unit.tile, tile)[0]}");
                if (newTile != targetTile)
                {
                    MoveUnit(unit, newTile);
                }
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
        List<Unit> orderedUnits;

        if (units[0].faction == Faction.Friendly)
        {
            orderedUnits = units.OrderByDescending(unit => unit.tile.x * 10 + unit.initiative).ToList();
        }
        else
        {
            orderedUnits = units.OrderBy(unit => unit.tile.x * 10 + unit.initiative).ToList();
        }

        return orderedUnits;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return first unit from units ordered by closest distance from a given tile and highest initiative
    /// </summary>
    /// <param name="tile"> Tile from which the distance is calculated </param>
    /// <param name="units"> Units to sort by distance and initiative </param>
    /// <param name="flyDistance"> Whether or not blocked tiles are considered </param>
    /// <returns> Units ordered by distance and initiative </returns>
    public Unit FirstUnitByDistanceAndInitiative(Tile tile, List<Unit> units, bool flyDistance = false)
    {
        Dictionary<Unit, int> dictionaryUnits = new Dictionary<Unit, int>();

        foreach (Unit unit in units)
        {
            List<Tile> path = GetPath(tile, unit.tile, flyDistance);
            // Add the unit and its path cost IF a path can be made
            if (path.Count > 0)
            {
                int pathCost = path.Count * 10;
                int priorityCost = 10 - unit.initiative;
                dictionaryUnits.Add(unit, pathCost + priorityCost);
            }
        }
        //Debug.Log($"Found {dictionaryUnits.Count} units");
        // If there are units in the given unit list
        if (dictionaryUnits.Count > 0)
        {
            int cost = 9999;
            Unit firstUnit = null;
            foreach (KeyValuePair<Unit, int> keyPair in dictionaryUnits)
            {
                if (keyPair.Value < cost)
                {
                    cost = keyPair.Value;
                    firstUnit = keyPair.Key;
                }
                //Debug.Log($"{tile.unit}: {keyPair.Key} has priority cost {keyPair.Value}");
            }

            return firstUnit;
        }
        //Debug.Log($"{orderedUnits} has length {orderedUnits.Count}");

        return null;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return tile path between two given tiles
    /// </summary>
    /// <param name="startTile"> First tile to calculate the path from </param>
    /// <param name="endTile"> Second tile to calculate the path from </param>
    /// <param name="flyDistance"> Whether or not blocked tiles are considered </param>
    /// <returns> Return tile path between the two given tiles </returns>
    public List<Tile> GetPath(Tile startTile, Tile endTile, bool flyDistance = false)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        startTile.tileCost = 0;
        openList.Add(startTile);

        Tile currentTile;
        while (openList.Count > 0)
        {
            // Get lowest cost tile
            currentTile = openList.OrderBy(tile => tile.tileCost).ToList()[0];
            // If the current tile is the target end tile
            if (currentTile == endTile)
            {
                //Debug.Log("Found end tile");
                return MakePath(endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);
            //currentTile.FeedbackSearched();

            foreach (Tile neighbourTile in GetNeighbourTiles(currentTile))
            {
                if (closedList.Contains(neighbourTile) || (!neighbourTile.FreeForMovement() && neighbourTile != endTile && !flyDistance))
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
        ResetTiles();
        return new List<Tile>();
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
    ///     Return the path given the cameFromTile in each tile
    /// </summary>
    /// <param name="tile"> Tiles to return the path from </param>
    /// <returns> Return optimal path </returns>
    public List<Tile> MakePath(Tile tile)
    {
        //Debug.Log("Path made");
        List<Tile> path = new List<Tile>();
        while (tile.cameFromTile != null)
        {
            //tile.FeedbackSearching();
            tile = tile.cameFromTile;
            path.Add(tile);
        }
        // Reverse the order, to start from the starting tile
        path.Reverse();

        ResetTiles();
        return path;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Reset pathfinding data in each tile
    /// </summary>
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
    ///     Reset tiles feedbacks
    /// </summary>
    public void ResetTilesFeedbacks()
    {
        foreach (Tile tile in _tiles.Values)
        {
            tile.FeedbackDefault();
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
        if (GameManager.instance.GetPlanificationMode())
        {
            Unit oldSelectedUnit = _selectedUnit;
            ResetSelection();

            if (oldSelectedUnit != unit)
            {
                _selectedUnit = unit;
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the tile
    ///     If a unit is selected, move selected unit to this tile
    /// </summary>
    /// <param name="tile"> Tile to move the selected unit to </param>
    public void SelectTile(Tile tile)
    {
        if (_selectedUnit != null && GameManager.instance.GetPlanificationMode())
        {
            if (_selectedUnit.faction == Faction.Friendly && GetPlayerTiles().Contains(tile))
            {
                // If the unit was in the reserve, remove it
                if (Reserve.instance.IsInReserve(_selectedUnit))
                {
                    Reserve.instance.RemoveUnit(_selectedUnit);
                }
                // If the unit is not yet on the board, add it to the corresponding list
                if (!GetAllUnits().Contains(_selectedUnit))
                {
                    if (_selectedUnit.faction == Faction.Friendly)
                    {
                        _playerUnits.Add(_selectedUnit);
                        _selectedUnit.transform.parent = _playerUnitsParent;
                    }
                }
                MoveUnit(_selectedUnit, tile);
            }

            UpdateCommandPoints();
            ResetSelection();
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
        //UpdateCommandPoints();
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
        _playerCommandPoints = 0;
        for (int i = 0; i < _playerUnits.Count; i++)
        {
            int formationLevel = GetFormationLevel(_playerUnits[i]);

            _playerUnits[i].SetFormationLevel(formationLevel);
            // TODO: CHANGE COMMAND POINTS DEPENDING ON FORMATION LEVEL FOR UNITS

            _playerCommandPoints += _playerUnits[i].commandPoints - (formationLevel - 1);
        }
        _playerCommandPointsValue.text = $"{_playerCommandPoints}/{maxCommandPoints}";

        // Update enemy command points
        _enemyCommandPoints = 0;
        for (int i = 0; i < _enemyUnits.Count; i++)
        {
            int formationLevel = GetFormationLevel(_enemyUnits[i]);

            _enemyUnits[i].SetFormationLevel(formationLevel);
            // TODO: CHANGE COMMAND POINTS DEPENDING ON FORMATION LEVEL FOR UNITS

            _enemyCommandPoints += _enemyUnits[i].commandPoints - (formationLevel - 1);
        }
        _enemyCommandPointsValue.text = $"{_enemyCommandPoints}/{maxCommandPoints}";
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the given faction's reserve
    /// </summary>
    /// <param name="faction"> Faction to which the selected reserve belongs to </param>
    public void SelectReserve(Faction _faction)
    {
        if (_selectedUnit != null)
        {
            if (_faction == Faction.Friendly && _selectedUnit.faction == Faction.Friendly)
            {
                _selectedUnit.ResetTile();
                Reserve.instance.AddUnit(_selectedUnit);
                SoundManager.instance.UnitStandDown(_selectedUnit);
                _playerUnits.Remove(_selectedUnit);
            }

            else if (_faction == Faction.Enemy && _selectedUnit.faction == Faction.Enemy)
            {
                _selectedUnit.tile.ResetUnit();
                _selectedUnit.ResetTile();
                EnemyReserve.instance.AddUnit(_selectedUnit);
                SoundManager.instance.UnitStandDown(_selectedUnit);
                _enemyUnits.Remove(_selectedUnit);
            }

            UpdateCommandPoints();
        }

        ResetSelection();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Get all tiles on player's side
    /// </summary>
    /// <returns> Returns list of player tiles </returns>
    public List<Tile> GetPlayerTiles()
    {
        List<Tile> playerTiles = new List<Tile>();

        for (int i = 0; i < _xSize/2; i++)
        {
            for (int j = 0; j < _ySize; j++)
            {
                playerTiles.Add(GetTile(i, j));
            }
        }

        return playerTiles;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Get all tiles on enemy's side
    /// </summary>
    /// <returns> Returns list of enemy tiles </returns>
    public List<Tile> GetEnemyTiles()
    {
        List<Tile> enemyTiles = new List<Tile>();

        for (int i = _xSize/2; i < _xSize; i++)
        {
            for (int j = 0; j < _ySize; j++)
            {
                enemyTiles.Add(GetTile(i, j));
            }
        }

        return enemyTiles;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Enables or disables the dark feedback on player tiles
    /// </summary>
    /// <param name="enable"> Whether or not the feedback must be enabled </param>
    public void DarkPlayerTiles(bool enable)
    {
        foreach (Tile tile in GetPlayerTiles())
        {
            if (enable)
            {
                tile.FeedbackDark();
            }
            else
            {
                tile.FeedbackDefault();
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Enables or disables the dark feedback on enemy tiles
    /// </summary>
    /// <param name="enable"> Whether or not the feedback must be enabled </param>
    public void DarkEnemyTiles(bool enable)
    {
        _fog.SetActive(enable);
        /*foreach (Tile tile in GetEnemyTiles())
        {
            if (enable)
            {
                tile.FeedbackDark();
            }
            else
            {
                tile.FeedbackDefault();
            }
        }*/
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Unselect selected unit, if any
    /// </summary>
    public void ResetSelection()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.Unselect();
            _selectedUnit = null;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Save player units in memory to load them back at the end of the battle phase
    /// </summary>
    public void SavePlayerUnits()
    {
        Player.instance.SavePlayerUnits(_playerUnits);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Reset the player units by loading them from memory
    /// </summary>
    public void ResetPlayerUnits()
    {
        RemovePlayerUnits();
        _playerUnits = Player.instance.LoadPlayerUnits();
        foreach (Unit unit in _playerUnits)
        {
            unit.gameObject.SetActive(true);
            MoveUnit(unit, unit.tile);
        }
        UpdateCommandPoints();

        Debug.Log(_playerUnits.Count);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Remove all player units from the board
    /// </summary>
    public void RemovePlayerUnits()
    {
        for (int i = _playerUnits.Count - 1; i >= 0; i--)
        {
            Unit unit = _playerUnits[i];
            unit.tile.ResetUnit();
            Destroy(unit.gameObject);
        }

        _playerUnits = new List<Unit>();
        UpdateCommandPoints();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Remove all enemy units from the board
    /// </summary>
    public void RemoveEnemyUnits()
    {
        for (int i = _enemyUnits.Count - 1; i >= 0; i--)
        {
            Unit unit = _enemyUnits[i];
            unit.tile.ResetUnit();
            _enemyUnits.Remove(unit);
            Destroy(unit.gameObject);
        }
        UpdateCommandPoints();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Determines the formation level of a given unit from its neighbour similar units
    /// </summary>
    /// <param name="unit"> Unit to look neighbour similar units to determine formation level </param>
    /// <returns> Formation level of the given unit </returns>
    public int GetFormationLevel(Unit unit)
    {
        int formationLevel = 1;

        foreach(Tile tile in GetNeighbourTiles(unit.tile))
        {
            if (tile.unit != null)
            {
                if (tile.unit.unitReference == unit.unitReference && tile.unit.faction == unit.faction)
                {
                    formationLevel = Mathf.Clamp(formationLevel + 1, 1, 3);
                }
            }
        }

        return formationLevel;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Determines if the battle phase can be launched, depending on player command points
    /// </summary>
    /// <returns> True if player command points is lesser or equal than the maximum allowed </returns>
    public bool CanLaunchBattle()
    {
        return _playerCommandPoints <= maxCommandPoints;
    }
}
