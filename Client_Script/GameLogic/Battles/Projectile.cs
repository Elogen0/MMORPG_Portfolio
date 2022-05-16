using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;

    public float damage;
    //총구에서 나오는 이펙트
    public GameObject muzzlePrefab;
    //맞는 순간의 이펙트
    public GameObject hitPrefab;
    //쏠때 소리
    public AudioClip shotSFX;
    //맞을때 소리
    public AudioClip hitSFX;

    //적에 한번 부딪쳤는가
    private bool collided;
    private Rigidbody _rigidbody;
    public GameObject owner { get; set; }
    public GameObject target { get; set; }
    public LayerMask layer { get; set; }

    public void Setup(GameObject owner, GameObject target, float speed, float damage, LayerMask mask)
    {
        this.owner = owner;
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.layer = mask;
    }
    
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer != layer)
        {
            return;
        }

        if (other.TryGetComponent(out Health helth))
        {
            helth.TakeDamage(owner, damage);
            Destroy(gameObject);
        }
    }

}
