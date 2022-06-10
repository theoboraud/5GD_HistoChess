using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     An Unit Reference contains all data related to a specific unit in the game
///     Each Unit Reference contains statistics, sprites, audio files...
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "UnitReference", menuName = "ScriptableObjects/UnitReference")]
public class UnitReference : ScriptableObject
{
    //[Header("Unit name")]
    //public string unitName = "";                // Unit name

    [Header("Unit stats")]
    public int power = 1;                       // How much damage this unit deals when fighting
    public int hp = 1;                          // Unit health points, dies when reduced to 0
    public int speed = 1;                       // How much tiles this unit moves every battle round
    public int range = 1;                       // How far the unit attack can reach
    public int commandPoints = 1;               // Cost to place the unit on the board
    public int initiative = 1;                  // Low initiatives will move last but be attacked first

    [Header("Unit traits")]
    public List<Trait> traits = new List<Trait>();  // Unit traits

    [Header("Unit sprites")]
    public Sprite friendlySprite;               // Unit friendly sprite reference
    public Sprite enemySprite;                  // Unit enemy sprite reference
}
