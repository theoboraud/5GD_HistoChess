using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enums
{
    public enum Faction
    {
        Friendly,
        Enemy
    }

    public enum Trait
    {
        Unarmed,
        Barrage,
        Charge,
        Distance,
        Enrage
    }

    public enum GameMode
    {
        Menu,
        Planification,
        Battle
    }

    [System.Serializable]
    public struct ArmyUnit
    {
        public UnitReference unitReference;
        public Vector2 position;
    }
}
