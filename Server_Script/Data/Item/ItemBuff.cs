using System.Collections;
using System.Collections.Generic;

namespace InflearnServer.Game.Data
{
    [System.Serializable]
    public class ItemBuff : IModifier<float>
    {
        public StatType statType;
        public ModifierType modifierType;
        public float value;

        public ItemBuff(){}

        public ItemBuff(ItemBuff other)
        {
            this.statType = other.statType;
            this.modifierType = other.modifierType;
            this.value = other.value;
        }
        
        public void AddValue(ref float baseValue)
        {
            if (modifierType == ModifierType.Flat)
                baseValue += value;
        }

        public void PercentAddValue(ref float baseValue)
        {
            if (modifierType == ModifierType.PercentAdd)
                baseValue += value;
        }

        public void PercentMultValue(ref float baseValue)
        {
            if (modifierType == ModifierType.PercentMult)
                baseValue *= value;
        }
    }   
}
