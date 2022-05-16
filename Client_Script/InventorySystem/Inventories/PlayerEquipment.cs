using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kame.Game;
using Kame.Game.Data;
using Kame;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public ItemType[] allowItemTypes;
    public InventorySlot[] Slots => equipment.Slots;
    public Inventory Container => equipment.Container;
    private InventoryObject equipment;

    private void OnEnable()
    {
        allowItemTypes = new ItemType[2];
        allowItemTypes[0] = ItemType.RightWeapon;
        allowItemTypes[1] = ItemType.Chest;
        InventoryObject equip = ResourceLoader.Load<InventoryObject>("ScriptableObjects/Equipment");
        SetContainer(equip);
    }

    private void OnDisable()
    {
        if (equipment != null)
        {
            equipment.DeRegisterPreUpdate(OnPreUpdate);
            equipment.DeRegisterPostUpdate(OnPostUpdate);
        }
    }

    protected void SetContainer(InventoryObject container)
    {
        if (equipment != null)
        {
            equipment.DeRegisterPreUpdate(OnPreUpdate);
            equipment.DeRegisterPostUpdate(OnPostUpdate);
        }
        this.equipment = container;
        equipment.Owner = gameObject;

        equipment.RegisterPreUpdate(OnPreUpdate);
        equipment.RegisterPostUpdate(OnPostUpdate);
        
        for (int i = 0; i < allowItemTypes.Length; i++)
        {
            equipment.Slots[i].allowedItemTypes = new ItemType[1];
            equipment.Slots[i].allowedItemTypes[0] = allowItemTypes[i];
        }
    }

    //UnEquip
    protected void OnPreUpdate(InventorySlot slot)
    {
        if (TryGetComponent(out EquipMeshApplier meshApplier))
        {
            meshApplier.OnRemoveItem(slot);   
        }
        RemoveStat(slot);
    }

    //Equip
    protected void OnPostUpdate(InventorySlot slot)
    {
        if (TryGetComponent(out EquipMeshApplier meshApplier))
        {
            GetComponent<EquipMeshApplier>()?.OnEquipItem(slot);    
        }
        AddStat(slot);
    }
    
    private void AddStat(InventorySlot slot)
    {
        List<ItemBuff> buffs = slot.Item.Data.buffs;
        if (gameObject.TryGetComponent(out BaseController controller))
        {
            foreach (var buff in buffs)
            {
                controller.Stat.AddModifier(buff.statType, buff);
            }    
        }
    }
    
    private void RemoveStat(InventorySlot slot)
    {
        List<ItemBuff> buffs = slot.Item.Data.buffs;
        if (gameObject.TryGetComponent(out BaseController controller))
        {
            foreach (var buff in buffs)
            {
                controller.Stat.RemoveModifier(buff.statType, buff);
            }    
        }
    }

    public void ReCalculateItemStat()
    {
        foreach (var slot in Container.slots)
        {
            if (!slot.IsEmptySlot())
            {
                RemoveStat(slot);
                AddStat(slot);
            }
        }
    }
    
    //todo : 서버에 Stat변경요청
}
