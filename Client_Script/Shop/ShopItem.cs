using System.Collections;
using System.Collections.Generic;
using Kame.Game.Data;
using UnityEngine;

public class ShopItem
{
    public ItemData Data { get; set; }
    public int Availability { get; set; }
    public float Price { get; set; }
    public int QuantityInTransaction { get; set; }

    public ShopItem(ItemData itemData, int availability, float price, int quantityInTransaction)
    {
        this.Data = itemData;
        this.Availability = availability;
        this.Price = price;
        this.QuantityInTransaction = quantityInTransaction;
    }

    public string Name => Data.name;
    public ItemType ItemType => Data.type;
}
