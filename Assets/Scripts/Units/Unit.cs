using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     An Unit represents the game instance of a given UnitReference in the game
///     They are bought and placed on the board during the Planning Phase,
///     and fight the opponent's units during the Battle Phase via BattleManager
/// </summary>
public class Unit : MonoBehaviour
{
    private int _hp;                                    // Unit health points, dies when reduced to 0
    private int _power;                                 // How much damage this unit deals when fighting
    private int _speed;                                 // How much tiles this unit moves every battle round
    private int _range;                                 // How far the unit attack can reach
    private int _commandPoints;                         // Cost to place the unit on the board
    private int _initiative;                            // Low initiatives will move last but be attacked first
    public Tile _tile;                                  // Tile on which the unit is located, if any

    // Public get/set
    public Tile tile { get => _tile; }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init class variables
    /// </summary>
    public void Init()
    {
        // TODO: Add all variable to init and their init value
        _hp = 1;
        _power = 1;
        _speed = 1;
        _range = 1;
        _commandPoints = 1;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Move this Unit to a given Tile
    /// </summary>
    /// <param name="tile"> Tile to which this unit will move </param>
    public void Move(Tile tile)
    {
        transform.position = tile.transform.position;
        _tile = tile;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns the damage dealt to a given target unit
    /// </summary>
    /// <param name="targetUnit"> Target unit which will be dealt damage </param>
    /// <returns> Damage dealt to targetUnit </param>
    public int GetDamage(Unit targetUnit)
    {
        // TODO: Change power depending on unit modificators, targetUnit type, etc...
        return _power;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when this unit receives damage
    /// </summary>
    /// <param name="damage"> How much damage this unit is dealt </param>
    public void DealtDamage(int damage)
    {
        // Minimum 0hp
        _hp = Mathf.Clamp(_hp - damage, 0, _hp);
        // checks if the unit is now dead
        CheckDeath();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Check if this unit survives the damage it has been dealt
    /// </summary>
    public void CheckDeath()
    {
        if (_hp == 0)
        {
            Die();
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Death behaviour when this unit dies
    /// </summary>
    public void Die()
    {
        // TODO: Change death behaviour (graveyard?)
        Destroy(this);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the unit
    /// </summary>
    public void Select()
    {
        Board.instance.SelectUnit(this);
    }
}
