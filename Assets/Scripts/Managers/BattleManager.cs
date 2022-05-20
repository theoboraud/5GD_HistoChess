using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The BattleManager class manages every battle happening during the Battle Phase
///     It acts as an intermediary between units on the board (move, fight, death...)
///     It also determines whether or not a player has won the battle
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;                       // BattleManager static instance
    private bool _movePhase = true;                             // Move phase boolean. True if units should move, false if units should attack

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
            Tile nextTile = Board.instance.GetTile(unit.tile.x + 1, unit.tile.y);

            if (nextTile != null)
            {
                Board.instance.MoveUnit(unit, nextTile);
            }
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     All units attack during the Attack Phase if possible
    /// </summary>
    private void AttackPhase()
    {

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
        }
        else
        {
            AttackPhase();
        }
    }
}
