using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Define;
using UnityEngine;

public class Shopper : MonoBehaviour
{
    private Shop activeShop = null;
        
    private VoidEventChannelSO _activeShopChange;
    private void Awake()
    {
        _activeShopChange = EventChannelSO.Get<VoidEventChannelSO>(ResourcePath.ShopChange);
    }

    public void SetActiveShop(Shop shop)
    {
        if (activeShop != null)
        {
            activeShop.SetShopper(null);
        }
        activeShop = shop;

        if (activeShop != null)
        {
            activeShop.SetShopper(this);
        }
        if (_activeShopChange != null)
        {
            _activeShopChange.RaiseEvent();
        }
    }

    public Shop GetActiveShop()
    {
        return activeShop;
    }
}
