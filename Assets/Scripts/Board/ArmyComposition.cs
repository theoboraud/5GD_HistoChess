using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     An Army Composition is used to brute-force army compositions for AI players
///     Contains all units references and their position
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "ArmyComposition", menuName = "ScriptableObjects/ArmyComposition")]
public class ArmyComposition : ScriptableObject
{
    //[Header("Unit name")]
    //public string unitName = "";                // Unit name

    [Header("Units and positions")]
    public List<ArmyUnit> armyUnits = new List<ArmyUnit>();
}
