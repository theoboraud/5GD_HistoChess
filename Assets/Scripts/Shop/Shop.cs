using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     The shop is used by the player to spawn units on the board
/// </summary>
public class Shop : MonoBehaviour
{
    // Constants
    private const int NB_SHOP_ITEMS = 5;
    private const int ROLL_COST = 1;
    private const int BUY_COST = 3;

    // Variables
    [SerializeField] private Faction _faction;

    // References
    public static Shop instance;                                                                            // Static reference
    [SerializeField] private List<UnitReference> _shopItemReferences = new List<UnitReference>();           // Possible shop items
    [SerializeField] private List<UnitReference> _tierOneUnitReferences = new List<UnitReference>();        // Tier 1 units references
    [SerializeField] private List<UnitReference> _tierTwoUnitReferences = new List<UnitReference>();        // Tier 2 units references
    [SerializeField] private List<UnitReference> _tierThreeUnitReferences = new List<UnitReference>();      // Tier 3 units references
    [SerializeField] private List<UnitReference> _tierFourUnitReferences = new List<UnitReference>();       // Tier 4 units references
    [SerializeField] private List<UnitReference> _tierFiveUnitReferences = new List<UnitReference>();       // Tier 5 units references
    private List<ShopItem> _shopItems = new List<ShopItem>();                                               // All shop items shown in the shop
    [SerializeField] private List<Transform> _shopZones = new List<Transform>();                            // Shop spawning zone where shop items spawn
    [SerializeField] private GameObject _unitPrefab;                                                        // Unit prefab
    [SerializeField] private GameObject _shopItemPrefab;                                                    // ShopItem prefab

    // Trait sprites references
    [Header("Trait sprites")]
    public Sprite traitSpriteWeak;
    public Sprite traitSpriteBarrage;
    public Sprite traitSpriteCharge;
    public Sprite traitSpriteDistance;
    public Sprite traitSpriteEnrage;
    public Sprite traitSpriteCheap;
    public Sprite traitSpriteSwarm;
    public Sprite traitSpriteReload;
    public Sprite traitSpriteSpear;
    public Sprite traitSpriteSupport;
    public Sprite traitSpriteRun;
    public Sprite traitSpriteSavage;
    public Sprite traitSpriteRaid;

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
        // TODO: Add all variable to init and their init value
        UpdateShop();

    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Spawns a given shop item in the shop
    /// </summary>
    /// <param name="shopItemReference"> Reference of the item to spawn in the shop </param>
    public void SpawnShopItem(UnitReference shopItemReference)
    {
        // TODO: Spawn ShopItem from UnitReference
        GameObject _spawnedShopItemGO = Instantiate(_shopItemPrefab);
        _spawnedShopItemGO.transform.parent = _shopZones[_shopItems.Count];
        _spawnedShopItemGO.transform.position = _spawnedShopItemGO.transform.parent.position;

        ShopItem _spawnedShopItem = _spawnedShopItemGO.GetComponent<ShopItem>();
        _spawnedShopItem.LoadReference(shopItemReference, _faction);
        _shopItems.Add(_spawnedShopItem);
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     The player buys a shop item and spawns it on its board
    /// </summary>
    /// <param name="shopItem"> shopItem to buy </param>
    public void BuyShopItem(ShopItem shopItem)
    {
        int buyCost = BUY_COST;

        if (shopItem.unitReference.traits.Contains(Trait.Cheap))
        {
            buyCost--;
        }

        if (Player.instance.golds >= buyCost)
        {
            Player.instance.PayGolds(buyCost);
            SoundManager.instance.UnitBuying();
            
            Reserve.instance.SpawnUnit(shopItem.unitReference);

            if (shopItem.unitReference.traits.Contains(Trait.Swarm) && Reserve.instance.unitsCount < Reserve.instance.reserveZonesCount)
            {
                Reserve.instance.SpawnUnit(shopItem.unitReference);
            }

            DeleteShopItem(shopItem);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     The given shop item is deleted from the shop
    /// </summary>
    /// <param name="shopItem"> shopItem to delete from the shop </param>
    public void DeleteShopItem(ShopItem shopItem)
    {
        _shopItems.Remove(shopItem);
        shopItem.DeleteFromShop();
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Roll the shop if the player can pay for it
    /// </summary>
    public void RollShop()
    {
        SoundManager.instance.ShopReroll();

        if (Player.instance.golds >= ROLL_COST)
        {
            Player.instance.PayGolds(ROLL_COST);

            UpdateShop();
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Randomly changes the shop items available
    /// </summary>
    public void UpdateShop()
    {
        // Delete current shop items, if any
        for (int i = _shopItems.Count - 1; i >= 0; i--)
        {
            DeleteShopItem(_shopItems[i]);
        }

        // Create the list that will contain every possible items depending on shop Tier
        List<UnitReference> availableUnits = GetAvailableUnits();

        // Spawn NB_SHOP_ITEMS randomly seleted from the _shopItemReferences list in the shop
        for (int i = 0; i < NB_SHOP_ITEMS; i++)
        {
            SpawnShopItem(availableUnits[Random.Range(0, availableUnits.Count)]);
        }
    }

    // ----------------------------------------------------------------------------------------

    /// <summary>
    ///     Create a list of all available units depending on shop Tier
    /// </summary>
    /// <returns> List of available Unit References </returns>
    private List<UnitReference> GetAvailableUnits()
    {
        List<UnitReference> availableUnits = new List<UnitReference>();

        if (GameManager.instance.TierUnlocked(1))
        {
            foreach (UnitReference unitReference in _tierOneUnitReferences)
            {
                availableUnits.Add(unitReference);
            }
        }

        if (GameManager.instance.TierUnlocked(2))
        {
            foreach (UnitReference unitReference in _tierTwoUnitReferences)
            {
                availableUnits.Add(unitReference);
            }
        }

        if (GameManager.instance.TierUnlocked(3))
        {
            foreach (UnitReference unitReference in _tierThreeUnitReferences)
            {
                availableUnits.Add(unitReference);
            }
        }

        if (GameManager.instance.TierUnlocked(4))
        {
            foreach (UnitReference unitReference in _tierFourUnitReferences)
            {
                availableUnits.Add(unitReference);
            }
        }

        if (GameManager.instance.TierUnlocked(5))
        {
            foreach (UnitReference unitReference in _tierFiveUnitReferences)
            {
                availableUnits.Add(unitReference);
            }
        }

        return availableUnits;
    }
}
