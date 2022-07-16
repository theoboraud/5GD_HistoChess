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
    private const int START_COMMAND_POINTS = 8;                 // Number of command points at the start of the game
    private const int COMMAND_POINTS_INCREMENT = 4;             // How much command points each player gains when reaching a new tier

    // GameManager static instance
    public static GameManager instance;

    // References
    public Color formationLevelOneStatColor;
    public Color formationLevelTwoStatColor;
    public Color formationLevelThreeStatColor;
    public Camera camera;
    public Vector3 cameraPlanificationPosition;
    public Quaternion cameraPlanificationRotation;
    public Vector3 cameraBattlePosition;
    public Quaternion cameraBattleRotation;
    public Quaternion unitBattleRotation;

    // Variables
    public bool unknownEnemyStats = false;
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

        if (Board.instance != null)
        {
            Board.instance.DarkEnemyTiles(true);
        }

        if (Shop.instance != null)
        {
            Shop.instance.EnableShop(true);
            Shop.instance.UpdateShop();
        }

        if (Reserve.instance != null)
        {
            Reserve.instance.EnableReserve(true);
        }

        if (Player.instance != null)
        {
            Player.instance.EnableUI(true);
        }

        if (EnemyShop.instance != null)
        {
            EnemyShop.instance.UpdateShop();
        }

        if (Board.instance != null)
        {
            for (int i = 0; i < Board.instance.xSize; i++)
            {
                for (int j = 0; j < Board.instance.ySize; j++)
                {
                    Board.instance.GetTile(i, j).FeedbackBlood(false);
                    Board.instance.GetTile(i, j).FeedbackShadow(false);
                }
            }
        }

        camera.transform.position = cameraPlanificationPosition;
        camera.transform.rotation = cameraPlanificationRotation;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Go to battle
    /// </summary>
    public void BattleMode()
    {
        _gameMode = GameMode.Battle;

        Board.instance.DarkEnemyTiles(false);
        Shop.instance.EnableShop(false);
        Reserve.instance.EnableReserve(false);
        Player.instance.EnableUI(false);

        camera.transform.position = cameraBattlePosition;
        camera.transform.rotation = cameraBattleRotation;

        foreach(Unit unit in Board.instance.playerUnits)
        {
            unit.tile.FeedbackShadow(true);
            unit.commandPointsIcon.SetActive(false);
            unit.transform.position += new Vector3(0f, 0.35f - 0.05f * unit.tile.y, 0f);
            if (unit.HasTrait(Trait.Charge))
            {
                unit.transform.position += new Vector3(0f, 0.15f - 0.025f * unit.tile.y, 0f);
            }
            unit.transform.rotation = unitBattleRotation;

        }

        foreach(Unit unit in Board.instance.enemyUnits)
        {
            unit.tile.FeedbackShadow(true);
            unit.commandPointsIcon.SetActive(false);
            unit.transform.position += new Vector3(0f, 0.35f - 0.05f * unit.tile.y, 0f);
            if (unit.HasTrait(Trait.Charge))
            {
                unit.transform.position += new Vector3(0f, 0.15f - 0.025f * unit.tile.y, 0f);
            }
            unit.transform.rotation = unitBattleRotation;
        }
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
            Board.instance.RemoveEnemyUnits();
        }
        else if (Board.instance.enemyUnits.Count == 0 && Board.instance.playerUnits.Count > 0)
        {
            //Player.instance.WinBattle();
        }
        else if (Board.instance.enemyUnits.Count == 0 && Board.instance.playerUnits.Count == 0)
        {
            // TODO: Draw results?

        }

        Player.instance.EndOfBattle();
        Board.instance.ResetPlayerUnits();

        _round++;

        if (TierUnlocked(4))
        {
            Board.instance.maxCommandPoints = START_COMMAND_POINTS + 3 * COMMAND_POINTS_INCREMENT;
        }
        else if (TierUnlocked(3))
        {
            Board.instance.maxCommandPoints = START_COMMAND_POINTS + 2 * COMMAND_POINTS_INCREMENT;
        }
        else if (TierUnlocked(2))
        {
            Board.instance.maxCommandPoints = START_COMMAND_POINTS + COMMAND_POINTS_INCREMENT;
        }
        else
        {
            Board.instance.maxCommandPoints = START_COMMAND_POINTS;
        }

        Board.instance.UpdateCommandPoints();

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
        return _round >= (tier - 1) * TIERS_ROUND_INCREMENT + 1;;
    }
}
