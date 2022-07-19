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
    public FMOD.Studio.EventInstance trait_Swarm;
    public FMOD.Studio.EventInstance formation_Start;
    public FMOD.Studio.EventInstance trait_Support;
    public FMOD.Studio.EventInstance trait_Charge;
    public FMOD.Studio.EventInstance unit_SufferCharge;
    public FMOD.Studio.EventInstance trait_Weak;
    public FMOD.Studio.EventInstance trait_Barrage;
    public FMOD.Studio.EventInstance trait_Spear;
    public FMOD.Studio.EventInstance trait_Raid;
    public FMOD.Studio.EventInstance trait_Enrage;
    public FMOD.Studio.EventInstance player_Win;
    public FMOD.Studio.EventInstance player_Defeat;
    //public FMOD.Studio.EventInstance player_Draw;
    public FMOD.Studio.EventInstance tier_Up;

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
        trait_Swarm = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Swarm");
        //formation_Start = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/Formation_Start");
        trait_Support = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Support");
        trait_Charge = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Charge");
        unit_SufferCharge = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Classic_Actions/Unit_Suffer_Charge");
        trait_Weak = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Weak");
        trait_Barrage = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Barrage");
        trait_Spear = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Spear");
        trait_Enrage = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Enrage");
        trait_Raid = FMODUnity.RuntimeManager.CreateInstance("event:/Unit_Feedbacks/Special_Traits/ST_Raid");
        player_Win = FMODUnity.RuntimeManager.CreateInstance("event:/Imagery/Player_Win");
        //player_Draw = FMODUnity.RuntimeManager.CreateInstance("event:/Imagery/Player_Draw");
        player_Defeat = FMODUnity.RuntimeManager.CreateInstance("event:/Imagery/Player_Defeat");
        tier_Up = FMODUnity.RuntimeManager.CreateInstance("event:/UI_Feedbacks/Tier_Up");
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
        //unit_Defeated.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        //unit_Defeated.start();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Unit_Feedbacks/Classic_Actions/Unit_Defeated");
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

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitSwarm(Unit unit)
    {
        trait_Swarm.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Swarm.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitSupport(Unit unit)
    {
        trait_Support.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Support.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitCharge(Unit unit)
    {
        trait_Charge.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Charge.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void UnitSufferCharge(Unit unit)
    {
        unit_SufferCharge.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        unit_SufferCharge.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitWeak(Unit unit)
    {
        trait_Weak.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Weak.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitBarrage(Unit unit)
    {
        trait_Barrage.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Barrage.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitSpear(Unit unit)
    {
        trait_Spear.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Spear.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitRaid(Unit unit)
    {
        trait_Raid.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Raid.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TraitEnrage(Unit unit)
    {
        trait_Enrage.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        trait_Enrage.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void PlayerWin()
    {
        player_Win.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void PlayerDefeat()
    {
        player_Defeat.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void PlayerDraw()
    {
        //player_Draw.start();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     TODO
    /// </summary>
    public void TierUp()
    {
        tier_Up.start();
    }
}
