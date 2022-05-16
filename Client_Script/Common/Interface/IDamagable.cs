using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    bool IsDead { get; }
    void TakeDamage(GameObject instigator, float damage);
}
