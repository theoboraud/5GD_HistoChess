using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

/// <summary>
///     TESTING ONLY
///     The shop is used by the player to spawn units on the board
/// </summary>
public class EnemyShop : MonoBehaviour
{
    // Variables
    private const int NB_SHOP_ITEMS = 5;
    [SerializeField] private Faction _faction;

    // References
    public static EnemyShop instance;                                                                            // Static reference
    [SerializeField] private List<UnitReference> _shopItemReferences = new List<UnitReference>();           // Possible shop items
    private List<ShopItem> _shopItems = new List<ShopItem>();                                               // All shop items shown in the shop
    [SerializeField] private List<Transform> _shopZones = new List<Transform>();                            // Shop spawning zone where shop items spawn
    [SerializeField] private GameObject _unitPrefab;                                                        // Unit prefab
    [SerializeField] private GameObject _shopItemPrefab;                                                    // ShopItem prefab

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
        RollShop();
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
        EnemyReserve.instance.SpawnUnit(shopItem.unitReference);

        DeleteShopItem(shopItem);
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
    ///     Randomly changes the shop items available
    /// </summary>
    public void RollShop()
    {
        // Delete current shop items, if any
        for (int i = _shopItems.Count - 1; i >= 0; i--)
        {
            DeleteShopItem(_shopItems[i]);
        }
        // Spawn NB_SHOP_ITEMS randomly seleted from the _shopItemReferences list in the shop
        for (int i = 0; i < NB_SHOP_ITEMS; i++)
        {
            SpawnShopItem(_shopItemReferences[Random.Range(0, _shopItemReferences.Count)]);
        }
    }
}
