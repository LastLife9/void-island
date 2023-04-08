using UnityEngine;

[CreateAssetMenu(fileName = "New move settings")]
public class MoveSettings : ScriptableObject
{
    public MoveType Reference;

    [Header("Move")]
    public Vector2 HorizontalLimits;
    public Vector2 VerticalLimits;
    public float Speed;
    public float SmoothSpeedMoment;
    [Header("Jump")]
    public float JumpHeight;
    public float JumpTimeout;
    public float FallTimeout;
    [Header("Camera")]
    public float CharacterRotationSpeed;
    public float CameraSmoothTimer;
    public float TopClamp;
    public float BottomClamp;
}

public enum MoveType
{
    Walk,
    Fly
}