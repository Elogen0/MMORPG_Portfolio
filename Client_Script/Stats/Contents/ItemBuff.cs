using System.Collections;
using System.Collections.Generic;

namespace Kame.Game.Data
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

        public ItemBuff(StatType statType, ModifierType modifierType, float value)
        {
            this.statType = statType;
            this.modifierType = modifierType;
            this.value = value;
        }
        
        public void AddValue(ref float value)
        {
            if (modifierType == ModifierType.Flat)
                value += this.value;
        }

        public void PercentAddValue(ref float value)
        {
            if (modifierType == ModifierType.PercentAdd)
                value += (this.value / 100);
        }

        public void PercentMultValue(ref float value)
        {
            if (modifierType == ModifierType.PercentMult)
                value *= (this.value / 100);
        }
    }   
}
