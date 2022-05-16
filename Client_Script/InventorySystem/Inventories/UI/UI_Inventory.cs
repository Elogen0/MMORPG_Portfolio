using System;
using System.Collections.Generic;
using System.Linq;
using Kame.Game;
using Kame.Game.Data;
using Kame.Items;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Kame
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class UI_Inventory : MonoBehaviour
    {
        #region Variables
        public InventoryObject inventory = null;
        //Key : InventorySlotUI, Value : InventorySlot
        public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();
        #endregion

        #region Properties
        public InventorySlot[] Slots => inventory?.Slots;
        #endregion

        protected virtual void Start()
        {
            AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInventory(gameObject); });
            AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInventory(gameObject); });
        }

        private void OnEnable()
        {
            Redraw();
        }

        private void OnDestroy()
        {
            foreach (var go in slotUIs.Keys)
            {
                go.GetComponent<UI_InventorySlot>()?.UnRegisterEvent();
            }
        }

        public void Setup(InventoryObject inventory)
        {
            if (!inventory)
                return;
            if (this.inventory == inventory)
            {
                // Redraw();
                return;
            }
            
            if (this.inventory != null)
            {
                foreach (var go in slotUIs.Keys)
                {
                    go.GetComponent<UI_InventorySlot>()?.UnRegisterEvent();
                }

                // this.inventory.OnChangeContainer -= Setup;
            }
            this.inventory = inventory;
            // this.inventory.OnChangeContainer += Setup;
            // Redraw();
        }

        protected abstract void Redraw();

        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetOrAddComponent<EventTrigger>();

            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.RemoveAllListeners();
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }
        
        public void OnEnterInventory(GameObject go)
        {
            MouseData.interfaceMouseIsOver = go.GetComponent<UI_Inventory>();
        }
        public void OnExitInventory(GameObject go)
        {
            MouseData.interfaceMouseIsOver = null;
        }

        public void OnEnterSlot(GameObject go)
        {
            MouseData.slotHoveredOver = go;
        }

        public void OnExitSlot(GameObject go)
        {
            MouseData.slotHoveredOver = null;
        }

        public void OnStartDrag(GameObject go)
        {
            MouseData.tempItemBeingDragged = CreateDragImage(go);
        }
        
        public void OnDrag(GameObject go)
        {
            if (MouseData.tempItemBeingDragged == null)
            {
                return;
            }
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
        
        public void OnEndDrag(GameObject go)
        {
            Destroy(MouseData.tempItemBeingDragged);

            if (MouseData.interfaceMouseIsOver == null)
            {
                inventory.Owner.GetComponent<PlayerInventory>()?.DropItem(slotUIs[go]);
                //slotUIs[go].RemoveItem();
            }
            else if (MouseData.slotHoveredOver)
            {
                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];
                inventory.SwapItems(slotUIs[go], mouseHoverSlotData);
            }
        }

        private GameObject CreateDragImage(GameObject go)
        {
            if (slotUIs[go].Item.TemplateId < 0)
            {
                return null;
            }

            GameObject dragImage = new GameObject();

            RectTransform rectTransform = dragImage.GetOrAddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);

            dragImage.GetOrAddComponent<Canvas>().sortingOrder = 200;
            
            dragImage.transform.SetParent(transform.parent);
            Image image = dragImage.AddComponent<Image>();
            AddressableLoader.LoadAssetAsync<ItemObject>(slotUIs[go].Item.Data.objectPath, obj =>
            {
                image.sprite = obj.icon;
                image.raycastTarget = false;

                dragImage.name = "Drag Image";
            });
            return dragImage;
        }
        
        public void OnClick(GameObject go, PointerEventData data)
        {
            InventorySlot slot = slotUIs[go];
            if (slot == null)
            {
                return;
            }

            if (data.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick(slot);
            }
            else if (data.button == PointerEventData.InputButton.Right)
            {
                OnRightClick(slot);
            }
        }

        protected virtual void OnRightClick(InventorySlot slot)
        {
            inventory.UseItem(slot);
        }

        protected virtual void OnLeftClick(InventorySlot slot)
        {
        }

        
    }
}