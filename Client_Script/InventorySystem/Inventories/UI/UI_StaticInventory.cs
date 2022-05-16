using System.Collections;
using System.Collections.Generic;
using Kame.Game;
using Kame.Game.Data;
using Kame;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StaticInventory : UI_Inventory
{
    public UI_InventorySlot[] staticSlots = null;
    
    protected override void Redraw()
    {
        if (inventory == null)
        {
            Debug.Log("Static, Inventory Is null");
            return;
        }
        
        slotUIs.Clear();
        for (int i = 0; i < Slots.Length; ++i)
        {
            UI_InventorySlot slotUI = staticSlots[i];
            slotUI.Initialize(Slots[i]);
            Slots[i].UpdateSlot(Slots[i].Item, Slots[i].Amount);
            
            GameObject go = slotUI.gameObject;
            AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnterSlot(go); });
            AddEvent(go, EventTriggerType.PointerExit, delegate { OnExitSlot(go); });
            AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
            AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
            AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
            AddEvent(go, EventTriggerType.PointerClick, (data) => { OnClick(go, (PointerEventData) data); });

            slotUIs.Add(go, Slots[i]);
        }
    }

    protected override void OnRightClick(InventorySlot slot)
    {
        base.OnRightClick(slot);
    }
}
