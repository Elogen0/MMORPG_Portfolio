using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Kame.Game.Data
{
    [System.Serializable] [JsonObject(MemberSerialization.OptIn)]
    public abstract class ModifiableBase<T>
    {
        [JsonProperty][SerializeField]
        protected T baseValue;
        [NonSerialized] protected T modifiedValue;
        [NonSerialized] private bool isDirty = true;
        public T BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                SetDirty();
            }
        }
        public T ModifiedValue
        {
            get
            {
                if (isDirty)
                {
                    UpdateModifiedValue();
                    isDirty = false;
                }
                return modifiedValue;                
            }
        }

        protected event Action<ModifiableBase<T>> OnModifiedValue;
        [NonSerialized] protected List<IModifier<T>> _modifiers = new List<IModifier<T>>();

        public ModifiableBase() { }

        public ModifiableBase(T baseValue)
        {
            this.baseValue = baseValue;
        }

        public ModifiableBase(ModifiableBase<T> other)
        {
            this.baseValue = other.baseValue;
            this.modifiedValue = other.modifiedValue;
            foreach (var modifier in other._modifiers)
            {
                this._modifiers.Add(modifier);
            }
            SetDirty();
        }
        
        public void RegisterModEvent(Action<ModifiableBase<T>> action)
        {
            if (action != null)
            {
                OnModifiedValue += action;
            }
        }
        
        public void UnRegisterModEvent(Action<ModifiableBase<T>> action)
        {
            if (action != null)
            {
                OnModifiedValue -= action;
            }
        }

        public void AddModifier(IModifier<T> modifier)
        {
            _modifiers.Add(modifier);
            SetDirty();
        }
        
        public void RemoveModifier(IModifier<T> modifier)
        {
            _modifiers.Remove(modifier);
            SetDirty();
        }

        protected void SetDirty()
        {
            isDirty = true;
            OnModifiedValue?.Invoke(this);
        }
        protected abstract void UpdateModifiedValue();
    }
    
    [System.Serializable]
    public class ModifiableFloat : ModifiableBase<float>
    {
        public ModifiableFloat() { }

        public ModifiableFloat(float baseValue) : base(baseValue){ }

        public ModifiableFloat(ModifiableFloat other) : base(other) { }
        protected override void UpdateModifiedValue()
        {
            //((baseValue + Add) * (PercentAdd + ...)) * PercentMult * ...
            float value = baseValue;
            foreach (var modifier in _modifiers)
            {
                modifier.AddValue(ref value);
            }

            float percentAdd = 0;
            foreach (var modifier in _modifiers)
            {
                modifier.PercentAddValue(ref percentAdd);
            }
            value = value * (1 + percentAdd);

            foreach (var modifier in _modifiers)
            {
                modifier.PercentMultValue(ref value);
            }
            modifiedValue = value;
        }
    }
    
    [System.Serializable]
    public class ModifiableInt : ModifiableBase<int>
    {
        public ModifiableInt() { }

        public ModifiableInt(int baseValue) : base(baseValue){ }

        public ModifiableInt(ModifiableInt other) : base(other) { }
        protected override void UpdateModifiedValue()
        {
            //((baseValue + Add) * (PercentAdd + ...)) * PercentMult * ...
            int value = baseValue;
            foreach (var modifier in _modifiers)
            {
                modifier.AddValue(ref value);
            }

            int percentAdd = 0;
            foreach (var modifier in _modifiers)
            {
                modifier.PercentAddValue(ref percentAdd);
            }
            value *= (1 + percentAdd);

            foreach (var modifier in _modifiers)
            {
                modifier.PercentMultValue(ref value);
            }

            modifiedValue = value;
        }
    }
    
}
