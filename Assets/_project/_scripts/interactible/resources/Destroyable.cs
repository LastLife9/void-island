﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Destroyable : MonoBehaviour
{
    private Animator anim;

    public UnityEvent OnDestroyEvent;
    public UnityEvent OnTakeDamageEvent;

    public float Health;
    private float _health;

    public bool _alive;

    private int _animIDTakeDmg;
    private int _animIDDestroy;

    public Transform Attacker => _attacker;
    private Transform _attacker;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _health = Health;
        AssignAnimationIDs();
    }

    public void TakeDamage(float dmg, Transform attacker = null)
    {
        if (!_alive) return;

        _health -= dmg;
        if (_health <= 0f)
        {
            DestroyResurce();
        }

        //if(anim) anim.SetTrigger(_animIDTakeDmg);
        OnTakeDamageEvent?.Invoke();

        if (attacker != null) _attacker = attacker;
    }

    public void DestroyResurce()
    {
        //if (anim) anim.SetTrigger(_animIDDestroy);
        OnDestroyEvent?.Invoke();
        _alive = false;
    }

    private void AssignAnimationIDs()
    {
        if (anim == null) return;

        _animIDTakeDmg = Animator.StringToHash("AttackImpact");
        _animIDDestroy = Animator.StringToHash("Destroy");
    }
}
