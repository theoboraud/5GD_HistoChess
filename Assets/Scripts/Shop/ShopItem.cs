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
    [SerializeField] private SpriteRenderer _spriteRenderer;        // Sprite renderer reference
    [SerializeField] private Faction _faction;                      // Faction reference
    [SerializeField] private TMP_Text _powerValue;                  // Power value text reference
    [SerializeField] private TMP_Text _hpValue;                     // Health point value text reference


    // Public get/set
    public UnitReference unitReference { get => _unitReference; }
    public Faction faction { get => _faction; }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Init class variables
    /// </summary>
    private void Init()
    {
        _spriteRenderer.sprite = _faction == Faction.Friendly ? _unitReference.friendlySprite : _unitReference.enemySprite;
        _powerValue.text = _unitReference.power.ToString();
        _hpValue.text = _unitReference.hp.ToString();
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
    ///     Randomly changes the shop items available
    /// </summary>
    public void RollShop()
    {
        // TODO: Change all shop items randomly from a given list
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
        Shop.instance.BuyShopItem(this);
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
}
