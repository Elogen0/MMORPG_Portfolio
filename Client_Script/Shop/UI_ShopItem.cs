using System;
using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Items;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace RPG.UI.Shopas
{
    public class UI_ShopItem : UI_Base
    {
        private Shop currentShop = null;
        private ShopItem item = null;
        private AsyncOperationHandle _handle;

        enum Img
        {
            img_icon,
        }

        enum Tmp
        {
            tmp_name,
            tmp_availability,
            tmp_price,
            tmp_quantity,
        }

        enum Buttons
        {
            btn_minus,
            btn_plus
        }
        
        public override void Init()
        {
            Bind<Image>(typeof(Img));
            Bind<TextMeshProUGUI>(typeof(Tmp));
            Bind<Button>(typeof(Buttons));
            GetButton((int)Buttons.btn_minus).onClick.AddListener(Remove);
            GetButton((int)Buttons.btn_plus).onClick.AddListener(Add);
        }
        
        public void Setup(Shop currentShop, ShopItem item)
        {
            this.currentShop = currentShop;
            this.item = item;
            Addressables.LoadAssetAsync<ItemObject>(item.Data.objectPath).Completed += handle =>
            {
                _handle = handle;
                ItemObject obj = handle.Result;
                GetImage((int)Img.img_icon).sprite = obj.icon;
            }; 
            GetText((int)Tmp.tmp_name).text = item.Name;
            GetText((int)Tmp.tmp_availability).text = $"{item.Availability.ToString()}";
            GetText((int)Tmp.tmp_price).text = $"${item.Price:N2}";
            GetText((int)Tmp.tmp_quantity).text = $"{item.QuantityInTransaction}";
        }
        
        
        public void Add()
        {
            currentShop.AddToCart(item.Data, 1);
        }

        public void Remove()
        {
            currentShop.AddToCart(item.Data, -1);
        }

        private void OnDisable()
        {
            AddressableLoader.Release(_handle);
        }

        
    }
}
