using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Game;
using Kame.Game.Data;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private string shopName;
    [Range(0, 100)] [SerializeField] private float sellingPercentage = 80;
    [SerializeField] private StockItemConfig[] stockConfig;

    [System.Serializable]
    class StockItemConfig
    {
        public int itemId;
        public int initialStock;
        [Range(0, 100)] public float buyingDiscountPercentage;
        public int levelToUnlock;
    }

    private Shopper currentShopper;
    private Dictionary<ItemData, int> cart = new Dictionary<ItemData, int>();
    private Dictionary<ItemData, int> soldItem = new Dictionary<ItemData, int>();
    private bool isBuyingMode = true;
    private ItemType filter = ItemType.None;
    public event Action onChange;

    public string ShopName => shopName;

    public bool IsBuyingMode => isBuyingMode;

    public void StartShopping(GameObject shopper)
    {
        shopper.GetComponent<Shopper>().SetActiveShop(this);
    }
    
    public void SetShopper(Shopper shopper)
    {
        currentShopper = shopper;
    }

    public IEnumerable<ShopItem> GetFilteredItems()
    {
        foreach (ShopItem shopItem in GetAllItems())
        {
            ItemData itemData = shopItem.Data;
            if (filter == ItemType.None || shopItem.ItemType == filter)
            {
                yield return shopItem;
            }
        }
    }

    public IEnumerable<ShopItem> GetAllItems()
    {
        Dictionary<ItemData, float> prices = GetPrices();
        Dictionary<ItemData, int> availabilities = GetAvailabilities();
        foreach (var item in availabilities.Keys)
        {
            if (availabilities[item] <= 0)
                continue;

            float price = prices[item];
            int quantityInTransaction = 0;
            cart.TryGetValue(item, out quantityInTransaction);
            int availability = availabilities[item];
            yield return new ShopItem(item, availability, price, quantityInTransaction);
        }
    }

    public void SelectFilter(ItemType itemType)
    {
        filter = itemType;
        onChange?.Invoke();
    }

    public ItemType GetFilter() => filter;

    public void SelectMode(bool isBuying)
    {
        isBuyingMode = isBuying;
        onChange?.Invoke();
    }
    
    public bool CanTransact()
    {
        if (IsTransactionEmpty()) return false;
        if (!HasSufficientFunds()) return false;
        if (!HasInventorySpace()) return false;
        return true;
    }

    private bool IsTransactionEmpty()
    {
        return cart.Count == 0;
    }

    public bool HasSufficientFunds()
    {
        PlayerInventory inventory = currentShopper.GetComponent<PlayerInventory>();
        if (inventory == null) return false;
        return inventory.Money.CompareTo((int)TransactionTotal()) >= 0;
    }

    public bool HasInventorySpace()
    {
        PlayerInventory shopperInventory = currentShopper.GetComponent<PlayerInventory>();
        if (shopperInventory == null)
            return false;

        List<Item> flatItems = new List<Item>();
        foreach (ShopItem shopItem in GetAllItems())
        {
            ItemData itemData = shopItem.Data;
            int quantity = shopItem.QuantityInTransaction;
            for (int i = 0; i < quantity; i++)
            {
                flatItems.Add(new Item(itemData));
            }
        }

        return shopperInventory.Container.HasSpaceFor(flatItems);
    }

    

    public void ConfirmTransaction()
    {
        PlayerInventory shopperInventory = currentShopper.GetComponent<PlayerInventory>();
        if (shopperInventory == null)
            return;
        foreach (ShopItem shopItem in GetAllItems())
        {
            ItemData itemData = shopItem.Data;
            int quantity = shopItem.QuantityInTransaction;
            float price = shopItem.Price;
            for (int i = 0; i < quantity; i++)
            {
                if (isBuyingMode)
                {
                    BuyItem(shopperInventory, itemData, price);
                }
                else
                {
                    SellItem(shopperInventory, itemData, price);
                }
            }
        }
        onChange?.Invoke();
    }
    
    public float TransactionTotal()
    {
        float total = 0;
        foreach (ShopItem shopItem in GetAllItems())
        {
            total += shopItem.Price * shopItem.QuantityInTransaction;
        }
        return total;
    }

    public void AddToCart(ItemData item, int quantity, bool refresh = true)
    {
        if (!cart.ContainsKey(item))
        {
            cart.Add(item, 0);
        }

        var availabilities = GetAvailabilities();
        int availability = availabilities[item];
        if (cart[item] + quantity > availability)
        {
            cart[item] = availability;
        }
        else
        {
            cart[item] += quantity;
        }

        if (cart[item] <= 0)
        {
            cart.Remove(item);
        }
        
        if (refresh)
            onChange?.Invoke();
    }

    public void SellItem(PlayerInventory shopperInventory, ItemData item, float price)
    {
        if (!shopperInventory.Container.Exist(item.id))
            return;
        
        AddToCart(item, -1);
        shopperInventory.Container.AddItem(item, -1);
        if (!soldItem.ContainsKey(item))
        {
            soldItem[item] = 0;
        }

        soldItem[item]--;
        shopperInventory.Money += (int)price;
    }

    public void BuyItem(PlayerInventory shopperInventory, ItemData item, float price)
    {
        if (shopperInventory.Money.CompareTo((int)price) < 0) return;
        bool success = shopperInventory.AddItem(new Item(item) {Amount = 1});
        if (success)
        {
            AddToCart(item, -1, refresh: false);
            if (!soldItem.ContainsKey(item))
            {
                soldItem[item] = 0;
            }
            soldItem[item]++;
            shopperInventory.Money -= (int)price;
        }
    }

    private Dictionary<ItemData, int> GetAvailabilities()
    {
        Dictionary<ItemData, int> availabilities = new Dictionary<ItemData, int>();
        foreach (var config in GetAvailableConfigs())
        {
            ItemData itemData = DataManager.ItemDict[config.itemId];
            if (itemData == null) continue;
            if (!availabilities.ContainsKey(itemData))
            {
                int sold = 0;
                soldItem.TryGetValue(itemData, out sold);
                availabilities[itemData] = -sold;
            }
            availabilities[itemData] += config.initialStock;
        }

        return availabilities;
    }


    private Dictionary<ItemData, float> GetPrices()
    {
        Dictionary<ItemData, float> prices = new Dictionary<ItemData, float>();
        foreach (var config in GetAvailableConfigs())
        {
            ItemData itemData = DataManager.ItemDict[config.itemId];
            if (itemData == null) continue;
            if (!prices.ContainsKey(itemData))
            {
                prices[itemData] = itemData.price;
            }

            prices[itemData] *= (1 - config.buyingDiscountPercentage / 100);
        }

        return prices;
    }
    
    private IEnumerable<StockItemConfig> GetAvailableConfigs()
    {
        int shopperLevel = GetShopperLevel();
        foreach (var config in stockConfig)
        {
            if (config.levelToUnlock > shopperLevel)
                continue;
            yield return config;
        }
    }

    private int GetShopperLevel()
    {
        if (currentShopper.TryGetComponent(out Health health))
        {
            return health.Stat.level;
        }
        return 0;
    }

    public float Distance { get; }
    public void Interact(GameObject other)
    {
        StartShopping(other);
    }

    public void StopInteract(GameObject other)
    {
    }
}