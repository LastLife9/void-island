using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableResource : MonoBehaviour
{
    private Animator anim;

    public UnityEvent OnDestroyEvent;
    public UnityEvent OnTakeDamageEvent;

    public float Health = 100f;
    private float _health = 0f;

    private int _animIDTakeDmg;
    private int _animIDDestroy;

    private bool alive = true;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        AssignAnimationIDs();

        _health = Health;
    }


    public void TakeDamage(float dmg)
    {
        if (!alive) return;

        _health -= dmg;
        if(_health <= 0f)
        {
            DestroyResurce();
        }

        OnTakeDamageEvent?.Invoke();
        anim?.SetTrigger(_animIDTakeDmg);
    }

    public void DestroyResurce()
    {
        alive = false;

        anim?.SetTrigger(_animIDDestroy);
        OnDestroyEvent?.Invoke();
    }

    private void AssignAnimationIDs()
    {
        if (anim == null) return;

        _animIDTakeDmg = Animator.StringToHash("TakeDamge");
        _animIDDestroy = Animator.StringToHash("Destroy");
    }
}
