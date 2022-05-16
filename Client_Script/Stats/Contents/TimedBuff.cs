using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using Kame;
using Kame.Game.Data;
using UnityEngine;

[Serializable]
public class TimedBuff : ItemBuff
{
    public float duration;
    public bool IsFinished { get; set; }
    public event Action<TimedBuff> EndBuffEvent;
    public void Tick(float deltaTime)
    {
        duration -= deltaTime;
        if (duration <= 0)
        {
            EndBuffEvent?.Invoke(this);
        }
    }
}
