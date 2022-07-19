using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     The zone where units spawn when bought
///     It represents the player reserve
/// </summary>
public class Reserve : MonoBehaviour
{
    // Variables
    [SerializeField] private Faction _faction;                                          // Unit faction

    // References
    public static Reserve instance;
    [SerializeField] private List<Transform> _reserveZones = new List<Transform>();     // All reserve zone where units appear on
    private List<Unit> _reserveUnits = new List<Unit>();                                // All units in the reserve
    [SerializeField] private GameObject _unitPrefab;                                    // Unit prefab
    private int _unitsCount = 0;

    public int reserveZonesCount { get => _reserveZones.Count; }
    public int unitsCount { get => _unitsCount; }

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
    private void Init()
    {
        // TODO: Add all variable to init and their init value

    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Spawns a given unit and add it in the reserve
    /// </summary>
    /// <param name="unitReference"> Unit Reference of the unit to spawn in the reserve </param>
    public void SpawnUnit(UnitReference unitReference, bool hasSwarm = false)
    {
        // If there is still some place for new units in the reserve
        if (_reserveUnits.Count < _reserveZones.Count)
        {
            GameObject spawnedUnitGO = Instantiate(_unitPrefab);
            spawnedUnitGO.name = $"{_faction}Soldier_{_unitsCount}";
            _unitsCount++;
            Unit spawnedUnit = spawnedUnitGO.GetComponent<Unit>();
            AddUnit(spawnedUnit);
            spawnedUnit.LoadUnitReference(unitReference, _faction);

            if (hasSwarm)
            {
                SoundManager.instance.TraitSwarm(spawnedUnit);
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public IEnumerator SpawnSwarmUnit(UnitReference unitReference)
    {
        yield return new WaitForSeconds(0.5f);
        SpawnUnit(unitReference, true);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Add the given unit to the reserve
    /// </summary>
    /// <param name="unit"> Unit to add to the reserve </param>
    public void AddUnit(Unit unit)
    {
        // If there is still some place for new units in the reserve
        if (_reserveUnits.Count < _reserveZones.Count)
        {
            _reserveUnits.Add(unit);
            unit.SetFormationLevel(1);
        }
        ReorderReserveUnits();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Remove the given unit from the reserve
    /// </summary>
    /// <param name="unit"> Unit to remove from the reserve </param>
    public void RemoveUnit(Unit unit)
    {
        _reserveUnits.Remove(unit);
        ReorderReserveUnits();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Sell the selected unit in the reserve
    /// </summary>
    public void SellSelectedUnit()
    {
        SoundManager.instance.ButtonPressed();
        // If there is a Player selected unit
        if (Board.instance.selectedUnit != null)
        {
            if (Board.instance.selectedUnit.faction == Faction.Friendly)
            {
                // Unselect the selected unit
                Unit unit = Board.instance.selectedUnit;
                Board.instance.ResetSelection();
                // The player gain golds when selling the unit
                Player.instance.SoldUnit(unit);

                if (_reserveUnits.Contains(unit))
                {
                    _reserveUnits.Remove(unit);
                }
                if (Board.instance.playerUnits.Contains(unit))
                {
                    Board.instance.playerUnits.Remove(unit);
                }
                Destroy(unit.gameObject);

                ReorderReserveUnits();

            }
        }
        Board.instance.UpdateCommandPoints();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Reorder all reserve units
    /// </summary>
    public void ReorderReserveUnits()
    {
        for (int i = 0; i < _reserveUnits.Count; i++)
        {
            Unit unit = _reserveUnits[i];
            unit.transform.parent = _reserveZones[i];
            unit.transform.position = unit.transform.parent.position;
            unit.transform.localScale = Vector3.one * 1.4f;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Checks if the reserve contains the given unit
    /// </summary>
    /// <param name="unit"> Unit to check if in reserve </param>
    /// <returns> Returns whether or not the given unit is in the reserve </returns>
    public bool IsInReserve(Unit unit)
    {
        return _reserveUnits.Contains(unit);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when the reserve is clicked on
    /// </summary>
    public void OnMouseDown()
    {
        Board.instance.SelectReserve(_faction);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void EnableReserve(bool enable)
    {

        foreach(Transform reserveZone in _reserveZones)
        {
            reserveZone.gameObject.SetActive(enable);
        }

        foreach(Unit unit in _reserveUnits)
        {
            unit.gameObject.SetActive(enable);
        }
    }
}
