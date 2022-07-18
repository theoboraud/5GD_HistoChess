using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;
using DG.Tweening;

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
    private int _movePointsUsed;
    private bool _hasAttacked;
    private bool _hasEnraged;
    private bool _hasToReload;
    private int _formationLevel;
    private bool _enableTooltip = false;
    [SerializeField] private float _timeToWaitTooltip = 0.5f;

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
    [SerializeField] private Color _colorHurtStat;              // Color to change to for resistance value when unit is hurt
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
    public int movePointsUsed { get => _movePointsUsed; set => _movePointsUsed = value; }
    public bool hasAttacked { get => _hasAttacked; set => _hasAttacked = value; }
    public bool hasEnraged { get => _hasEnraged; set => _hasEnraged = value; }
    public bool hasToReload { get => _hasToReload; set => _hasToReload = value; }
    public int formationLevel { get => _formationLevel; }
    public List<SpriteRenderer> spriteRenderers { get => _spriteRenderers; set => _spriteRenderers = value; }
    public GameObject commandPointsIcon { get => _commandPointsIcon; set => _commandPointsIcon = value; }
    public List<Trait> traits { get => _traits; }

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
            _initiative = _unitReference.initiative;
            _commandPoints = _unitReference.commandPoints;
            _speed = 1;
            _range = 1;

            if (_faction == Faction.Friendly)
            {
                foreach(SpriteRenderer spriteRenderer in _spriteRenderers)
                {
                    spriteRenderer.sprite = _unitReference.friendlySprite[Random.Range(0, _unitReference.friendlySprite.Count - 1)];
                }
                _unitFactionFeedback.GetComponent<SpriteRenderer>().color = _colorFriendly;
            }
            else if (_faction == Faction.Enemy)
            {
                foreach(SpriteRenderer spriteRenderer in _spriteRenderers)
                {
                    spriteRenderer.sprite = _unitReference.enemySprite[Random.Range(0, _unitReference.enemySprite.Count - 1)];
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

            if (HasTrait(Trait.Distance))
            {
                _range = 2;
            }

            if (HasTrait(Trait.Run))
            {
                _speed = 2;
            }

            _stunned = false;
            _movePointsUsed = 0;
            _hasAttacked = false;
            _hasEnraged = false;
            _hasToReload = false;
            _formationLevel = 1;
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
        _commandPointsValue.text = (_commandPoints - _formationLevel + 1).ToString();

        Color color = GameManager.instance.formationLevelOneStatColor;

        if (_formationLevel == 1)
        {
            _commandPointsValue.color = GameManager.instance.formationLevelOneStatColor;
            _powerValue.color = GameManager.instance.formationLevelOneStatColor;
            _hpValue.color = GameManager.instance.formationLevelOneStatColor;
        }

        if (_formationLevel > 1)
        {
            _commandPointsValue.color = GameManager.instance.formationLevelTwoStatColor;

            if (HasTrait(Trait.Support))
            {
                _powerValue.color = GameManager.instance.formationLevelTwoStatColor;
                _hpValue.color = GameManager.instance.formationLevelTwoStatColor;
            }
        }

        if (_formationLevel > 2)
        {
            _commandPointsValue.color = GameManager.instance.formationLevelThreeStatColor;
        }

        int maxHp = HasTrait(Trait.Support) ? _unitReference.hp + Mathf.Clamp(_formationLevel - 1, 0, 1) : _unitReference.hp;
        if (_hp < maxHp)
        {
            _hpValue.color = _colorHurtStat;
        }
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
        if (_tile != tile)
        {
            if (GameManager.instance.gameMode == GameMode.Battle)
            {
                if (HasTrait(Trait.Charge))
                {
                    transform.DOMove(tile.transform.position + new Vector3(0f, 0.5f - 0.075f * _tile.y, 0f), 0.7f);
                    foreach(SpriteRenderer sprite in _spriteRenderers)
                    {
                        sprite.transform.DOLocalJump(sprite.transform.localPosition, Random.Range(0.02f, 0.1f), 4, Random.Range(0.8f, 1.6f), false);
                    }
                }
                else
                {
                    transform.DOMove(tile.transform.position + new Vector3(0f, 0.35f - 0.05f * _tile.y, 0f), 0.9f);
                    foreach(SpriteRenderer sprite in _spriteRenderers)
                    {
                        sprite.transform.DOLocalJump(sprite.transform.localPosition, Random.Range(0.02f, 0.1f), 4, Random.Range(0.8f, 1.6f), false);
                    }
                }
            }
            else
            {
                transform.position = tile.transform.position;
            }
            _tile = tile;
            _movePointsUsed++;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Set tile reference to null
    /// </summary>
    public void ResetTile()
    {
        if (_tile != null)
        {
            _tile.ResetUnit();
            _tile = null;
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Returns the damage dealt to a given target unit
    /// </summary>
    /// <param name="targetUnit"> Target unit which will be dealt damage </param>
    /// <returns> Damage dealt to targetUnit </param>
    public int GetDamage(Unit targetUnit)
    {
        int damage = _power;
        // If the unit has charge and has moved this turn, it gains more power for this attack
        if (HasTrait(Trait.Charge) && _movePointsUsed > 0)
        {
            damage++;
        }
        if (HasTrait(Trait.Spear) && targetUnit.movePointsUsed > 0)
        {
            damage++;
        }

        return damage;
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when this unit receives damage
    /// </summary>
    /// <param name="damage"> How much damage this unit is dealt </param>
    public void TakeDamage(int damage)
    {
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
        if (active)
        {
            SoundManager.instance.UnitSelectionCAC(this);

            foreach(SpriteRenderer sprite in _spriteRenderers)
            {
                sprite.transform.DOLocalJump(sprite.transform.localPosition, Random.Range(0.04f, 0.08f), 2, Random.Range(0.6f, 0.8f), false);
            }
        }
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

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Changes the formation level of the unit
    /// </summary>
    /// <param name="formationLevel"> New formation level </param>
    public void SetFormationLevel(int formationLevel)
    {
        if (!HasTrait(Trait.Savage))
        {
            _formationLevel = formationLevel;

            if (HasTrait(Trait.Support))
            {
                if (formationLevel > 1)
                {
                    _power = unitReference.power + 1;
                    _hp = unitReference.hp + 1;
                }
                else
                {
                    _power = unitReference.power;
                    _hp = unitReference.hp;
                }
            }
        }

        UpdateStats();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Do not render stats of the unit on screen
    /// </summary>
    public void UnknownStats()
    {
        _powerValue.transform.parent.gameObject.SetActive(false);
        _hpValue.transform.parent.gameObject.SetActive(false);
        _commandPointsValue.transform.parent.gameObject.SetActive(false);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Attack animation
    /// </summary>
    /// <param name="targetUnit"> Unit targeted by the attack </param>
    public IEnumerator AnimationAttackMelee(Unit targetUnit)
    {
        List<Vector3> spritePositions = new List<Vector3>();
        for(int i = 0; i < _spriteRenderers.Count; i++)
        {
            SpriteRenderer sprite = _spriteRenderers[i];
            spritePositions.Add(sprite.transform.position);
            Vector3 spriteOffsetFromUnit = sprite.transform.position - this.transform.position;
            Vector3 positionOffsetFromTarget = targetUnit.transform.position - this.transform.position;
            Vector3 targetPosition = targetUnit.transform.position + spriteOffsetFromUnit;
            float randomTime = 0.1f;//Random.Range(0.1f, 0.12f);
            sprite.transform.DOJump(targetPosition - positionOffsetFromTarget * 0.25f, Random.Range(0.3f, 0.4f), 1, randomTime, false);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.9f);

        for(int i = 0; i < _spriteRenderers.Count; i++)
        {
            SpriteRenderer sprite = _spriteRenderers[i];
            float randomTime = 0.2f;//Random.Range(0.2f, 0.3f);
            //sprite.transform.DOLocalJump(sprite.transform.localPosition, Random.Range(0.04f, 0.08f), 4, randomTime / 4f, false);
            sprite.transform.DOMove(spritePositions[i], randomTime);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Start tooltip
    /// </summary>
    public void OnMouseEnter()
    {
        _enableTooltip = true;
        StartCoroutine("WaitBeforeTooltip");
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Wait before enabling the tooltip
    /// </summary>
    public IEnumerator WaitBeforeTooltip()
    {
        UnitTooltip.instance.InitUnit(_unitReference);

        yield return new WaitForSeconds(_timeToWaitTooltip);

        if (_enableTooltip)
        {
            UnitTooltip.instance.EnableTooltip(true);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Stop tooltip
    /// </summary>
    public void OnMouseExit()
    {
        _enableTooltip = false;

        UnitTooltip.instance.EnableTooltip(false);
    }
}
