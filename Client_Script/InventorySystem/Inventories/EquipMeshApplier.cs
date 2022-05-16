using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kame;
using Kame.Game;
using Kame.Game.Data;
using UnityEngine;

public class EquipMeshApplier : MonoBehaviour
{
    //장비를 장착하지 않았을 때 기본적으로 장착되어야할 장비
    public EquipItemObject[] defaultItemObjects = new EquipItemObject[8];
    
    private EquipmentCombiner _combiner;
    private EquipedItemInstances[] _equippedItemInstances = new EquipedItemInstances[8];
   
    //Equiment PreUpdate에 적용
    public void OnRemoveItem(InventorySlot slot)
    {
        AddressableLoader.LoadAssetAsync<EquipItemObject>(slot.Item.Data.objectPath,
        itemObject =>
        {
            if (itemObject == null)
            {
                //DestroyDefaultItem;
                RemoveItemBy(slot.allowedItemTypes[0]);
                return;
            }
    
            if (itemObject.equippedPrefab != null)
            {
                RemoveItemBy(slot.allowedItemTypes[0]);
            }
        });
    }
    
    //Equipment의 PostUpdate에 적용 
    public void OnEquipItem(InventorySlot slot)
    {
        AddressableLoader.LoadAssetAsync<EquipItemObject>(slot.Item.Data.objectPath,
        result =>
        {
            EquipItemObject itemObject = result;
            if (itemObject == null)
            {
                EquipDefaultItemBy(slot.allowedItemTypes[0]);
                return;
            }

            ApplyMesh(slot.allowedItemTypes[0], itemObject);
        });
    }
    
    /// <summary>
    /// 착용아이템 Mesh적용
    /// </summary>
    /// <param name="type">착용하는 ItemType</param>
    /// <param name="itemObject">착용하는 ItemObject</param>
    private void ApplyMesh(ItemType type, EquipItemObject itemObject)
    {
        int index = GetIndexBy(type);
        switch(type)
        {
            case ItemType.Helmet:
            case ItemType.Chest:
            case ItemType.Pants:
            case ItemType.Boots:
            case ItemType.Gloves:
                _equippedItemInstances[index] = EquipSkinnedItem(itemObject);
                break;
            case ItemType.Shoulders:
            case ItemType.LeftWeapon:
            case ItemType.RightWeapon:
                _equippedItemInstances[index] = EquipMeshItem(itemObject);
                break;
        }
    }

    /// <summary>
    /// skinned아이템 장착
    /// </summary>
    /// <param name="itemObject"></param>
    /// <returns></returns>
    private EquipedItemInstances EquipSkinnedItem(EquipItemObject itemObject)
    {
        if (itemObject == null)
        {
            return null;
        }

        Transform itemTransform = _combiner.AddLimb(itemObject.equippedPrefab, itemObject.boneNames);

        EquipedItemInstances instance = new EquipedItemInstances();
        if (itemTransform != null)
        {
            instance.itemTransforms.Add(itemTransform);
            return instance;
        }

        return null;
    }

    /// <summary>
    /// static item장착
    /// </summary>
    /// <param name="itemObject"></param>
    /// <returns></returns>
    private EquipedItemInstances EquipMeshItem(EquipItemObject itemObject)
    {
        if (itemObject == null)
        {
            return null;
        }

        Transform[] itemTransforms = _combiner.AddMesh(itemObject.equippedPrefab);
        if (itemTransforms.Length > 0)
        {
            EquipedItemInstances instance = new EquipedItemInstances();
            instance.itemTransforms.AddRange(itemTransforms.ToList<Transform>());
            return instance;
        }

        return null;
    }

    private void EquipDefaultItemBy(ItemType type)
    {
        int index = GetIndexBy(type);

        EquipItemObject itemObject = defaultItemObjects[index];
        ApplyMesh(type, itemObject);
    }
    
    private void RemoveItemBy(ItemType type)
    {
        int index = GetIndexBy(type);
        if (_equippedItemInstances[index] != null)
        {
            _equippedItemInstances[index].OnDestroy();
            _equippedItemInstances[index] = null;
        }
    }

    private int GetIndexBy(ItemType type)
    {
        PlayerEquipment equipment = GetComponent<PlayerEquipment>();
        for (int i = 0; i < equipment.Slots.Length; ++i)
        {
            if (equipment.Slots[i].allowedItemTypes[0] == type)
                return i;
        }

        return -1;
    }
    
    private void OnDestroy()
    {
        foreach (EquipedItemInstances item in _equippedItemInstances)
        {
            if (item != null)
            {
                item.OnDestroy();
            }
        }
    }
}
