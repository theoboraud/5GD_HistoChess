using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     The SoundManager manages all FMOD events in the game
/// </summary>
public class SoundManager : MonoBehaviour
{
    // SoundManager static instance
    public static SoundManager instance;
    public FMOD.Studio.EventInstance unit_Selection_CAC;
    public FMOD.Studio.EventInstance unit_Attack_CAC;
    public FMOD.Studio.EventInstance unit_Attack_DIST;
    public FMOD.Studio.EventInstance unit_Move_CAC_DIST;
    public FMOD.Studio.EventInstance unit_Move_CAV;
    public FMOD.Studio.EventInstance unit_Stand_Down;
    public FMOD.Studio.EventInstance general_Button;
    public FMOD.Studio.EventInstance shop_Reroll;
    public FMOD.Studio.EventInstance unit_Buying;
    public FMOD.Studio.EventInstance allied_Arrival;
    public FMOD.Studio.EventInstance enemy_Arrival;
    public FMOD.Studio.EventInstance unit_Defeated;
    public FMOD.Studio.EventInstance battle_Start;

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
        unit_Selection_CAC = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_Selection_CAC");
        unit_Attack_CAC = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_Attack_CAC");
        unit_Attack_DIST = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_Attack_DIST");
        unit_Move_CAC_DIST = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_Move_CAC_DIST");
        unit_Move_CAV = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_Move_CAV");
        unit_Stand_Down = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_StandDown");
        general_Button = FMODUnity.RuntimeManager.CreateInstance("event:/UI_Feedbacks/General_Button");
        shop_Reroll = FMODUnity.RuntimeManager.CreateInstance("event:/UI_Feedbacks/Shop_Reroll");
        unit_Buying = FMODUnity.RuntimeManager.CreateInstance("event:/UI_Feedbacks/Buying_Unit");
        allied_Arrival = FMODUnity.RuntimeManager.CreateInstance("event:/Imagery/Allied_Arrival");
        enemy_Arrival = FMODUnity.RuntimeManager.CreateInstance("event:/Imagery/Enemy_Arrival");
        unit_Defeated = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_Defeated");
        battle_Start = FMODUnity.RuntimeManager.CreateInstance("event:/Imagery/Battle_Start");
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Unit selection CAC
    /// </summary>
    public void UnitSelectionCAC(Unit unit)
    {
        unit_Selection_CAC.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_Selection_CAC.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitMoveCAC_DIST(Unit unit)
    {
        unit_Move_CAC_DIST.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_Move_CAC_DIST.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitMoveCAV(Unit unit)
    {
        unit_Move_CAV.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_Move_CAV.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitAttackCAC(Unit unit)
    {
        unit_Attack_CAC.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_Attack_CAC.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitAttackDIST(Unit unit)
    {
        unit_Attack_DIST.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_Attack_DIST.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitStandDown(Unit unit)
    {
        unit_Stand_Down.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_Stand_Down.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitDefeated(Unit unit)
    {
        unit_Defeated.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_Defeated.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitBuying()
    {
        unit_Buying.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void ButtonPressed()
    {
        general_Button.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void ShopReroll()
    {
        shop_Reroll.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void AlliedArrival(Tile tile)
    {
        allied_Arrival.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(tile.gameObject));
        allied_Arrival.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void EnemyArrival(Tile tile)
    {
        enemy_Arrival.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(tile.gameObject));
        enemy_Arrival.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void BattleStart()
    {
        battle_Start.start();
    }
}
