using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectable : MonoBehaviour
{
    private Collider coll;
    private Rigidbody rb;

    public string ItemTag = "";
    public int Count = 1;

    public float FollowDuration = 1f;

    private void Awake()
    {
        coll = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        coll.enabled = true;
        rb.isKinematic = false;
    }

    private void OnDisable()
    {
        
    }

    public void Take(Transform owner)
    {
        coll.enabled = false;
        rb.isKinematic = true;

        transform.DOMove(owner.position, FollowDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(OnAnimationComplete);
    }

    private void OnAnimationComplete()
    {
        if(ItemsManager.instance != null)
        {
            ItemsManager.instance.TakeItem(ItemTag, Count);
        }
        else
        {
            Debug.LogError("ItemsManager instance not found");
        }
    }
}
