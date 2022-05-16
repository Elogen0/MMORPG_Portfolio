using System.Collections;
using System.Collections.Generic;
using Kame;
using Kame.Define;
using RPG.UI.Shopas;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class UI_Shop : UI_Base
    {
        [SerializeField] private AssetReference prefab;
        
        private Shop currentShop = null;

        private Color originalTotalTextColor;
        private TransformAnchor _playerTransformAnchor;
        private VoidEventChannelSO _activeShopChange;
        enum GameObjects
        {
            content_root,
            btn_confirm,
            btn_switch
        }

        enum Texts
        {
            tmp_shop_name,
            tmp_total
        }
        
        public override void Init()
        {
            _playerTransformAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor);
            _activeShopChange = EventChannelSO.Get<VoidEventChannelSO>(ResourcePath.ShopChange);
            Bind<GameObject>(typeof(GameObjects));
            Bind<TextMeshProUGUI>(typeof(Texts));
        }
        
        void Start()
        {
            originalTotalTextColor = GetText((int)Texts.tmp_total).color;

            _activeShopChange.OnEventRaised += ShopChanged;
            GetObject((int)GameObjects.btn_confirm).GetComponent<Button>().onClick.AddListener(ConfirmTransaction);
            GetObject((int)GameObjects.btn_switch).GetComponent<Button>().onClick.AddListener(SwitchMode);
            //ShopChanged();
        }

        private void ShopChanged()
        {
            Debug.Log("ShopChanged");
            if (currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }
            currentShop = _playerTransformAnchor.Value.GetComponent<Shopper>().GetActiveShop();
            //gameObject.SetActive(currentShop != null);
            if (currentShop != null)
                UI_ViewNavigation.Instance.Show(GetComponent<UI_View>().BindingTag);
            else
                UI_ViewNavigation.Instance.Home();
            Debug.Log($"Currentsp {currentShop != null}");

            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(currentShop);
            }
            
            if (currentShop == null) return;
            GetText((int)Texts.tmp_shop_name).text = currentShop.ShopName;

            currentShop.onChange += RefreshUI;
            
            RefreshUI();
        }

        private void RefreshUI()
        {
            StartCoroutine(CoRefreshUI());
        }

        private IEnumerator CoRefreshUI()
        {
            Transform root = GetObject((int) GameObjects.content_root).transform;
            root.RemoveAllChildren();
            var items = currentShop.GetFilteredItems();
            foreach (var item in items) 
            {
                var request = AddressableLoader.Instantiate(prefab, root);
                yield return request.Wait();
                var obj = request.Result;
                UI_ShopItem row = obj.GetComponent<UI_ShopItem>(); 
                row.Setup(currentShop, item);
            }
            
            GetText((int)Texts.tmp_total).text = $"Total : ${currentShop.TransactionTotal():N2}";
            GetText((int)Texts.tmp_total).color = currentShop.HasSufficientFunds() ? originalTotalTextColor : Color.red;
            GetObject((int)GameObjects.btn_confirm).GetComponent<Button>().interactable = currentShop.CanTransact();
            TextMeshProUGUI switchText = GetObject((int)GameObjects.btn_switch).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI confirmText = GetObject((int)GameObjects.btn_confirm).GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>();
            if (currentShop.IsBuyingMode)
            {
                switchText.text = "Switch to Selling";
                confirmText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch to Buying";
                confirmText.text = "Sell";
            }
            
            foreach (FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }
        }

        public void Close()
        {
            _playerTransformAnchor.Value.GetComponent<Shopper>().SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode);
        }

        
    }
}
