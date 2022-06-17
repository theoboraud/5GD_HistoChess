using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     The GameManager class manages the game flow (states, game phases...) and references used in the game (settings, units...)
/// </summary>
public class GameManager : MonoBehaviour
{
    // GameManager static instance
    public static GameManager instance;

    // Variables
    private GameMode _gameMode;

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
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to battle
    /// </summary>
    public void BattleMode()
    {
        _gameMode = GameMode.Battle;
    }
}
