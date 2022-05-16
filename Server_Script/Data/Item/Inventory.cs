using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;

public enum InventoryType
{
    None = -1,
    Inventory = 0,
    Equipment = 1,
    QuickSlot = 2,
    Box = 3,
}

namespace InflearnServer.Game
{
    [System.Serializable]
    public class Inventory
    {
        public int id;
        public InventoryType type;
        public InventorySlot[] slots;
        public InventorySlot[] Slots => slots;
        public Inventory()
        {
            slots = new InventorySlot[0];
        }

        public Inventory(int count)
        {
            slots = new InventorySlot[count];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot();
                slots[i].Index = i;
                slots[i].parent = this;
            }
        }

        public Inventory(Inventory other)
        {
            this.slots = new InventorySlot[other.slots.Length];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot(other.slots[i]);
                slots[i].Index = i;
                slots[i].parent = this;
            }
        }

        public bool AddItem(Item item)
        {
            InventorySlot slot = GetSlotByTemplateId(item.TemplateId);
            if (slot == null || item.Stackable == false)
            {
                InventorySlot emptySlot = GetEmptySlot();
                if (emptySlot == null)
                    return false;

                emptySlot.AddItem(item);
            }
            else
            {
                slot.AddItem(item);
            }

            return true;
        }

        public bool PlaceItem(int index, Item item)
        {
            if (index < 0 || index >= slots.Length)
                return false;
            if (item == null || item.TemplateId < 0)
                slots[index].RemoveItem();
            else
                slots[index].UpdateSlot(item, item.Amount);
            return true;
        }

        public InventorySlot GetSlotByTemplateId(int templateId)
        {
            return Array.Find(slots, slot => slot.TemplateId == templateId);
        }

        public InventorySlot GetSlotByDbId(int itemDbId)
        {
            return Array.Find(slots, slot => slot.ItemDbId == itemDbId);
        }

        public bool IsEmptySlot(int index)
        {
            if (index < 0 || index >= slots.Length) return false;
            if (slots[index].IsEmptySlot()) return true;
            return false;
        }

        public InventorySlot GetEmptySlot()
        {
            return Array.Find(slots, i => i.IsEmptySlot());
        }

        public bool HasEmptySlot() { return Array.Exists(slots, slot => slot.IsEmptySlot()); }

        public InventorySlot Find(Func<InventorySlot, bool> condition)
        {
            foreach (var slot in slots)
            {
                if (condition.Invoke(slot))
                    return slot;
            }

            return null;
        }

        public bool SwapItems(InventorySlot slotA, InventorySlot slotB)
        {
            if (slotA == slotB)
                return false;

            if (slotB.CanPlaceInSlot(slotA.Item) && slotA.CanPlaceInSlot(slotB.Item))
            {
                InventorySlot temp = new InventorySlot(slotB.Item, slotB.Amount);
                slotB.UpdateSlot(slotA.Item, slotA.Amount);
                slotA.UpdateSlot(temp.Item, temp.Amount);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            foreach (InventorySlot slot in slots)
            {
                slot.UpdateSlot(null, 0);
            }
        }

        public void RegisterPreUpdate(Action<InventorySlot> onPreUpdate)
        {
            foreach (var slot in slots)
            {
                slot.OnPreUpdate += onPreUpdate;
            }
        }

        public void DeRegisterPreUpdate(Action<InventorySlot> onPreUpdate)
        {
            foreach (var slot in slots)
            {
                slot.OnPreUpdate -= onPreUpdate;
            }
        }

        public void RegisterPostUpdate(Action<InventorySlot> onPostUpdate)
        {
            foreach (var slot in slots)
            {
                slot.OnPostUpdate += onPostUpdate;
            }
        }

        public void DeRegisterPostUpdate(Action<InventorySlot> onPostUpdate)
        {
            foreach (var slot in slots)
            {
                slot.OnPostUpdate -= onPostUpdate;
            }
        }

        public int GetEmptySlotCount()
        {
            int i = 0;
            foreach (var slot in slots)
            {
                if (slot.IsEmptySlot())
                    ++i;
            }
            return i;
        }

        public bool HasItem(Item item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmptySlot()) continue;
                if (item.Data.id == slots[i].Item.Data.id)
                    return true;
            }
            return false;
        }

        public bool HasSpaceFor(IEnumerable<Item> items)
        {
            int emptySlotCount = GetEmptySlotCount();
            List<int> stackedItemIds = new List<int>();
            foreach (var item in items)
            {
                if (item.Data.stackable)
                {
                    if (HasItem(item))
                        continue;
                    if (stackedItemIds.Contains(item.Data.id))
                        continue;
                    stackedItemIds.Add(item.Data.id);
                }

                if (emptySlotCount <= 0) return false;
                --emptySlotCount;
            }
            return true;
        }

        public InventorySlot GetSuitableSlot(Item item)
        {
            InventorySlot slot = null;
            if (item.Stackable)
            {
                slot = GetSlotByTemplateId(item.TemplateId);
                if (slot == null)
                    slot = GetEmptySlot();
                
            }
            else
            {
                return GetEmptySlot();
            }
            return slot;
        }


        public bool TryGetSlotsFor(IEnumerable<Item> items, out List<InventorySlot> returnSlots)
        {
            int emptySlotCount = GetEmptySlotCount();
            returnSlots = new List<InventorySlot>();
            foreach (var item in items)
            {
                if (item.Data.stackable)
                {
                    InventorySlot slot = GetSlotByTemplateId(item.Data.id);
                    if (slot != null)
                    {
                        slot.Reserved = true;
                        returnSlots.Add(slot);
                        continue;
                    }
                }

                if (emptySlotCount <= 0)
                {
                    foreach (var slot in returnSlots)
                    {
                        slot.Reserved = false;
                    }
                    returnSlots.Clear();
                    return false;
                }
                
                InventorySlot emptySlot = GetEmptySlot();
                if (emptySlot == null)
                {
                    foreach (var slot in returnSlots)
                    {
                        slot.Reserved = false;
                    }
                    returnSlots.Clear();
                    return false;
                }
                emptySlot.Reserved = true;
                returnSlots.Add(emptySlot);
                --emptySlotCount;
            }
            return true;
        }
    }
}

