using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;

/// <summary>
///     The BattleManager class manages every battle happening during the Battle Phase
///     It acts as an intermediary between units on the board (move, fight, death...)
///     It also determines whether or not a player has won the battle
/// </summary>
public class BattleManager : MonoBehaviour
{
    // References
    public static BattleManager instance;                       // BattleManager static instance
    private List<Unit> _deathList = new List<Unit>();           // List of units dead this round
    private float _gameSpeed = 0.8f;

    [SerializeField] private List<ArmyComposition> tierOneArmyCompositions = new List<ArmyComposition>();       // List of army composition in tier 1
    [SerializeField] private List<ArmyComposition> tierTwoArmyCompositions = new List<ArmyComposition>();       // List of army composition in tier 2
    [SerializeField] private List<ArmyComposition> tierThreeArmyCompositions = new List<ArmyComposition>();     // List of army composition in tier 3
    [SerializeField] private List<ArmyComposition> tierFourArmyCompositions = new List<ArmyComposition>();      // List of army composition in tier 4

    [SerializeField] private TMP_Text _btnText;
    [SerializeField] private GameObject _unitPrefab;

    // Variables
    private bool _movePhase = false;                            // Move phase boolean. True if units should move, false if units should attack
    private float _speedMultiplier = 1f;

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
    ///     Player units attack or move if they can't attack
    /// </summary>
    private IEnumerator PlayerPhase()
    {
        List<Unit> orderedPlayerUnits = Board.instance.OrderUnitsByInitiative(Board.instance.playerUnits);

        for (int i = 0; i < orderedPlayerUnits.Count; i++)
        {
            Unit unit = orderedPlayerUnits[i];
            Unit targetUnit = null;

            unit.SelectFeedback(true);
            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);

            if (Board.instance.enemyUnits.Count > 0)
            {
                int speed = unit.speed;
                // For each unit speed point
                for (int j = 0; j < speed; j++)
                {
                    if (unit != null)
                    {
                        targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits, true);

                        // TESTING ONLY
                        //yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                        //Board.instance.ResetTilesFeedbacks();

                        // Move the unit if not target available
                        if (!Board.instance.CanAttack(unit, targetUnit) && !unit.hasAttacked)
                        {
                            targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits);

                            if (targetUnit != null && !unit.stunned)
                            {
                                Board.instance.MoveUnitTowards(unit, targetUnit.tile);

                                // TESTING ONLY
                                //Board.instance.ResetTilesFeedbacks();
                            }
                        }
                        yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);

                        targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits, true);

                        if (unit.HasTrait(Trait.Reload) && unit.hasToReload)
                        {
                            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                            unit.hasToReload = false;
                            Debug.Log("RELOAD");
                        }
                        // If the unit can attack the target unit, a fight occurs
                        else if (Board.instance.CanAttack(unit, targetUnit))
                        {
                            UnitAttack(unit, targetUnit);
                            targetUnit.HurtFeedback(true);
                            // If the target unit can attack back
                            if (Board.instance.CanAttack(targetUnit, unit))
                            {
                                UnitAttack(targetUnit, unit, targetUnit.HasTrait(Trait.Weak));
                                unit.HurtFeedback(true);
                            }
                            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                            // Reset hurt feedbacks
                            targetUnit.HurtFeedback(false);
                            unit.HurtFeedback(false);

                            unit.hasAttacked = true;

                            KillDeathList();
                        }
                    }
                }
            }

            if (unit != null)
            {
                // Reset stunned variable
                unit.stunned = false;
                unit.movePointsUsed = 0;
                unit.hasAttacked = false;
                unit.SelectFeedback(false);
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Enemy units attack or move if they can't attack
    /// </summary>
    private IEnumerator EnemyPhase()
    {
        List<Unit> orderedEnemyUnits = Board.instance.OrderUnitsByInitiative(Board.instance.enemyUnits);

        for (int i = 0; i < orderedEnemyUnits.Count; i++)
        {
            Unit unit = orderedEnemyUnits[i];
            Unit targetUnit = null;

            unit.SelectFeedback(true);
            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);

            if (Board.instance.playerUnits.Count > 0)
            {
                int speed = unit.speed;
                // For each unit speed point
                for (int j = 0; j < speed; j++)
                {
                    if (unit != null)
                    {
                        targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.playerUnits, true);

                        // TESTING ONLY
                        //yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                        //Board.instance.ResetTilesFeedbacks();

                        // Move the unit if not target available
                        if (!Board.instance.CanAttack(unit, targetUnit) && !unit.hasAttacked)
                        {
                            targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.playerUnits);

                            if (targetUnit != null && !unit.stunned)
                            {
                                Board.instance.MoveUnitTowards(unit, targetUnit.tile);

                                // TESTING ONLY
                                //Board.instance.ResetTilesFeedbacks();
                            }
                        }
                        yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);

                        targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.playerUnits, true);

                        if (unit.HasTrait(Trait.Reload) && unit.hasToReload)
                        {
                            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                            unit.hasToReload = false;
                            Debug.Log("RELOAD");
                        }
                        // If the unit can attack the target unit, a fight occurs
                        else if (Board.instance.CanAttack(unit, targetUnit))
                        {
                            UnitAttack(unit, targetUnit);
                            targetUnit.HurtFeedback(true);
                            // If the target unit can attack back
                            if (Board.instance.CanAttack(targetUnit, unit))
                            {
                                UnitAttack(targetUnit, unit, targetUnit.HasTrait(Trait.Weak));
                                unit.HurtFeedback(true);
                            }
                            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                            // Reset hurt feedbacks
                            targetUnit.HurtFeedback(false);
                            unit.HurtFeedback(false);

                            unit.hasAttacked = true;

                            KillDeathList();
                        }
                    }
                }
            }

            if (unit != null)
            {
                // Reset stunned variable
                unit.stunned = false;
                unit.movePointsUsed = 0;
                unit.hasAttacked = false;
                unit.SelectFeedback(false);
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     When an unit attacks another unit
    /// </summary>
    /// <param name="attackingUnit"> Unit attacking the target unit </param>
    /// <param name="targetUnit"> Unit targeted by the attack </param>
    /// <param name="isWeak"> If the attacking unit is weak or not </param>
    private void UnitAttack(Unit attackingUnit, Unit targetUnit, bool isWeak = false)
    {
        if (attackingUnit.faction != targetUnit.faction)
        {
            int damage = attackingUnit.GetDamage(targetUnit);

            if (isWeak)
            {
                damage--;
            }

            if (targetUnit.HasTrait(Trait.Enrage) && !targetUnit.hasEnraged)
            {
                targetUnit.hasEnraged = true;
            }
            else
            {
                targetUnit.TakeDamage(damage);
            }

            //Debug.Log($"BattlePhase.UnitAttack: {attackingUnit} deals {damage} damage to {targetUnit}");

            // If the attackingUnit has the Barrage trait, targetUnit becomes stunned
            if (attackingUnit.HasTrait(Trait.Barrage))
            {
                targetUnit.stunned = true;
            }

            // If the target unit has 0 hp, it will die at the end of the round
            if (targetUnit.CheckDeath())
            {
                _deathList.Add(targetUnit);
            }

            // If unit has to reload after attack, set hasToReload to true
            if (attackingUnit.HasTrait(Trait.Reload))
            {
                attackingUnit.hasToReload = true;
                Debug.Log($"RELOAD NEEDED: hasToReload = {attackingUnit.hasToReload}");
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Kill every units in the death list
    /// </summary>
    private void KillDeathList()
    {
        for (int i = 0; i < _deathList.Count; i++)
        {
            Unit deadUnit = _deathList[i];
            deadUnit.Die();
        }
        _deathList.Clear();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Calls the next phase in Battle Phase : Move => Attack => Move => Attack => Move...
    /// </summary>
    public IEnumerator NextPhase()
    {
        _movePhase = !_movePhase;

        if (_movePhase)
        {
            _btnText.text = "Player Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.red;
            yield return StartCoroutine("PlayerPhase");
        }
        else
        {
            _btnText.text = "Enemy Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.green;
            yield return StartCoroutine("EnemyPhase");
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    private IEnumerator AutoPhase()
    {
        // Unselect currently selected unit
        Board.instance.ResetSelection();
        Board.instance.SavePlayerUnits();

        GameManager.instance.BattleMode();

        LoadArmyCompositionDependingOnTier();

        while (Board.instance.playerUnits.Count > 0 && Board.instance.enemyUnits.Count > 0)
        {
            yield return StartCoroutine("NextPhase");
        }

        GameManager.instance.EndOfRound();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void StartAutoPhase()
    {
        if (Board.instance.CanLaunchBattle())
        {
            StartCoroutine("AutoPhase");
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Update the game speed
    /// </summary>
    public void UpdateGameSpeed(float speedMultiplier)
    {
        _speedMultiplier = speedMultiplier;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void LoadArmyCompositionDependingOnTier()
    {
        if (GameManager.instance.TierUnlocked(4))
        {
            ArmyComposition armyComposition = tierFourArmyCompositions[Random.Range(0, tierFourArmyCompositions.Count - 1)];
            LoadArmyComposition(armyComposition);
        }
        else if (GameManager.instance.TierUnlocked(3))
        {
            ArmyComposition armyComposition = tierThreeArmyCompositions[Random.Range(0, tierThreeArmyCompositions.Count - 1)];
            LoadArmyComposition(armyComposition);
        }
        else if (GameManager.instance.TierUnlocked(2))
        {
            ArmyComposition armyComposition = tierTwoArmyCompositions[Random.Range(0, tierTwoArmyCompositions.Count - 1)];
            LoadArmyComposition(armyComposition);
        }
        else
        {
            ArmyComposition armyComposition = tierOneArmyCompositions[Random.Range(0, tierOneArmyCompositions.Count - 1)];
            LoadArmyComposition(armyComposition);
        }

        Board.instance.UpdateCommandPoints();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void LoadArmyComposition(ArmyComposition armyComposition)
    {
        foreach(ArmyUnit armyUnit in armyComposition.armyUnits)
        {
            SpawnUnit(armyUnit.unitReference, armyUnit.position);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Spawns a given unit on the board
    /// </summary>
    /// <param name="unitReference"> Unit Reference of the unit to spawn in the reserve </param>
    public void SpawnUnit(UnitReference unitReference, Vector2 position)
    {
        GameObject spawnedUnitGO = Instantiate(_unitPrefab);
        //spawnedUnitGO.name = $"{_faction}Soldier_{_unitsCount}";
        spawnedUnitGO.transform.parent = Board.instance.enemyUnitsParent;
        spawnedUnitGO.transform.localScale = Vector3.one * 1.4f;

        Unit spawnedUnit = spawnedUnitGO.GetComponent<Unit>();
        Board.instance.MoveUnit(spawnedUnit, Board.instance.GetTile((int) position.x, (int) position.y));
        spawnedUnit.LoadUnitReference(unitReference, Faction.Enemy);
        Board.instance.enemyUnits.Add(spawnedUnit);
    }
}
