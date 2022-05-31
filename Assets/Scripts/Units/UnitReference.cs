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
[System.Serializable]
[CreateAssetMenu(fileName = "UnitReference", menuName = "ScriptableObjects/UnitReference")]
public class UnitReference : ScriptableObject
{
    [Header("Unit stats")]
    public int power = 1;                       // How much damage this unit deals when fighting
    public int hp = 1;                          // Unit health points, dies when reduced to 0
    public int speed = 1;                       // How much tiles this unit moves every battle round
    public int range = 1;                       // How far the unit attack can reach
    public int commandPoints = 1;               // Cost to place the unit on the board
    public int initiative = 1;                  // Low initiatives will move last but be attacked first
    public Faction faction;                     // Faction to which this unit belongs to

    [Header("Unit sprite")]
    public Sprite sprite;                       // Unit sprite reference              
}
