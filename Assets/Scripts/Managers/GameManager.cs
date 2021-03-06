using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     The GameManager class manages the game flow (states, game phases...) and references used in the game (settings, units...)
/// </summary>
public class GameManager : MonoBehaviour
{
    // Constants
    private const int TIERS_ROUND_INCREMENT = 2;                // Number of rounds needed to increment the shop Tier

    // GameManager static instance
    public static GameManager instance;

    // Variables
    private GameMode _gameMode;
    private int _round;                                         // Round number

    // Public get/set
    public GameMode gameMode { get => _gameMode; }

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
        _round = 1;
        PlanificationMode();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to menu
    /// </summary>
    /// <returns> Whether or not the game is in Menu mode </param>
    public bool GetMenuMode()
    {
        return _gameMode == GameMode.Menu;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to planification
    /// </summary>
    /// <returns> Whether or not the game is in Planification mode </param>
    public bool GetPlanificationMode()
    {
        return _gameMode == GameMode.Planification;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to battle
    /// </summary>
    /// <returns> Whether or not the game is in Battle mode </param>
    public bool GetBattleMode()
    {
        return _gameMode == GameMode.Battle;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to menu
    /// </summary>
    public void MenuMode()
    {
        _gameMode = GameMode.Menu;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to planification
    /// </summary>
    public void PlanificationMode()
    {
        _gameMode = GameMode.Planification;
        if (Shop.instance != null)
        {
            Shop.instance.UpdateShop();
        }
        if (EnemyShop.instance != null)
        {
            EnemyShop.instance.UpdateShop();
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to battle
    /// </summary>
    public void BattleMode()
    {
        _gameMode = GameMode.Battle;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called at the end of the round to start off the new round
    /// </summary>
    public void EndOfRound()
    {
        // Check what happens to the player depending on whether or not he won the battle
        if (Board.instance.playerUnits.Count == 0 && Board.instance.enemyUnits.Count > 0)
        {
            Player.instance.LoseHealthPoints(1);
            Player.instance.EndOfBattle();
            Board.instance.RemoveEnemyUnits();
            Board.instance.ResetPlayerUnits();
        }
        else if (Board.instance.enemyUnits.Count == 0 && Board.instance.playerUnits.Count > 0)
        {
            Player.instance.EndOfBattle();
            Board.instance.ResetPlayerUnits();
        }
        else if (Board.instance.enemyUnits.Count == 0 && Board.instance.playerUnits.Count == 0)
        {
            // TODO: Draw results?
            Board.instance.ResetPlayerUnits();
        }

        _round++;
        PlanificationMode();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns whether or not the given tier is unlocked in the shop, depending on the number of rounds
    /// </summary>
    /// <param name="tier"> Tier to check </param>
    /// <returns> True if the tier is unlocked, false otherwise </returns>
    public bool TierUnlocked(int tier)
    {
        return _round >= (tier - 1) * TIERS_ROUND_INCREMENT + 1;
    }
}
