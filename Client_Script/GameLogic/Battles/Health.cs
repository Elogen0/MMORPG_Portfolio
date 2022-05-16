using Kame.Battles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Google.Protobuf.Protocol;
using Kame;
using Kame.Define;
using Kame.Game.Data;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using StatType = Kame.Game.Data.StatType;
using Vector3 = UnityEngine.Vector3;

public class Health : MonoBehaviour
{
    public Vector3 heartPosition;
    public bool showHealthBar = true;
    public bool IsDead { get; private set; } = false;
    public Vector3 LookAtPoint => transform.position + heartPosition;
    public event Action<float, float> OnHpChanged;
    public event Action<Health> OnDeadEvent;
    public event Action<GameObject, float> OnDamaged;
    public event Action<StatValue> OnStatChanged;

    private CharacterStat _stat { get; }= new CharacterStat();
    public CharacterStat Stat => _stat;
    private UI_HealthBar attachedHealthBar;

    public virtual void InitStat(StatInfo value)
    {
        if (Stat.id == value.CharacterId && Stat.level == value.Level)
            return;

        GameEntityType type = DataManager.GetEntityType(value.CharacterId);
        if (type == GameEntityType.Player)
        {
            if (DataManager.TryGetStat(value.CharacterId, value.Level, out CharacterStat stat))
            {
                Stat.ChangeBaseValue(stat);
                Stat.Hp = value.Hp;
                Stat.Exp = BigInteger.Parse(value.Exp);
            }    
        }
        else if (type == GameEntityType.Monster || type == GameEntityType.NPC || type == GameEntityType.Object)
        {
            if (DataManager.EntityDict.TryGetValue(value.CharacterId, out EntityData data))
            {
                Stat.ChangeBaseValue(data.stat);
                Stat.Hp = value.Hp;
                Stat.Exp = BigInteger.Parse(value.Exp);
            }    
        }
        
        Speed = Stat.GetModifiedValue(StatType.MOVE_SPEED);
    }
    public float Speed { get; private set; }
    
    private void Awake()
    {
        _stat.OnHpChanged += HpChanged;
    }

    public void OnEnable()
    {
        _stat.RegisterStatModifiedEvent(StatChanged);

        if (showHealthBar)
        {
            ObjectSpawner.Instance.SpawnAsync(gameObject, UI_HealthBar.HealthBarPath, null, Vector3.up * 2f, go =>
            {
                attachedHealthBar = go.GetComponent<UI_HealthBar>();
            });
        }
    }

    public void OnDisable()
    {
        _stat.UnRegisterStatModifiedEvent(StatChanged);
        if (attachedHealthBar)
             AddressableLoader.ReleaseInstance(attachedHealthBar.gameObject);
    }

    public void TakeDamage(GameObject instigator, float damage)
    {
        if (IsDead)
            return;
        _stat.Hp -= damage;
        OnDamaged?.Invoke(instigator, damage);
    }

    public void Heal(GameObject instigator, float healAmount)
    {
        if (IsDead)
            return;
        _stat.Hp += healAmount;
    }

    public void Resurrection(GameObject instigator, float healAmount)
    {
        _stat.Hp += healAmount;
        IsDead = false;
        OnDeadEvent?.Invoke(this);
    }

    private void StatChanged(StatValue stat)
    {
        OnStatChanged?.Invoke(stat);
        if (stat.type == StatType.MOVE_SPEED)
            Speed = stat.value.ModifiedValue;
    }

    private void HpChanged(float currentHealth, float maxHealth)
    {
        OnHpChanged?.Invoke(currentHealth, maxHealth);
        if (IsDead == false && currentHealth <= 0)
        {
            IsDead = true;
            OnDeadEvent?.Invoke(this);
        }

        if (IsDead == true && currentHealth > 0)
        {
            IsDead = false;
        }
    }

#if UNITY_EDITOR
    public float lookAtPointRadius = .5f;
    public bool visibleGizmo = false;
    private void OnDrawGizmosSelected()
    {
        if (visibleGizmo)
            Gizmos.DrawSphere(LookAtPoint, lookAtPointRadius);
    }
#endif
}
