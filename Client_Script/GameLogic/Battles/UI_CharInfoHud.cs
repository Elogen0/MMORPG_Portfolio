using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Kame.Define;
using Kame.Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharInfoHud : UI_Base
{
    private TransformAnchor _playerAnchor;

    enum Texts
    {
        tmp_exp,
        tmp_level,
        tmp_hp
    }

    enum Img
    {
        img_hp,
        img_hp_tick,
        img_exp
    }

    private Image hp;
    private Image hpTick;
    private Image exp;
    private TextMeshProUGUI expTmp;
    private TextMeshProUGUI levelTmp;
    private TextMeshProUGUI hpTmp;

    private float currentHealthPercentage;
    private Health health;
    public override void Init()
    {
        _playerAnchor = TransformAnchor.Get<TransformAnchor>(ResourcePath.PlayerTransformAnchor);
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Img));
        
        hp = GetImage((int) Img.img_hp);
        hpTick = GetImage((int) Img.img_hp_tick);
        exp = GetImage((int) Img.img_exp);

        expTmp = GetText((int) Texts.tmp_exp);
        levelTmp = GetText((int) Texts.tmp_level);
        hpTmp = GetText((int) Texts.tmp_hp);
    }

    private void OnEnable()
    {
        _playerAnchor.OnAnchorProvided += Register;
        Register();
    }
    
    private void OnDisable()
    {
        _playerAnchor.OnAnchorProvided -= Register;
    }

    private void Update()
    {
        // if (!Mathf.Approximately(hp.fillAmount, hpTick.fillAmount))
        // {
        //     hpTick.fillAmount = Mathf.Lerp(hpTick.fillAmount, hp.fillAmount, 0.3f);
        // }
    }

    private void Register()
    {
        if (!_playerAnchor.isSet)
            return;
        health = _playerAnchor.Value.GetComponent<Health>();
        health.Stat.OnHpChanged += OnHealthChanged;
        OnHealthChanged(health.Stat.Hp, health.Stat.MaxHp);

        health.Stat.OnExpChange += OnExpChanged;
        
        health.Stat.OnLevelChanged += OnLevelChanged;
        OnLevelChanged(health.Stat);
    }
    
    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        hp.fillAmount = currentHealth / maxHealth;
        hpTmp.text = $"{currentHealth}/{maxHealth}";
        DOTween.To(() => hpTick.fillAmount, x => hpTick.fillAmount = x, hp.fillAmount, 0.3f).SetEase(Ease.InExpo);
    }

    private void OnExpChanged(BigInteger currentExp, BigInteger totalExp)
    {
        int e = (int)(totalExp - currentExp);
    }

    private void OnLevelChanged(CharacterStat stat)
    {
        levelTmp.text = stat.level.ToString();
    }
}
