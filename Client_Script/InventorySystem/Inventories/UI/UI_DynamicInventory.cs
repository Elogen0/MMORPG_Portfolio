using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Game;
using Kame.Game.Data;
using Kame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public class UI_DynamicInventory : UI_Inventory
{
    [SerializeField] private AssetReference slotPrefab = null;
    
    protected override void Redraw()
    {
        if (inventory == null)
        {
            Debug.Log("Dynamic, Inventory Is null");
            return;
        }
        
        transform.RemoveAllChildren();
        //collapse slots
        // foreach (Transform child in transform)
        // {
        //     if (transform.TryGetComponent(out UI_InventorySlot slot))
        //     {
        //         slot.Clear();   
        //     }
        //     AddressableLoader.ReleaseInstance(child.gameObject);
        // }

        //rebuild slots
        slotUIs.Clear();
        foreach (var slot in Slots)
        {
            StartCoroutine(CoCreateSlotUI(slot));
        }
    }
    IEnumerator CoCreateSlotUI(InventorySlot slot)
    {
         var request = AddressableLoader.InstantiatePooling(slotPrefab, transform);
         yield return request.Wait();
        GameObject go = request.Result;
        UI_InventorySlot slotUI = go.GetComponent<UI_InventorySlot>();
        slotUI.Initialize(slot);
        slot.UpdateSlot(slot.Item, slot.Amount);

        GameObject obj = slotUI.gameObject;
        AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnterSlot(obj); });
        AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExitSlot(obj); });
        AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnStartDrag(obj); });
        AddEvent(obj, EventTriggerType.EndDrag, delegate { OnEndDrag(obj); });
        AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
        AddEvent(obj, EventTriggerType.PointerClick, (data) => { OnClick(obj, (PointerEventData) data); });

        slotUIs.Add(obj, slot);
        yield return null;
    }
}
