using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class StageSector : MonoBehaviour
{
    [SerializeField] LayerMask detectLayerMask;
    [SerializeField] GameObject[] enemies;
    [SerializeField] GameObject[] props;
    
    public bool IsCleared { get; set; } = false;

    public event Action<Health> OnDeadEvent;
    public event Action OnClearSectorEvent;
    
    private void Awake()
    {
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<Health>().OnDeadEvent += CallbackEnemyDeath;
            enemy.SetActive(false);
        }
        foreach (var prop in props)
        {
            prop.SetActive(false);
        }
    }

    public void Spawn()
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(true);
        }
        foreach (var prop in props)
        {
            prop.SetActive(true);
        }
    }
    
    private void CallbackEnemyDeath(Health enemyHealth)
    {
        OnDeadEvent?.Invoke(enemyHealth);
        foreach (var enemy in enemies)
        {
            if (!enemy.GetComponent<Health>().IsDead)
            {
                return;
            }
        }
        IsCleared = true;
        OnClearSectorEvent?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer != detectLayerMask)
            return;
        
        //Trigger Sector
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1);
    }
#endif
}
