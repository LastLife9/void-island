using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jetpack : MonoBehaviour
{
    private PlayerMove _playerMove;

    [SerializeField] private GameObject flyBtn;
    [SerializeField] private GameObject jetpackObj;
    [SerializeField] private Slider capacitySlider;

    public float Capacity = 100f;
    public float CapacityPerSec = 2f;

    private float _capacity = 0f;

    public bool Enable = false;
    private bool isFlying = false;

    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        _capacity = Capacity;
    }

    private void Update()
    {
        if (_playerMove.GetMoveType().Equals(MoveType.Fly)) isFlying = true;
        else isFlying = false;

        if (flyBtn.activeSelf != Enable) flyBtn.SetActive(Enable);

        if(jetpackObj.activeSelf != isFlying) jetpackObj.SetActive(isFlying);

        capacitySlider.value = _capacity;

        if (_capacity <= 0 && Enable)
        {
            Enable = false;
            _capacity = 0;
            _playerMove.MoveStateChange(MoveType.Walk);
            return;
        }

        if (isFlying)
        {
            _capacity -= CapacityPerSec * Time.deltaTime;
        }
    }

    public void VirtualFlyInput()
    {
        _playerMove.MoveStateChange(MoveType.Fly);
    }
    public void VirtualWalkInput()
    {
        _playerMove.MoveStateChange(MoveType.Walk);
    }
}
