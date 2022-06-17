using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
///     Manages player variables and player gameplay
/// </summary>
public class Player : MonoBehaviour
{
    // Constants
    // TODO: Add to a Parameters scriptable object?
    private const int STARTING_HEALTH_POINTS = 10;
    private const int STARTING_GOLDS = 10;

    // References
    public static Player instance;                              // Player static instance
    [SerializeField] private TMP_Text _healthPointsText;        // Health point Text Mesh Pro component reference
    [SerializeField] private TMP_Text _goldsText;               // Golds Text Mesh Pro component reference

    // Variables
    private int _healthPoints;
    private int _golds;

    // Public get/set
    public int golds { get => _golds; }

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
        _healthPoints = STARTING_HEALTH_POINTS;
        _golds = STARTING_GOLDS;

        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Update player UI values
    /// </summary>
    private void UpdateUI()
    {
        _healthPointsText.text = _healthPoints.ToString();
        _goldsText.text = _golds.ToString();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Increase the amount of golds by the given value
    /// </summary>
    /// <param name="goldsToPay"> Amount of golds to add </param>
    public void GainGolds(int goldsToAdd)
    {
        _golds += goldsToAdd;
        _golds = Mathf.Clamp(_golds, 0, 99);
        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the amount of golds by the given value
    /// </summary>
    /// <param name="goldsToPay"> Amount of golds to pay </param>
    public void PayGolds(int goldsToPay)
    {
        _golds -= goldsToPay;
        _golds = Mathf.Clamp(_golds, 0, 99);
        UpdateUI();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Decrease the amount of health points by a given amount
    /// </summary>
    /// <param name="hpToLose"> Amount of health points to lose </param>
    public void LoseHealthPoints(int hpToLose)
    {
        _healthPoints -= hpToLose;
        _healthPoints = Mathf.Clamp(_healthPoints, 0, 99);
        UpdateUI();
    }
}
