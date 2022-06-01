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
    public void SpawnUnit(UnitReference unitReference)
    {
        // If there is still some place for new units in the reserve
        if (_reserveUnits.Count < _reserveZones.Count)
        {
            GameObject _spawnedUnitGO = Instantiate(_unitPrefab);
            Unit _spawnedUnit = _spawnedUnitGO.GetComponent<Unit>();
            AddUnit(_spawnedUnit);
            _spawnedUnit.LoadUnitReference(unitReference, _faction);
        }
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
            unit.transform.parent = _reserveZones[_reserveUnits.Count];
            unit.transform.position = unit.transform.parent.position;
            unit.transform.localScale = Vector3.one * 1.5f;
            _reserveUnits.Add(unit);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Remove the given unit from the reserve
    /// </summary>
    /// <param name="unit"> Unit to remove from the reserve </param>
    public void RemoveUnit(Unit unit)
    {
        _reserveUnits.Remove(unit);
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
}
