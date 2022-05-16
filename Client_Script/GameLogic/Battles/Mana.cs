using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kame.Battles
{
    public class Mana : MonoBehaviour
    {
        float manaRegenRate = 1f;

        public event Action<float> OnMpChanged;

        public float MaxMP { get; set; } = 10;

        public float MP
        {
            get => mp;
            set
            {
                mp = value;

                if (mp > MaxMP)
                {
                    mp = MaxMP;
                }
                OnMpChanged?.Invoke(Mathf.Max(0, mp / MaxMP));
            }
        }

        float mp;


        private void Awake()
        {
            MP = MaxMP;
        }

        private void Update()
        {
            if (mp < MaxMP)
            {
                MP += manaRegenRate * Time.deltaTime;
                if (MP > MaxMP)
                {
                    MP = MaxMP;
                }
            }
        }

        public bool UseMana(float manaToUse)
        {
            if (manaToUse > MP)
            {
                return false;
            }
            MP -= manaToUse;
            return true;
        }



    }
}
