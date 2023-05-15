using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Tree : MonoBehaviour
{
    public float ShakeDuration = 0.25f;
    public Vector3 Punch;

    public void OnTakeDamageImpact()
    {
        transform.DOPunchRotation(Punch, ShakeDuration);
    }

    
}
