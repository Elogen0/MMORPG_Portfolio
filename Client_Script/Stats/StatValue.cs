using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Kame.Game.Data
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class StatValue
    {
        [JsonProperty] public StatType type;
        [JsonProperty] public ModifiableFloat value;
        private event Action<StatValue> OnChangedValue;

        public StatValue()
        {
            value = new ModifiableFloat();
        }

        public StatValue(StatType type, float value)
        {
            this.type = type;
            this.value = new ModifiableFloat(value);
        }

        public StatValue(StatValue other)
        {
            this.value = new ModifiableFloat(other.value);
            this.type = other.type;
        }
        
        public void RegisterModEvent(Action<StatValue> action)
        {
            value.RegisterModEvent(ChangedValue);
            if (action != null)
                OnChangedValue += action;
        }
        
        public void UnRegisterModEvent(Action<StatValue> action)
        {
            value.UnRegisterModEvent(ChangedValue);
            if (action != null)
                OnChangedValue -= action;
        }

        private void ChangedValue(ModifiableBase<float> modifiable)
        {
            OnChangedValue?.Invoke(this);
        }

        // #region IModifier
        // public void AddValue(ref float baseValue)
        // {
        //     //if (modifierType == ModifierType.Flat)
        //         baseValue += value.ModifiedValue;
        // }
        //
        // public void PercentAddValue(ref float baseValue)
        // {
        //     //if (modifierType == ModifierType.PercentAdd)
        //         baseValue += value.ModifiedValue;
        // }
        //
        // public void PercentMultValue(ref float baseValue)
        // {
        //     //if (modifierType == ModifierType.PercentMult)
        //         baseValue *= value.ModifiedValue;
        // }
        // #endregion
    }
}