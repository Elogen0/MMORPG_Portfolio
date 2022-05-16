using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Game.Data;
using Kame;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UI_Stats : UI_Base
{
    private CharacterStat _characterStat;
    
    enum Texts
    {
        tmp_display_stat
    }
    
    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
    }

    public void SetUp(CharacterStat inspectStat)
    {
        _characterStat?.UnRegisterStatModifiedEvent(OnChangeStat);
        _characterStat = inspectStat;
        _characterStat.RegisterStatModifiedEvent(OnChangeStat);
    }

    private void OnEnable()
    {
        MyPlayerController myPlayer = ObjectManager.Instance.MyPlayer;
        if (myPlayer == null)
            return;
        SetUp(myPlayer.Stat);
        UpdateAttributeText();
        //equipment = GameObject.FindWithTag(TagAndLayer.TagName.Player).GetComponent<PlayerController>().equipment;
        //_stats = GameObject.FindWithTag(TagAndLayer.TagName.Player).GetComponent<PlayerController>().Stats;
    }

    private void OnDisable()
    {
        if (_characterStat != null)
            _characterStat.UnRegisterStatModifiedEvent(OnChangeStat);
    }

    private void OnChangeStat(StatValue obj)
    {
        UpdateAttributeText();
    }

    void UpdateAttributeText()
    {
        if (_characterStat == null)
            return;
        Get<TextMeshProUGUI>((int) Texts.tmp_display_stat).text =
            $"HP : {_characterStat.GetModifiedValue(StatType.HP)} \t SPD : {_characterStat.GetModifiedValue(StatType.MOVE_SPEED)}\n " +
            $"ATK : {_characterStat.GetModifiedValue(StatType.ATK)} \t DEF : {_characterStat.GetModifiedValue(StatType.DEF)}";
    }
}
