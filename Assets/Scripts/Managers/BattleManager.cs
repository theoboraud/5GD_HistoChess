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

    [SerializeField] private TMP_Text _btnText;

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
                // Defines if the unit has already attacked this turn
                bool hasAttacked = false;

                // For each unit speed point
                for (int j = 0; j < unit.speed; j++)
                {
                    targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits, true);

                    // TESTING ONLY
                    //yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                    //Board.instance.ResetTilesFeedbacks();

                    // If the unit can attack the target unit, a fight occurs
                    if (Board.instance.CanAttack(unit, targetUnit) && !hasAttacked)
                    {
                        UnitAttack(unit, targetUnit);
                        targetUnit.HurtFeedback(true);
                        // If the target unit can attack back
                        if (Board.instance.CanAttack(targetUnit, unit) && !targetUnit.HasTrait(Trait.Unarmed))
                        {
                            UnitAttack(targetUnit, unit);
                            unit.HurtFeedback(true);
                        }
                        yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                        // Reset hurt feedbacks
                        targetUnit.HurtFeedback(false);
                        unit.HurtFeedback(false);

                        // Unit can't attack twice if it has remaining speed points
                        hasAttacked = true;
                    }
                    // Else, move the unit towars closest enemy unit
                    else
                    {
                        targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits);

                        if (targetUnit != null && !unit.stunned)
                        {
                            Board.instance.MoveUnitTowards(unit, targetUnit.tile);

                            // TESTING ONLY
                            //Board.instance.ResetTilesFeedbacks();
                        }
                    }

                    // Reset stunned variable
                    unit.stunned = false;
                }
            }

            unit.SelectFeedback(false);
        }

        KillDeathList();
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
                // Defines if the unit has already attacked this turn
                bool hasAttacked = false;

                // For each unit speed point
                for (int j = 0; j < unit.speed; j++)
                {
                    targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.playerUnits, true);

                    // TESTING ONLY
                    //yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                    //Board.instance.ResetTilesFeedbacks();

                    // If the unit can attack the target unit, a fight occurs
                    if (Board.instance.CanAttack(unit, targetUnit) && !hasAttacked)
                    {
                        UnitAttack(unit, targetUnit);
                        targetUnit.HurtFeedback(true);

                        // If the target unit can attack back
                        if (Board.instance.CanAttack(targetUnit, unit) && !targetUnit.HasTrait(Trait.Unarmed))
                        {
                            UnitAttack(targetUnit, unit);
                            unit.HurtFeedback(true);
                        }
                        yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                        // Reset hurt feedbacks
                        targetUnit.HurtFeedback(false);
                        unit.HurtFeedback(false);

                        // Unit can't attack twice if it has remaining speed points
                        hasAttacked = true;
                    }
                    // Else, move the unit towars closest enemy unit
                    else
                    {
                        targetUnit = Board.instance.FirstUnitByDistanceAndInitiative(unit.tile, Board.instance.playerUnits);

                        if (targetUnit != null && !unit.stunned)
                        {
                            Board.instance.MoveUnitTowards(unit, targetUnit.tile);

                            // TESTING ONLY
                            //Board.instance.ResetTilesFeedbacks();
                        }
                    }

                    // Reset stunned variable
                    unit.stunned = false;
                }
            }

            unit.SelectFeedback(false);
        }

        KillDeathList();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     When an unit attacks another unit
    /// </summary>
    /// <param name="attackingUnit"> Unit attacking the target unit </param>
    /// <param name="targetUnit"> Unit targeted by the attack </param>
    private void UnitAttack(Unit attackingUnit, Unit targetUnit)
    {
        if (attackingUnit.faction != targetUnit.faction)
        {
            int damage = attackingUnit.GetDamage(targetUnit);
            targetUnit.TakeDamage(damage);
            Debug.Log($"BattlePhase.UnitAttack: {attackingUnit} deals {damage} damage to {targetUnit}");

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
        StartCoroutine("AutoPhase");
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Update the game speed
    /// </summary>
    public void UpdateGameSpeed(float speedMultiplier)
    {
        _speedMultiplier = speedMultiplier;
    }
}
