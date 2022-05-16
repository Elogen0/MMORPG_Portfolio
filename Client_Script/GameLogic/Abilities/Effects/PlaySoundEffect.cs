using System;
using System.Collections;
using System.Collections.Generic;
using Kame.Abilities;
using Kame.Sounds;
using UnityEngine;

[CreateAssetMenu(fileName = "New Play Sound Effect", menuName = "Abilities/Effects/PlaySound", order = 0)]
public class PlaySoundEffect : EffectStrategy
{
    [SerializeField] private SoundList _soundList;

    public override void Init()
    {
        SoundManager.Instance.Load(_soundList);
    }

    public override void StartEffect(AbilityData data, Action finished)
    {
        SoundManager.Instance.Play(_soundList, data.TargetedPoint);
    }
}
