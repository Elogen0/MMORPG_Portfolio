using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Kame.Game;
using Kame.Game.Data;
using Kame.Items;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Kame
{
    public class PlayerInventory : MonoBehaviour
    {
        private InventoryObject inventory;
        private BigInteger _money;
        public event Action<BigInteger> OnChangeMoney;
        public BigInteger Money
        {
            get => _money;
            set => _money = value;
        }

        public Inventory Container => inventory.Container;
        public InventorySlot[] Slots => Container.Slots;
        #region Method

        private void Awake()
        {
            InventoryObject inv = ResourceLoader.Load<InventoryObject>("ScriptableObjects/Inventory");
            inventory = inv;
            inventory.Owner = gameObject;
            _money += 1000000;
        }
        
        public bool AddItem(Item item)
        {
            bool changed = false;
            if (item.ItemType == ItemType.Money)
            {
                _money += item.Amount;
                OnChangeMoney?.Invoke(_money);
                return true;
            }

            if (item.ItemType == ItemType.Exp)
            {
                Debug.Log("Exp +" + item.Amount);
                //todo: ExpUP
                return true;
            }

            changed = inventory.AddItem(item);

            // if (changed)
            //     QuestManager.Instance.ProcessQuest(QuestType.AcquireItem, itemData.id, amount);
            return changed;
        }
        
        public void Clear() => inventory.Clear();
        public bool DropItem(InventorySlot slotToDrop, int amount = 1)
        {
            if (slotToDrop.IsEmptySlot() || slotToDrop.Amount <= 0)
                return false;

            Debug.Log("Drop Item");
            // if (TryGetComponent(out ItemDropper dropper))
            // {
            //     ItemObject itemObject = slotToDrop.ItemObject;
            //     if (dropper.DropItem(itemObject, amount))
            //     {
            //         slotToDrop.AddItem(itemObject.data, -amount);
            //         QuestManager.Instance.ProcessQuest(QuestType.AcquireItem, itemObject.data.id, -amount);
            //         return true;
            //     }
            // }
            return false;
        }

        #endregion
    }  
}

