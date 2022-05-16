using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using UnityEngine;

namespace Kame.Game.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class CharacterStat : Stat
    {
        #region SerializeVariable
        [JsonProperty][SerializeField]
        public int id;
        [JsonProperty][SerializeField] 
        public int level;
        [JsonProperty][SerializeField] 
        protected string totalExp;   //temporary variable for serializing
        #endregion

        #region Exp
        public BigInteger TotalExp;
        public event Action<CharacterStat> OnLevelChanged;
        public event Action<BigInteger, BigInteger> OnExpChange;
        protected BigInteger currentExp;
        public BigInteger Exp
        {
            get { return currentExp; }
            set
            {
                currentExp = value;
                int level = this.level;
                if (currentExp >= this.TotalExp)
                {
                    CharacterStat tempStat;
                    while (true)
                    {
                        if (DataManager.TryGetStat(id, level + 1, out tempStat) == false)
                            break;
                        if (currentExp < tempStat.TotalExp)
                            break;
                        ++level;
                    }
                    if (level != this.level)
                    {
                        this.level = level;
                        ChangeBaseValue(tempStat);
                        OnLevelChanged?.Invoke(this);
                    }
                }
                OnExpChange?.Invoke(currentExp, TotalExp);
            }
        }
        #endregion
        
        #region HP
        protected float _hp;
        public float MaxHp => GetModifiedValue(StatType.HP);
        public event Action<float, float> OnHpChanged;

        public float Hp
        {
            get => _hp;
            set
            {
                _hp = Mathf.Clamp(value, 0, MaxHp);
                OnHpChanged?.Invoke(_hp, MaxHp);
            }
        }
        #endregion
        
        public CharacterStat() { }

        public CharacterStat(CharacterStat other) : base(other)
        {
            this.id = other.id;
            this.level = other.level;
            this.TotalExp = other.TotalExp;
            this._hp = other._hp;
        }

        public CharacterStat ChangeBaseValue(CharacterStat other)
        {
            base.ChangeBaseValue(other);
            this.id = other.id;
            this.level = other.level;
            this.TotalExp = other.TotalExp;
            this._hp = other._hp;
            return this;
        }

        [OnSerializing]
        public void OnBeforeSerializeCharacterStat(StreamingContext context)
        {
            BigInt2Str();
        }

        [OnDeserialized]
        public void OnAfterDeserializeCharacterStat(StreamingContext context)
        {
            Str2BigInt();
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
            BigInt2Str();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            Str2BigInt();
        }

        private void BigInt2Str()
        {
            totalExp = TotalExp.ToString();
        }

        private void Str2BigInt()
        {
            if (!string.IsNullOrEmpty(totalExp))
                TotalExp = BigInteger.Parse(totalExp);
            totalExp = null;
        }
    }
}
