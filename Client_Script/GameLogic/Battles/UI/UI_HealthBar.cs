using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kame.Define;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

namespace Kame.Battles
{
    public class UI_HealthBar : UI_FollowedObject
    {
        [SerializeField] float visibleTime = 3f;
        protected float timer;
        protected Image hp;
        protected Image hpTick;
        
        public const string HealthBarPath = "Assets/Game/Prefab/UI/HealthBar.prefab";

        enum Img
        {
            hp,
            hpTick,
        }
        public override void Init()
        {
            base.Init();
            Bind<Image>(typeof(Img));
            hp = GetImage((int) Img.hp);
            hpTick = GetImage((int) Img.hpTick);
        }

        protected void OnDisable()
        {
            if (!follower)
                return;
            if (follower.TryGetComponent(out Health health))
            {
                health.OnHpChanged -= OnHealthChanged;
            }
        }
        
        private void OnHealthChanged(float currentHealth, float maxHealth)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            hp.fillAmount = currentHealth / maxHealth;
            timer = visibleTime;
            DOTween.To(() => hpTick.fillAmount, x => hpTick.fillAmount = x, hp.fillAmount, 0.3f).SetEase(Ease.InExpo);
        }


        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                //gameObject.SetActive(false);
            }
        }

        public override IEnumerator AfterSpawnAction(GameObject caller, object data)
        {
            Setup(caller.transform, Vector3.up * 2f);
            
            if (follower.TryGetComponent(out Health health))
            {
                health.OnHpChanged += OnHealthChanged;
                OnHealthChanged(health.Stat.Hp, health.Stat.MaxHp);
            }
            
            transform.SetParent(UIManager.Instance.WorldSpaceCanvas.transform, false);
            yield return null;
        }
    }

}
