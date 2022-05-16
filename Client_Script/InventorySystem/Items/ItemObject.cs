using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Game.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kame.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Kame/InventorySystem/Item", order = 0)]
    public class ItemObject : ScriptableObject
    {
        [TextArea(5, 20)] public string description;
        public Sprite icon = null;
        public AssetReference droppedItem;
        
        public IEnumerator SpawnPickup(Item item, Vector3 position, Action<PickupItem> complete)
        {
            var request = AddressableLoader.InstantiatePooling(this.droppedItem, null);
            yield return request.Wait();
            GameObject go = request.Result;
            PickupItem pickup = go.GetOrAddComponent<PickupItem>();
        
            //todo : ResourceLoader를 써서 레어도에따라 이펙트생성 
            //DataManager.Item.AttachRarityEffect(data.grade, pickup.transform);
            pickup.transform.position = position;
            pickup.Setup(item, this);
            complete?.Invoke(pickup);
        }
        
        public virtual void Use(Item item, GameObject User)
        {
            
        }
    }
}