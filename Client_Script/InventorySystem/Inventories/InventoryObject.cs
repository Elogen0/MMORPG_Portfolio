using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Kame.Game;
using Kame.Game.Data;
using Kame.Items;
using UnityEngine;

namespace Kame
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Kame/InventorySystem/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        [SerializeField] protected Inventory container = new Inventory();
        public InventorySlot[] Slots => container.slots;
        public Inventory Container => container;
        public GameObject Owner { get; set; }

        public void Awake()
        {
            for (int i = 0; i < container.slots.Length; i++)
            {
                container.slots[i].Index = i;
                container.slots[i].parent = container;
            }
        }

        public void OnValidate()
        {
            for (int i = 0; i < container.slots.Length; i++)
            {
                container.slots[i].Index = i;
                container.slots[i].parent = container;
            }
        }


        // public event Action<InventoryObject> OnChangeContainer;
        // public virtual void SetContainer(Inventory container)
        // {
        //     this.container = container;
        //     OnChangeContainer?.Invoke(this);
        // }
        
        public void RegisterPreUpdate(Action<InventorySlot> preUpdate) => container.RegisterPreUpdate(preUpdate);
        
        public void DeRegisterPreUpdate(Action<InventorySlot> preUpdate) => container.DeRegisterPreUpdate(preUpdate);
        
        public void RegisterPostUpdate(Action<InventorySlot> postUpdate) => container.RegisterPostUpdate(postUpdate);
        
        public void DeRegisterPostUpdate(Action<InventorySlot> postUpdate) => container.DeRegisterPostUpdate(postUpdate);
        
        public bool AddItem(Item item) => container.AddItem(item);
        
        public bool PlaceItem(int index, Item item) => container.PlaceItem(index, item);
        
        public InventorySlot GetSlotByTemplateId(int templateId) => container.GetSlotByTemplateId(templateId);
        
        public InventorySlot GetSlotByDbId(int itemDbId) => container.GetSlotByDbId(itemDbId);
        
        public bool IsEmptySlot(int index) => container.IsEmptySlot(index);
        
        public InventorySlot GetEmptySlot() => container.GetEmptySlot();
        
        public InventorySlot Find(Func<InventorySlot, bool> condition) => container.Find(condition);

        public bool SwapItems(InventorySlot slotA, InventorySlot slotB)
        {
            if (slotA == slotB)
                return false;
            if (slotB.CanPlaceInSlot(slotA.Item) && slotA.CanPlaceInSlot(slotB.Item))
            {
                C_SwapSlot swapSlot = new C_SwapSlot();
                
                swapSlot.ItemA = slotA.Item.Info;
                swapSlot.ItemA.SlotIndex = slotB.Index;
                swapSlot.ItemA.InventoryType  = slotB.InventoryType;
                
                swapSlot.ItemB = slotB.Item.Info;
                swapSlot.ItemB.SlotIndex = slotA.Index;
                swapSlot.ItemB.InventoryType = slotA.InventoryType;
                NetworkManager.Instance.Send(swapSlot);
                return true;
            }
            return false;
        } 
        public void Clear() => container.Clear();
        public void UseItem(InventorySlot slotToUse)
        {
            if (slotToUse.IsEmptySlot() || slotToUse.Amount <= 0)
                return;
            
            AddressableLoader.LoadAssetAsync<ItemObject>(slotToUse.Item.Data.objectPath, itemObject =>
            {
                if (itemObject)
                    itemObject.Use(slotToUse.Item, Owner);
                //todo: 서버에 사용요청후 결과처리
            });
        }
    }
}
