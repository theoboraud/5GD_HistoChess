using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private void MovePhase()
    {
        foreach (Unit unit in Board.instance.playerUnits)
        {
            Tile nextTile = null;

            if (unit.tile != null)
            {
                nextTile = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits)[0].tile;
            }

            if (nextTile != null)
            {
                Board.instance.MoveUnitTowards(unit, nextTile);
            }
        }

        foreach (Unit unit in Board.instance.enemyUnits)
        {
            Tile nextTile = null;

            if (unit.tile != null)
            {
                nextTile = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.playerUnits)[0].tile;
            }

            if (nextTile != null)
            {
                Board.instance.MoveUnitTowards(unit, nextTile);
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     All units attack during the Attack Phase if possible
    /// </summary>
    private void AttackPhase()
    {
        for (int i = 0; i < Board.instance.playerUnits.Count; i++)
        {
            Unit unit = Board.instance.playerUnits[i];
            Unit targetUnit = null;
            if (unit.tile != null)
            {
                targetUnit = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.enemyUnits)[0];
                // If closest enemy if too far, no attack target for this turn
                if (Board.instance.GetDistance(unit.tile, targetUnit.tile) > unit.range)
                {
                    targetUnit = null;
                }
            }

            if (targetUnit != null)
            {
                UnitAttack(unit, targetUnit);
            }
        }

        for (int i = 0; i < Board.instance.enemyUnits.Count; i++)
        {
            Unit unit = Board.instance.enemyUnits[i];
            Unit targetUnit = null;
            if (unit.tile != null)
            {
                targetUnit = Board.instance.OrderUnitsByDistanceAndInitiative(unit.tile, Board.instance.playerUnits)[0];
                // If closest enemy if too far, no attack target for this turn
                if (Board.instance.GetDistance(unit.tile, targetUnit.tile) > unit.range)
                {
                    targetUnit = null;
                }
            }

            if (targetUnit != null)
            {
                UnitAttack(unit, targetUnit);
            }
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
            MovePhase();
            _btnText.text = "Attack Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.red;
        }
        else
        {
            AttackPhase();
            _btnText.text = "Move Phase";
            _btnText.transform.parent.GetComponent<Image>().color = Color.green;
        }
    }
}
