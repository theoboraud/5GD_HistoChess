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
    public static BattleManager instance;                       // BattleManager static instance
    private bool _movePhase = false;                            // Move phase boolean. True if units should move, false if units should attack
    private List<Unit> _deathList = new List<Unit>();           // List of units dead this round
    private float _gameSpeed = 0.8f;
    private float _speedMultiplier = 1f;

    [SerializeField] private TMP_Text _btnText;

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
    ///     All units move during the Move Phase if possible
    /// </summary>
    private IEnumerator MovePhase()
    {
        List<Unit> orderedUnits = Board.instance.OrderUnitsByInitiative(Board.instance.GetAllUnits());
        for (int i = 0; i < orderedUnits.Count; i++)
        {
            Unit unit = orderedUnits[i];
            unit.SelectFeedback(true);
            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);

            // For each movement point (unit's speed), the unit will move one tile towards an enemy
            for (int j = 0; j < unit.speed; j++)
            {
                Unit targetUnit = null;

                if (unit.tile != null)
                {
                    if (unit.faction == Faction.Friendly && Board.instance.enemyUnits.Count > 0)
                    {
                        //Debug.Log($"{unit.name} looking for move target");
                        List<Unit> enemyOrderedUnits = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits);
                        if (enemyOrderedUnits.Count > 0)
                        {
                            targetUnit = enemyOrderedUnits[0];
                            //Debug.Log($"{unit.name} has found {targetUnit.name}");
                        }
                    }
                    else if (unit.faction == Faction.Enemy && Board.instance.playerUnits.Count > 0)
                    {
                        //Debug.Log($"{unit.name} looking for move target");
                        List<Unit> playerOrderedUnits = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.playerUnits);
                        if (playerOrderedUnits.Count > 0)
                        {
                            targetUnit = playerOrderedUnits[0];
                            //Debug.Log($"{unit.name} has found {targetUnit.name}");
                        }
                    }
                }

                if (targetUnit != null)
                {
                    // If unit can't attack this closest target unit, it will move towards it
                    if (!Board.instance.CanAttack(unit, targetUnit))
                    {
                        Board.instance.MoveUnitTowards(unit, targetUnit.tile);
                    }
                }
            }

            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
            unit.SelectFeedback(false);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Wait for a given key
    /// </summary>
    /// <param name="key"> Key to wait for </param>
    private IEnumerator WaitForKeyPress(KeyCode key)
    {
        bool done = false;
        while(!done) // essentially a "while true", but with a bool to break out naturally
        {
            if(Input.GetKeyDown(key))
            {
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }

        // now this function returns
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     All units attack during the Attack Phase if possible
    /// </summary>
    private IEnumerator AttackPhase()
    {
        List<Unit> allUnits = Board.instance.GetAllUnits();

        for (int i = 0; i < allUnits.Count; i++)
        {
            Unit unit = allUnits[i];
            Unit closestHostileUnit = null;
            Unit targetUnit = null;

            unit.SelectFeedback(true);
            yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);

            if (unit.tile != null)
            {
                if (unit.faction == Faction.Friendly && Board.instance.enemyUnits.Count > 0)
                {
                    Debug.Log($"{unit.name} looking for attack target");
                    List<Unit> enemyOrderedUnits = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits);
                    if (enemyOrderedUnits.Count > 0)
                    {
                        closestHostileUnit = enemyOrderedUnits[0];
                        Debug.Log($"{unit.name} has found {closestHostileUnit.name}");
                    }
                }

                else if (unit.faction == Faction.Enemy && Board.instance.playerUnits.Count > 0)
                {
                    Debug.Log($"{unit.name} looking for attack target");
                    List<Unit> playerOrderedUnits = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.playerUnits);
                    if (playerOrderedUnits.Count > 0)
                    {
                        closestHostileUnit = playerOrderedUnits[0];
                        Debug.Log($"{unit.name} has found {closestHostileUnit.name}");
                    }
                }

                // If closest enemy if too far, no attack target for this turn
                if (Board.instance.CanAttack(unit, closestHostileUnit))
                {
                    Debug.Log($"{unit} can attack {closestHostileUnit}");
                    targetUnit = closestHostileUnit;
                }
            }

            if (targetUnit != null)
            {
                UnitAttack(unit, targetUnit);
                targetUnit.HurtFeedback(true);
                yield return new WaitForSeconds(_gameSpeed / _speedMultiplier);
                targetUnit.HurtFeedback(false);
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
    public void NextPhase()
    {
        _movePhase = !_movePhase;

        if (_movePhase)
        {
            StartCoroutine("MovePhase");
            _btnText.text = "Attack Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.red;
        }
        else
        {
            StartCoroutine("AttackPhase");
            _btnText.text = "Move Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.green;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    private IEnumerator AutoPhase()
    {
        while (Board.instance.playerUnits.Count > 0 && Board.instance.enemyUnits.Count > 0)
        {
            _btnText.text = "Move Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.green;
            yield return StartCoroutine("MovePhase");

            _btnText.text = "Attack Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.red;
            yield return StartCoroutine("AttackPhase");
        }
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
