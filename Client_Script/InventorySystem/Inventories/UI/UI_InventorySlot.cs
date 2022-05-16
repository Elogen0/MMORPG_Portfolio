using System;
using Kame.Game;
using Kame.Game.Data;
using Kame.Items;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Kame
{
    public class UI_InventorySlot : UI_Base
    {
        enum GameObjects
        {
            tmp_slot_amount,
            img_icon,
            img_cooldown,
            tmp_cooldown
        }
        
        #region Variables
        private InventorySlot _slot;
        private TextMeshProUGUI _amount;
        private Image _icon;
        private Image _cooldownImg;
        private TextMeshProUGUI _cooldownText;
        private ItemObject _itemObject;
        private AsyncOperationHandle _handle;
        #endregion

        public override void Init()
        {
            Bind<GameObject>(typeof(GameObjects));
            _amount = GetObject((int)GameObjects.tmp_slot_amount).GetComponent<TextMeshProUGUI>();
            _icon = GetObject((int)GameObjects.img_icon).GetComponent<Image>();
            _cooldownImg = GetObject((int)GameObjects.img_cooldown).GetComponent<Image>();
            _cooldownText = GetObject((int)GameObjects.tmp_cooldown).GetComponent<TextMeshProUGUI>();
            _cooldownImg.gameObject.SetActive(false);
            _cooldownText.gameObject.SetActive(false);
        }

        public void Initialize(InventorySlot slot)
        {
            _slot = slot;
            RegisterEvent();
        }

        private void OnDisable()
        {
            UnRegisterEvent();
        }

        public void RegisterEvent()
        {
            if (_slot != null)
            {
                _slot.OnPostUpdate += OnPostUpdate;
            }
        }

        public void UnRegisterEvent()
        {
            if (_slot != null)
            {
                _slot.OnPostUpdate -= OnPostUpdate;
            }
        }

        public void OnPreUpdate(InventorySlot slot)
        {
            Clear();
        }
        
        public void OnPostUpdate(InventorySlot slot)
        {
            Redraw();
        }
        
        public void Redraw()
        {
            AddressableLoader.Release(_handle);
            if (_icon == null || _amount == null)
                return;
            if (_slot.Item.TemplateId < 0)
            {
                _icon.sprite = null;
                _icon.color = new Color(1, 1, 1, 0);
                _amount.text = string.Empty;
            }
            else
            {
                Addressables.LoadAssetAsync<ItemObject>(_slot.Item.Data.objectPath).Completed += handle =>
                {
                    _handle = handle;
                    _itemObject = handle.Result;
                    _icon.sprite = _itemObject.icon;
                    _icon.color = new Color(1, 1, 1, 1);
                    _amount.text = _slot.Amount == 1 ? string.Empty : _slot.Amount.ToString("n0");
                };
            }
        }

        public void Clear()
        {
            if (_icon == null || _amount == null)
                return;
            _amount.text = string.Empty;
            _icon.sprite = null;
            _icon.color = new Color(1, 1, 1, 0);
            _slot = null;
        }
    }
}