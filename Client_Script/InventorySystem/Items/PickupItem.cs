using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Game.Data;
using Kame.Items;
using UnityEngine;

namespace Kame
{
    //todo : attatch cursorInteractor
    public class PickupItem : MonoBehaviour
    {
        public Item item;
        public ItemObject obj;
        public float Amount => item.Amount;
        public Sprite Icon => obj.icon;
        public void Setup(Item item, ItemObject obj)
        {
            this.item = item;
            this.obj = obj;
        }
    }
}