using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using TMPro;

/// <summary>
///     An Unit represents the game instance of a given UnitReference in the game
///     They are bought and placed on the board during the Planning Phase,
///     and fight the opponent's units during the Battle Phase via BattleManager
/// </summary>
public class Unit : MonoBehaviour
{
    [Header("Unit stats")]
    [SerializeField] private int _hp = 1;                   // Unit health points, dies when reduced to 0
    [SerializeField] private int _power = 1;                // How much damage this unit deals when fighting
    [SerializeField] private int _speed = 1;                // How much tiles this unit moves every battle round
    [SerializeField] private int _range = 1;                // How far the unit attack can reach
    [SerializeField] private int _commandPoints = 1;        // Cost to place the unit on the board
    [SerializeField] private int _initiative = 1;           // Low initiatives will move last but be attacked first
    public Faction faction;                                 // Faction to which this unit belongs to

    [Header("References")]
    [SerializeField] private TMP_Text _powerValue;
    [SerializeField] private TMP_Text _healthValue;
    private Tile _tile;                                     // Tile on which the unit is located, if any

    // Public get/set
    public Tile tile { get => _tile; }
    public int range { get => _range; }
    public int initiative { get => _initiative; }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init class variables
    /// </summary>
    public void Init()
    {
        // TODO: Add all variable to init and their init value from the UnitReference
        UpdateStats();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Update all stats and their in-game visual
    /// </summary>
    public void UpdateStats()
    {
        _powerValue.text = _power.ToString();
        _healthValue.text = _hp.ToString();
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
    public void TakeDamage(int damage)
    {
        // Minimum 0hp
        _hp = Mathf.Clamp(_hp - damage, 0, _hp);
        UpdateStats();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Check if this unit has been killed
    /// </summary>
    /// <returns> Whether or not this unit should die </param>
    public bool CheckDeath()
    {
        return _hp == 0;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Death behaviour when this unit dies
    /// </summary>
    public void Die()
    {
        // TODO: Change death behaviour (graveyard?)
        Board.instance.KillUnit(this);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the unit
    /// </summary>
    public void OnMouseDown()
    {
        Board.instance.SelectUnit(this);
    }
}
