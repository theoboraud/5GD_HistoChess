using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;

/// <summary>
///     Manages player variables and player gameplay
/// </summary>
public class Player : MonoBehaviour
{
    // Constants
    // TODO: Add to a Parameters scriptable object?
    private const int STARTING_HEALTH_POINTS = 10;
    private const int STARTING_GOLDS = 10;

    // References
    public static Player instance;                              // Player static instance
    [SerializeField] private TMP_Text _healthPointsText;        // Health point Text Mesh Pro component reference
    [SerializeField] private TMP_Text _goldsText;               // Golds Text Mesh Pro component reference
    [SerializeField] private TMP_Text _victoriesText;           // Victories Text Mesh Pro component reference
    private List<Unit> _savedPlayerUnits = new List<Unit>();    // Player units saved in memory to respawn them at the end of the round
    [SerializeField] private GameObject _unitPrefab;            // Unit prefab
    [SerializeField] private GameObject _UI;

    // Variables
    private int _healthPoints;
    private int _golds;
    private int _victories;

    // Public get/set
    public int golds { get => _golds; }

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
        _healthPoints = STARTING_HEALTH_POINTS;
        _golds = STARTING_GOLDS;

        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Update player UI values
    /// </summary>
    private void UpdateUI()
    {
        _healthPointsText.text = _healthPoints.ToString();
        _goldsText.text = _golds.ToString();
        _victoriesText.text = _victories.ToString();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Increase the amount of golds by the given value
    /// </summary>
    /// <param name="goldsToAdd"> Amount of golds to add </param>
    public void GainGolds(int goldsToAdd)
    {
        _golds += goldsToAdd;
        _golds = Mathf.Clamp(_golds, 0, 99);
        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Set the amount of golds to the given value
    /// </summary>
    /// <param name="goldsToSet"> Amount of golds to add </param>
    public void SetGolds(int goldsToSet)
    {
        _golds = goldsToSet;
        _golds = Mathf.Clamp(_golds, 0, 99);
        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the amount of golds by the given value
    /// </summary>
    /// <param name="goldsToPay"> Amount of golds to pay </param>
    public void PayGolds(int goldsToPay)
    {
        _golds -= goldsToPay;
        _golds = Mathf.Clamp(_golds, 0, 99);
        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when an unit is sold
    /// </summary>
    /// <param name="unit"> Unit sold </param>
    public void SoldUnit(Unit unit)
    {
        GainGolds(1);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the amount of health points by a given amount
    /// </summary>
    /// <param name="hpToLose"> Amount of health points to lose </param>
    public void LoseHealthPoints(int hpToLose)
    {
        _healthPoints -= hpToLose;
        _healthPoints = Mathf.Clamp(_healthPoints, 0, 99);
        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Increases the number of victories by one when the player wins a battle
    /// </summary>
    public void EndOfBattle()
    {
        // The player gains 10 golds, plus eventual unit bonuses
        int goldsToAdd = 10;

        bool raidUnit = false;
        foreach(Unit unit in Board.instance.playerUnits)
        {
            if (unit.HasTrait(Trait.Raid))
            {
                SoundManager.instance.TraitRaid(unit);
                raidUnit = true;
                goldsToAdd++;
            }
        }

        if (raidUnit)
        {
            //SoundManager.instance.TraitRaid();
        }

        SetGolds(goldsToAdd);

        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Save player units to load them at the end of battle
    /// </summary>
    /// <param name="units"> List of units to save in memory </param>
    public void SavePlayerUnits(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            SpawnPlayerUnit(unit);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Spawn a clone unit from a given unit
    /// </summary>
    /// <param name="unit"> Unit to clone for spawning </param>
    public void SpawnPlayerUnit(Unit unit)
    {
        GameObject spawnedUnitGO = Instantiate(_unitPrefab);
        spawnedUnitGO.name = unit.gameObject.name;
        spawnedUnitGO.transform.parent = unit.transform.parent;
        spawnedUnitGO.transform.localScale = unit.transform.localScale;
        spawnedUnitGO.transform.position = unit.transform.position;

        Unit spawnedUnit = spawnedUnitGO.GetComponent<Unit>();
        _savedPlayerUnits.Add(spawnedUnit);
        spawnedUnit.tile = unit.tile;
        LoadPlayerUnit(spawnedUnit, unit);
        spawnedUnitGO.SetActive(false);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Load data from a given unit to a new one
    /// </summary>
    /// <param name="loadingUnit"> Unit to load the data to </param>
    /// <param name="loadedUnit"> Unit from which the data is loaded </param>
    public void LoadPlayerUnit(Unit loadingUnit, Unit loadedUnit)
    {
        loadingUnit.LoadUnitReference(loadedUnit.unitReference, Faction.Friendly);
        loadingUnit.tile = loadedUnit.tile;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Return the saved player units stocked in memory to load them, and reset the saved list
    /// </summary>
    /// <returns> List of saved player units </returns>
    public List<Unit> LoadPlayerUnits()
    {
        List<Unit> units = _savedPlayerUnits;
        // Reset the saved list
        _savedPlayerUnits = new List<Unit>();
        return units;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void EnableUI(bool enable)
    {
        Fade[] fades = _UI.transform.GetComponentsInChildren<Fade>();

        foreach (Fade fade in fades)
        {
            if (enable)
            {
                fade.Appear();
            }
            else
            {
                fade.Disappear();
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void WinBattle()
    {
        _victories++;
    }
}
