using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Enums;

/// <summary>
///     Shop item appearing in the shop for the player to buy
/// </summary>
public class ShopItem : MonoBehaviour, ISelectableEntity
{
    // References
    [SerializeField] private UnitReference _unitReference;          // Unit Reference
    [SerializeField] private TMP_Text _name;                        // Health point value text reference
    [SerializeField] private SpriteRenderer _spriteRenderer;        // Sprite renderer reference
    [SerializeField] private Faction _faction;                      // Faction reference
    [SerializeField] private TMP_Text _powerValue;                  // Power value text reference
    [SerializeField] private TMP_Text _hpValue;                     // Health point value text reference
    [SerializeField] private GameObject _commandPointsIcon;         // Command points icon GameObject reference
    [SerializeField] private TMP_Text _commandPointsValue;          // Command points value text reference
    [SerializeField] private List<SpriteRenderer> _traitSprites = new List<SpriteRenderer>();


    // Public get/set
    public UnitReference unitReference { get => _unitReference; }
    public Faction faction { get => _faction; }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init class variables
    /// </summary>
    private void Init()
    {
        _spriteRenderer.sprite = _faction == Faction.Friendly ? _unitReference.friendlySprite[Random.Range(0, _unitReference.friendlySprite.Count - 1)] : _unitReference.enemySprite[Random.Range(0, _unitReference.enemySprite.Count - 1)];
        _name.text = unitReference.unitName;
        _powerValue.text = _unitReference.power.ToString();
        _hpValue.text = _unitReference.hp.ToString();
        _commandPointsIcon.SetActive(true);
        _commandPointsValue.text = _unitReference.commandPoints.ToString();

        if (_faction == Faction.Enemy)
        {
            float currentScale = _spriteRenderer.transform.localScale.x;
            _spriteRenderer.transform.localScale = new Vector3(-currentScale, currentScale, currentScale);
        }

        LoadTraitSprites();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Load given shopItemReference and faction
    /// </summary>
    /// <param name="shopItemReference"> Reference of the item to load </param>
    /// <param name="faction"> Faction the shop item belongs to </param>
    public void LoadReference(UnitReference shopItemReference, Faction faction)
    {
        _unitReference = shopItemReference;
        _faction = faction;
        Init();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Called when the shop item has been clicked on
    /// </summary>
    public void OnMouseDown()
    {
        Select();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Select the shop item
    /// </summary>
    public void Select()
    {
        SoundManager.instance.ButtonPressed();

        if (_faction == Faction.Friendly)
        {
            Shop.instance.BuyShopItem(this);
        }
        else if (_faction == Faction.Enemy)
        {
            EnemyShop.instance.BuyShopItem(this);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Unselect the shop item
    /// </summary>
    public void Unselect()
    {
        // TODO: Unselect behaviour
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Delete the shop item from the shop
    /// </summary>
    public void DeleteFromShop()
    {
        Destroy(gameObject);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Load trait sprites depending on unit traits
    /// </summary>
    public void LoadTraitSprites()
    {
        int traitCount = 0;

        foreach(Trait trait in _unitReference.traits)
        {
            SpriteRenderer spriteRenderer = _traitSprites[traitCount];
            spriteRenderer.gameObject.SetActive(true);

            switch(trait)
            {
                case Trait.Weak:
                    spriteRenderer.sprite = Shop.instance.traitSpriteWeak;
                    break;
                case Trait.Barrage:
                    spriteRenderer.sprite = Shop.instance.traitSpriteBarrage;
                    break;
                case Trait.Charge:
                    spriteRenderer.sprite = Shop.instance.traitSpriteCharge;
                    break;
                case Trait.Distance:
                    spriteRenderer.sprite = Shop.instance.traitSpriteDistance;
                    break;
                case Trait.Enrage:
                    spriteRenderer.sprite = Shop.instance.traitSpriteEnrage;
                    break;
                case Trait.Cheap:
                    spriteRenderer.sprite = Shop.instance.traitSpriteCheap;
                    break;
                case Trait.Swarm:
                    spriteRenderer.sprite = Shop.instance.traitSpriteSwarm;
                    break;
                case Trait.Reload:
                    spriteRenderer.sprite = Shop.instance.traitSpriteReload;
                    break;
                case Trait.Spear:
                    spriteRenderer.sprite = Shop.instance.traitSpriteSpear;
                    break;
                case Trait.Support:
                    spriteRenderer.sprite = Shop.instance.traitSpriteSupport;
                    break;
                case Trait.Run:
                    spriteRenderer.sprite = Shop.instance.traitSpriteRun;
                    break;
                case Trait.Savage:
                    spriteRenderer.sprite = Shop.instance.traitSpriteSavage;
                    break;
                case Trait.Raid:
                    spriteRenderer.sprite = Shop.instance.traitSpriteRaid;
                    break;
                default:
                    break;
            }

            traitCount++;
        }
    }
}
