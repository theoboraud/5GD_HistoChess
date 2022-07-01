using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;

/// <summary>
///     An Unit represents the game instance of a given UnitReference in the game
///     They are bought and placed on the board during the Planning Phase,
///     and fight the opponent's units during the Battle Phase via BattleManager
/// </summary>
public class Unit : MonoBehaviour, ISelectableEntity
{
    [Header("Unit stats")]
    [SerializeField] private int _power = 1;                    // How much damage this unit deals when fighting
    [SerializeField] private int _hp = 1;                       // Unit health points, dies when reduced to 0
    [SerializeField] private int _speed = 1;                    // How much tiles this unit moves every battle round
    [SerializeField] private int _range = 1;                    // How far the unit attack can reach
    [SerializeField] private int _initiative = 1;               // Low initiatives will move last but be attacked first
    [SerializeField] private int _commandPoints = 1;            // Cost to place the unit on the board
    [SerializeField] private Faction _faction;                  // Faction to which this unit belongs to

    // Variables
    private bool _stunned;

    [Header("Unit traits")]
    [SerializeField] private List<Trait> _traits = new List<Trait>();   // All unit traits

    [Header("References")]
    [SerializeField] private UnitReference _unitReference;      // Unit Reference scriptable object
    [SerializeField] private List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();    // Sprite renderers references
    [SerializeField] private TMP_Text _powerValue;              // TextMeshPro component containing the power value string
    [SerializeField] private TMP_Text _hpValue;                 // TextMeshPro component containing the health point value string
    [SerializeField] private GameObject _rangeIcon;             // Range icon GameObject reference
    [SerializeField] private TMP_Text _rangeValue;              // Range value text reference
    [SerializeField] private GameObject _commandPointsIcon;     // Command points icon GameObject reference
    [SerializeField] private TMP_Text _commandPointsValue;      // Command points value text reference
    [SerializeField] private GameObject _activeFeedback;        // Active feedback game object
    [SerializeField] private GameObject _hurtFeedback;          // Hurt feedback game object
    [SerializeField] private GameObject _unitFactionFeedback;   // Unit faction feedback game object
    [SerializeField] private Color _colorFriendly;              // Friendly color reference
    [SerializeField] private Color _colorEnemy;                 // Enemy color reference
    private Tile _tile;                                         // Tile on which the unit is located, if any

    // Public get/set
    public int power { get => _power; }
    public int hp { get => _hp; }
    public int speed { get => _speed; }
    public int range { get => _range; }
    public int initiative { get => _initiative; }
    public int commandPoints { get => _commandPoints; }
    public Faction faction { get => _faction; }
    public Tile tile { get => _tile; set => _tile = value; }
    public bool stunned { get => _stunned; set => _stunned = value; }
    public UnitReference unitReference { get => _unitReference; }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init class variables
    /// </summary>
    public void Init()
    {
        // If the Unit Reference is initialized, load all unit variables from it
        if (_unitReference != null)
        {
            _power = _unitReference.power;
            _hp = _unitReference.hp;
            _speed = _unitReference.speed;
            _range = _unitReference.range;
            _initiative = _unitReference.initiative;
            _commandPoints = _unitReference.commandPoints;

            if (_faction == Faction.Friendly)
            {
                foreach(SpriteRenderer spriteRenderer in _spriteRenderers)
                {
                    spriteRenderer.sprite = _unitReference.friendlySprite;
                }
                _unitFactionFeedback.GetComponent<SpriteRenderer>().color = _colorFriendly;
            }
            else if (_faction == Faction.Enemy)
            {
                foreach(SpriteRenderer spriteRenderer in _spriteRenderers)
                {
                    spriteRenderer.sprite = _unitReference.enemySprite;
                    float currentScale = spriteRenderer.transform.localScale.x;
                    spriteRenderer.transform.localScale = new Vector3(-currentScale, currentScale, currentScale);
                }
                _unitFactionFeedback.GetComponent<SpriteRenderer>().color = _colorEnemy;
            }

            // Range icon visible only if the unit has a range greater than 1
            if (_range > 1)
            {
                _rangeIcon.SetActive(true);
                _rangeValue.text = _range.ToString();
            }

            _commandPointsIcon.SetActive(true);
            _commandPointsValue.text = _commandPoints.ToString();

            foreach (Trait trait in _unitReference.traits)
            {
                _traits.Add(trait);
            }

            _stunned = false;
        }

        UpdateStats();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Update all stats and their in-game visual
    /// </summary>
    public void UpdateStats()
    {
        _powerValue.text = _power.ToString();
        _hpValue.text = _hp.ToString();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Load the given Unit Reference and change this unit's variables to correspond to the new Unit Reference
    /// </summary>
    /// <param name="unitReference"> Target Unit Reference to load </param>
    public void LoadUnitReference(UnitReference unitReference, Faction faction)
    {
        _unitReference = unitReference;
        _faction = faction;
        Init();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Move this Unit to a given Tile
    /// </summary>
    /// <param name="tile"> Tile to which this unit will move </param>
    public void Move(Tile tile)
    {
        transform.position = tile.transform.position;
        _tile = tile;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Set tile reference to null
    /// </summary>
    public void ResetTile()
    {
        _tile = null;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns the damage dealt to a given target unit
    /// </summary>
    /// <param name="targetUnit"> Target unit which will be dealt damage </param>
    /// <returns> Damage dealt to targetUnit </param>
    public int GetDamage(Unit targetUnit)
    {
        // TODO: Change power depending on unit modificators, targetUnit type, etc...
        return _power;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when this unit receives damage
    /// </summary>
    /// <param name="damage"> How much damage this unit is dealt </param>
    public void TakeDamage(int damage)
    {
        // Minimum 0hp
        _hp = Mathf.Clamp(_hp - damage, 0, _hp);
        UpdateStats();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Check if this unit has been killed
    /// </summary>
    /// <returns> Whether or not this unit should die </param>
    public bool CheckDeath()
    {
        return _hp == 0;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Death behaviour when this unit dies
    /// </summary>
    public void Die()
    {
        // TODO: Change death behaviour (graveyard?)
        Board.instance.KillUnit(this);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when the unit has been clicked on
    /// </summary>
    public void OnMouseDown()
    {
        Select();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the unit
    /// </summary>
    public void Select()
    {
        Board.instance.SelectUnit(this);
        // Only if in planification mode
        if (GameManager.instance.GetPlanificationMode())
        {
            SelectFeedback(true);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Unselect the unit
    /// </summary>
    public void Unselect()
    {
        // TODO: Unselect behaviour
        SelectFeedback(false);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Selection feedback
    /// </summary>
    /// <param name="active"> Whether or not to activate the feedback </param>
    public void SelectFeedback(bool active)
    {
        _activeFeedback.SetActive(active);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Hurt feedback
    /// </summary>
    /// <param name="active"> Whether or not to activate the feedback </param>
    public void HurtFeedback(bool active)
    {
        _hurtFeedback.SetActive(active);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Whether or not the unit has the given trait
    /// </summary>
    /// <param name="checkTrait"> Trait to look for in the unit traits </param>
    /// <returns> True if the unit has checkTrait in _traits, false otherwise </param>
    public bool HasTrait(Trait checkTrait)
    {
        bool hasTrait = false;

        foreach(Trait trait in _traits)
        {
            if (checkTrait == trait)
            {
                hasTrait = true;
            }
        }

        return hasTrait;
    }
}
