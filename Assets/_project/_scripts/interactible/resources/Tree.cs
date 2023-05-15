using UnityEngine;
using DG.Tweening;

public class Tree : MonoBehaviour
{
    private Destroyable _destroyable;

    public GameObject baseSkin;
    public GameObject deadSkin;
    public float ShakeDuration = 0.25f;
    public Vector3 Punch;

    private void Awake()
    {
        _destroyable = GetComponent<Destroyable>();
    }

    public void OnTakeDamageImpact()
    {
        transform.DOPunchRotation(Punch, ShakeDuration);
    }

    public void OnKill()
    {
        baseSkin.SetActive(false);
        deadSkin.SetActive(true);

        Vector3 treePosition = transform.position;
        Vector3 playerPosition = _destroyable.Attacker.position;
        Vector3 upVector = Vector3.up;

        Vector3 direction = treePosition - playerPosition;
        Quaternion rotation = Quaternion.AngleAxis(90f, Vector3.up);
        Vector3 fallDirection = Vector3.Cross(direction, upVector).normalized;
        Vector3 newFallDirection = rotation * fallDirection;
        Quaternion finalRotation = Quaternion.FromToRotation(upVector, newFallDirection);
        Vector3 finalRotationVector = finalRotation.eulerAngles;

        transform.DORotate(finalRotationVector, 4f, RotateMode.Fast).SetEase(Ease.InCubic);
    }
}
