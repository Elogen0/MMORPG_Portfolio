using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.Game.Data;
using Newtonsoft.Json;

namespace Kame.Game
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class InventorySlot
    {
        #region vaiables
        public ItemType[] allowedItemTypes = new ItemType[0];
        [NonSerialized] public Inventory parent;
        public Item Item { get; private set; } = new Item();
        public int Index { get; set; } = -1;
        public bool Reserved { get; set; } = false; //for Net DB
        
        #endregion
        public int ItemDbId
        {
            get => Item.ItemDbId;
            set => Item.ItemDbId = value;
        }
        public int TemplateId
        {
            get => Item.TemplateId;
            set => Item.TemplateId = value;
        }
        public int Amount
        {
            get => Item.Amount;
            set => Item.Amount = value;
        }

        public int InventoryType
        {
            get => (int)parent.type;
        }

        public event Action<InventorySlot> OnPreUpdate;
        public event Action<InventorySlot> OnPostUpdate;

        public InventorySlot()
        {
        }

        public InventorySlot(Item item, int amount)
        {
            UpdateSlot(item, amount);
        }

        public InventorySlot(InventorySlot other)
        {
            allowedItemTypes = new ItemType[other.allowedItemTypes.Length];
            for (int i = 0; i < allowedItemTypes.Length; i++)
            {
                this.allowedItemTypes[i] = other.allowedItemTypes[i];
            }
            UpdateSlot(new Item(other.Item.Data), other.Amount);
        }

        public void UpdateSlot(Item item, int amount)
        {
            OnPreUpdate?.Invoke(this);
            if (amount <= 0 || item == null)
            {
                this.Item = new Item();
            }
            else
            {
                this.Item = item;
            }
            this.Amount = amount;
            Item.SlotIndex = Index;
            Item.InventoryType = (int)InventoryType;
            OnPostUpdate?.Invoke(this);
        }

        public bool AddItem(Item item)
        {
            if (item == null || item.TemplateId < 0)
                RemoveItem();
            if (item.Amount == 0)
                return false;
            if (item.Amount < 0 && item.TemplateId != Item.TemplateId)
                return false;
            if (item.Amount > 0)
            {
                if (!CanPlaceInSlot(item))
                {
                    return false;
                }
            }

            if (item.TemplateId == TemplateId && Item.Stackable)
            {
                return AddAmount(item.Amount);
            }
            else
            {
                UpdateSlot(item, item.Amount);
            }

            return true;
        }

        public void RemoveItem() => UpdateSlot(null, 0);

        public bool AddAmount(int value)
        {
            if (value == 0)
                return false;
            UpdateSlot(Item, Amount + value);
            return true;
        }

        public bool CanPlaceInSlot(Item item)
        {
            //넣고자 하는 아이템이 빈아이템이면 무조건 OK
            if (allowedItemTypes.Length <= 0 || item == null || item.TemplateId < 0)
            {
                return true;
            }

            //allowed 타입검사
            foreach (ItemType itemType in allowedItemTypes)
            {
                if (item.ItemType == itemType)
                    return true;
            }

            return false;
        }

        public bool IsEmptySlot()
        {
            if (Reserved)
                return false;
            if (Item == null || TemplateId < 0 || Amount <= 0)
                return true;

            return false;
        }
    }
}
