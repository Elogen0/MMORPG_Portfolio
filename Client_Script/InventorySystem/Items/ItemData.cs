using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kame.Game.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ItemData : IManagedData
    {
        public int ID => id;
        [JsonProperty] public int id = -1;
        [JsonProperty] public string name;
        [JsonProperty] public ItemType type = ItemType.None;
        [JsonProperty] public ItemGrade grade = ItemGrade.Common;
        [JsonProperty] public List<ItemBuff> buffs = new List<ItemBuff>();
        [JsonProperty] public int price;
        [JsonProperty] public bool stackable = false;

        [JsonProperty] public string objectPath;
        public ItemData()
        {
        }

        public ItemData(ItemData other)
        {
            name = other.name;
            id = other.id;
            type = other.type;
            grade = other.grade;
            foreach (var buff in other.buffs)
            {
                this.buffs.Add(new ItemBuff(buff));
            }

            stackable = other.stackable;
        }
        
        public static ItemData NullItemData { get; } = new ItemData();
    }
}

